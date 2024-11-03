using System;
using SSpot.Grids;
using SSpot.Level;
using SSPot.Level;
using SSpot.Utilities;
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
            //Stop listening to section
            if (_currentSectionIndex >= 0)
            {
                sections[_currentSectionIndex].endMarker.SteppedOnEvent.RemoveListener(OnSectionEndReached);
            }
            
            //Uncover next section and start listening
            _currentSectionIndex++;
            if (_currentSectionIndex < sections.Length)
            {
                sections[_currentSectionIndex].coveringRoof.SetActive(false);
                sections[_currentSectionIndex].endMarker.SteppedOnEvent.AddListener(OnSectionEndReached);
            }
        }
        
        private void Start()
        {
            int localIndex = PlayerSetup.Local.PlayerIndex;
            player1.Init(0, localIndex);
            player2.Init(1, localIndex);

            player1.SetCoder(true);
            _currentCoder = player1;
        
            player2.SetCoder(false);
            _currentWatcher = player2;
            
            //Cover all sections and begin listening to the first
            sections.ForEach(s => s.coveringRoof.SetActive(true));
            _currentSectionIndex = -1;
            OnSectionEndReached();
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