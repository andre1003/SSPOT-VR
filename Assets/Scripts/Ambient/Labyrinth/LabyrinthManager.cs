using System;
using Photon.Pun;
using SSpot.Robot;
using UnityEngine;
using UnityEngine.UI;

namespace SSPot.Ambient.Labyrinth
{
    public class LabyrinthManager : MonoBehaviour
    {
        #region Singleton
        public static LabyrinthManager Instance { get; private set; }

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }
        
            Instance = this;
        }
        #endregion

        //TODO remove all these old fields
        [SerializeField] private GameObject computer1;
        [SerializeField] private GameObject computer2;

        [SerializeField] private GameObject roof1;
        [SerializeField] private GameObject roof2;

        [SerializeField] private GameObject tv1;
        [SerializeField] private GameObject tv2;

        [SerializeField] private GameObject instructions1;
        [SerializeField] private GameObject instructions2;

        [SerializeField] private Text instructionPlayer1;
        [SerializeField] private Text instructionPlayer2;

        [Header("Environment")] 
        [SerializeField] private RobotData robot;
        [SerializeField] private GameObject projector;
        [SerializeField] private GameObject invisibleWall;
        
        [Header("Checkpoints")]
        [Tooltip("If true, the player is allowed to activate checkpoints older than the current one.")]
        [SerializeField] private bool allowActivatingPrevious;
        [SerializeField] private Checkpoint[] checkpoints;
        private int _checkpointIndex = 0;

        [Header("Players")]
        [SerializeField] private LabyrinthPlayer player1;
        [SerializeField] private LabyrinthPlayer player2;
        
        private LabyrinthPlayer _currentWatcher;
        private LabyrinthPlayer _currentCoder;

        private void SwitchPlayers()
        {
            //TODO ideally instructions should be stored in the manager itself
            string watcherInstructions = _currentWatcher.InstructionsText.text;
            string coderInstructions = _currentCoder.InstructionsText.text;
            _currentWatcher.SetInstructions(coderInstructions);
            _currentCoder.SetInstructions(watcherInstructions);
            
            _currentWatcher.SetCoder(true);
            _currentCoder.SetCoder(false);
            (_currentWatcher, _currentCoder) = (_currentCoder, _currentWatcher);
        }
        
        private void Start()
        {
            // Find local player's index on network
            int localIndex = Array.FindIndex(PhotonNetwork.PlayerList, player => player.IsLocal);
        
            player1.Init(0, localIndex);
            player2.Init(1, localIndex);

            player1.SetCoder(true);
            _currentCoder = player1;
        
            player2.SetCoder(false);
            _currentWatcher = player2;
        }

        public void SetCheckpoint(Checkpoint checkpoint)
        {
            int newIndex = Array.IndexOf(checkpoints, checkpoint);
            if (!allowActivatingPrevious && newIndex < _checkpointIndex)
                return;

            _checkpointIndex = newIndex;
            SwitchPlayers();
        }

        public void LabyrinthSuccess()
        {
            // Reset computer
            //TODO fix
            //_currentCoder.RunCubes.ResetComputer();

            // Deactivate objects for both players
            player1.SetActive(false);
            player2.SetActive(false);

            // Activate projector
            projector.SetActive(true);

            // Destroy invisible wall
            Destroy(invisibleWall);
        }

        public void ResetRobot()
        {
            //_currentCoder.ResetCubes.Reset();

            robot.Animator.Reset();
            
            var checkpoint = checkpoints[_checkpointIndex];
            robot.Mover.Facing = checkpoint.Facing;
            robot.Mover.ChangeNode(checkpoint.GridPosition);
        }
    }
}