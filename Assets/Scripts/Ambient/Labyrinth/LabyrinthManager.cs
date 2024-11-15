using System;
using SSpot.Grids;
using SSpot.Level;
using SSPot.Level;
using SSPot.Utilities;
using UnityEngine;

namespace SSPot.Ambient.Labyrinth
{
    public class LabyrinthManager : MonoBehaviour
    {
        [Serializable]
        private struct Section
        {
            public GridObject endMarker;
            public GameObject coveringRoof;
        }
        
        [Header("Environment")]
        [SerializeField] private GameObject projector;
        [SerializeField] private GameObject invisibleWall;
        
        [Header("Objectives")]
        [SerializeField] private GridObject objective;
        [SerializeField] private Section[] sections = Array.Empty<Section>();
        [SerializeField, TextArea] private string wrongMoveError = "Caminho errado!";

        [Header("Players")]
        [SerializeField] private LabyrinthPlayer player1;
        [SerializeField] private LabyrinthPlayer player2;
        
        private LabyrinthPlayer _currentWatcher;
        private LabyrinthPlayer _currentCoder;

        private int _currentSectionIndex = -1;
        private int CurrentSectionIndex
        {
            get => _currentSectionIndex;
            set
            {
                // Clears listener for old section
                if (_currentSectionIndex >= 0)
                {
                    sections[_currentSectionIndex].endMarker.SteppedOnEvent.RemoveListener(OnSectionEndReached);
                }
                
                // Uncovers and listens to new section
                if (value >= 0 && value < sections.Length)
                {
                    sections[value].endMarker.SteppedOnEvent.AddListener(OnSectionEndReached);
                    sections[value].coveringRoof.SetActive(false);
                }
                
                _currentSectionIndex = value;
            }
        }
        
        private void ReportWrongMove() => LevelManager.Instance.ReportResult(LevelResult.Error(wrongMoveError));
        
        private static void ReportSuccess() => LevelManager.Instance.ReportResult(LevelResult.Success());

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

        private void OnSectionEndReached()
        {
            CurrentSectionIndex++;
            SwitchPlayers();
        }
        
        private void Start()
        {
            // Initialize players
            int localIndex = PlayerSetup.Local.PlayerIndex;
            player1.Init(0, localIndex);
            player2.Init(1, localIndex);

            player1.SetCoder(true);
            _currentCoder = player1;
        
            player2.SetCoder(false);
            _currentWatcher = player2;
            
            // Cover all sections and begin listening to the first
            sections.ForEach(s => s.coveringRoof.SetActive(true));
            CurrentSectionIndex = 0;
            
            // Listen to objective
            objective.SteppedOnEvent.AddListener(ReportSuccess);
        }
        
        private void OnEnable()
        {
            LevelManager.Instance.OnSuccess.AddListener(LabyrinthSuccess);
            LevelManager.Instance.Robot.Mover.OnFailedToMove += ReportWrongMove;
        }

        private void OnDisable()
        {
            if (!LevelManager.Instance) return;
            LevelManager.Instance.OnSuccess.RemoveListener(LabyrinthSuccess);
            
            if (!LevelManager.Instance.Robot) return;
            LevelManager.Instance.Robot.Mover.OnFailedToMove -= ReportWrongMove;
        }

        private void LabyrinthSuccess()
        {
            // Deactivate objects for both players
            player1.SetObjectsActive(false);
            player2.SetObjectsActive(false);

            // Activate projector
            projector.SetActive(true);

            // Destroy invisible wall
            Destroy(invisibleWall);
        }
    }
}