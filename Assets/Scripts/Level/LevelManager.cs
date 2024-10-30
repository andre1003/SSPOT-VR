using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SSpot.ComputerCode;
using SSpot.Objectives;
using SSpot.Robot;
using SSpot.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace SSpot.Level
{
    public class LevelManager : NetworkedSingleton<LevelManager>
    {
        #region Serialized Properties
        
        [field: SerializeField]
        public RobotData Robot { get; private set; }
        
        [field: SerializeField]
        public LevelObjectiveSolver Objective { get; private set; }
        
        [SerializeField] private CubeCompiler compiler;
        
        [SerializeField] private CubeRunner runner;
        
        #endregion
        
        #region Events
        
        //Consider using event bus ScriptableObjects
        
        [field: Header("Events"), SerializeField]
        public UnityEvent OnReset { get; private set; } = new();

        [field: SerializeField]
        public UnityEvent OnError { get; private set; } = new();
        
        [field: SerializeField]
        public UnityEvent OnSuccess { get; private set; } = new();
        
        [field: SerializeField]
        public UnityEvent OnStartRunning { get; private set; } = new();
        
        [field: SerializeField]
        public UnityEvent OnFinishRunning { get; private set; } = new();
        
        [field: SerializeField]
        public UnityEvent OnLevelCompleted { get; private set; } = new();
                
        #endregion

        public bool IsRunning { get; private set; }
        
        public ObjectiveResult CurrentResult { get; private set; } = ObjectiveResult.None();
        
        private Coroutine _runCoroutine;

        protected void Start()
        {
            Objective.Init(HandleObjectiveResult);
        }

        #region Run Methods
        
        public void Run(IReadOnlyList<CodingCell> cells)
        {
            if (PhotonNetwork.OfflineMode)
                RunRpc(cells);
            else
                photonView.RPC(nameof(RunRpc), RpcTarget.AllBuffered, cells);
        }

        [PunRPC]
        private void RunRpc(IReadOnlyList<CodingCell> cells)
        {
            if (IsRunning)
                return;

            var compilation = compiler.Compile(cells);
            if (compilation.IsError)
            {
                var compilationError = ObjectiveResult.Error(compilation.Error, compilation.ErrorIndex);
                HandleObjectiveResult(compilationError);
                return;
            }

            var cubes = compilation.Result;
            
            Objective.EvaluateCubes(cubes);
            if (CurrentResult.Type == ObjectiveResult.ResultType.Error)
                return;

            IsRunning = true;
            OnStartRunning.Invoke();
            _runCoroutine = StartCoroutine(runner.RunCubesCoroutine(cubes, Robot, () =>
            {
                IsRunning = false;
                _runCoroutine = null;
                
                OnFinishRunning.Invoke();
                StartCoroutine(OnFinishRunningCoroutine());
            }));
        }

        private IEnumerator OnFinishRunningCoroutine()
        {
            yield return new WaitUntil(() => CurrentResult.Type != ObjectiveResult.ResultType.None);

            if (CurrentResult.Type == ObjectiveResult.ResultType.Success)
            {
                OnLevelCompleted.Invoke();
            }
        }
        
        #endregion
        
        #region Reset Methods
        
        public void Reset()
        {
            if (PhotonNetwork.OfflineMode)
                ResetRpc();
            else
                photonView.RPC(nameof(ResetRpc), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void ResetRpc()
        {
            if (!IsRunning)
                return;
            
            OnReset.Invoke();
            
            StopCoroutine(_runCoroutine);
            _runCoroutine = null;
            
            Robot.Reset();
            runner.Reset();
            
            IsRunning = false;
        }
        
        #endregion

        #region Result methods
        
        private void HandleObjectiveResult(ObjectiveResult result)
        {
            CurrentResult = result;
            switch (result.Type)
            {
                case ObjectiveResult.ResultType.Error:
                    Error();
                    break;
                case ObjectiveResult.ResultType.Success:
                    Success();
                    break;
            }
        }
        
        private void Error()
        {
            if (IsRunning)
            {
                StopCoroutine(_runCoroutine);
                _runCoroutine = null;
                IsRunning = false;
            }
            
            OnError.Invoke();
        }
        
        private void Success()
        {
            OnSuccess.Invoke();
        }
        
        #endregion
    }
}