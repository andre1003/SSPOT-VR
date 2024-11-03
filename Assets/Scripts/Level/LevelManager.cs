using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SSpot.Ambient.ComputerCode;
using SSPot.Level;
using SSpot.Evaluators;
using SSpot.Robot;
using SSpot.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace SSpot.Level
{
    public class LevelManager : NetworkedSingleton<LevelManager>
    {
        #region Serialized Properties
        
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

        private RobotData _robot;

        public RobotData Robot
        {
            get
            {
                if (!_robot)
                    _robot = FindObjectOfType<RobotData>();
                
                if (!_robot)
                    Debug.LogError($"{nameof(RobotData)} not found in scene", gameObject);
                
                return _robot;
            }
        }

        public bool IsRunning => _runCoroutine != null;
        
        public LevelResult CurrentResult { get; private set; } = LevelResult.None();

        private CodeEvaluator[] _evaluators = Array.Empty<CodeEvaluator>(); 
        
        private Coroutine _runCoroutine;

        protected void Start()
        {
            _evaluators = GetComponentsInChildren<CodeEvaluator>();
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
                var compilationError = LevelResult.Error(compilation.Error, compilation.ErrorIndex);
                ReportResult(compilationError);
                return;
            }
            
            _evaluators.ForEach(e => e.EvaluatePreCompilation(cells));
            if (CurrentResult.Type == LevelResult.ResultType.Error)
                return;

            _evaluators.ForEach(e => e.EvaluatePostCompilation(compilation.Result));
            if (CurrentResult.Type == LevelResult.ResultType.Error)
                return;

            OnStartRunning.Invoke();
            _runCoroutine = StartCoroutine(runner.RunCubesCoroutine(compilation.Result, Robot, () =>
            {
                _runCoroutine = null;
                
                OnFinishRunning.Invoke();
                StartCoroutine(OnFinishRunningCoroutine());
            }));
        }

        //Waits until a result is reported if still haven't had any results
        private IEnumerator OnFinishRunningCoroutine()
        {
            yield return new WaitUntil(() => CurrentResult.Type != LevelResult.ResultType.None);

            if (CurrentResult.Type == LevelResult.ResultType.Success)
            {
                OnLevelCompleted.Invoke();
            }
        }
        
        #endregion
        
        #region Reset Methods
        
        public void ResetExecution()
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
            
            Robot.ResetRobot();
            runner.Reset();
        }
        
        #endregion

        #region Result methods
        
        public void ReportResult(LevelResult result)
        {
            CurrentResult = result;
            switch (result.Type)
            {
                case LevelResult.ResultType.Error:
                    Error();
                    break;
                case LevelResult.ResultType.Success:
                    Success();
                    break;
                case LevelResult.ResultType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void Error()
        {
            if (IsRunning)
            {
                StopCoroutine(_runCoroutine);
                _runCoroutine = null;
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