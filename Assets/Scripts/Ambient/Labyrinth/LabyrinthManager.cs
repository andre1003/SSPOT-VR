using SSpot.Grids;
using SSpot.Level;
using SSPot.Level;
using SSpot.Utilities;
using UnityEngine;

namespace SSPot.Ambient.Labyrinth
{
    public class LabyrinthManager : MonoBehaviour
    {
        [Header("Environment")]
        [SerializeField] private GameObject projector;
        [SerializeField] private GameObject invisibleWall;
        
        [Header("Objectives")]
        [SerializeField] private GridObject objective;
        [SerializeField] private GridObject[] switchPlayerPoints;
        [SerializeField, TextArea] private string wrongMoveError = "Caminho errado!";

        [Header("Players")]
        [SerializeField] private LabyrinthPlayer player1;
        [SerializeField] private LabyrinthPlayer player2;
        
        private LabyrinthPlayer _currentWatcher;
        private LabyrinthPlayer _currentCoder;
        
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
        
        private void Start()
        {
            int localIndex = PlayerSetup.Local.PlayerIndex;
            player1.Init(0, localIndex);
            player2.Init(1, localIndex);

            player1.SetCoder(true);
            _currentCoder = player1;
        
            player2.SetCoder(false);
            _currentWatcher = player2;
            
            switchPlayerPoints.ForEach(c => c.SteppedOnEvent.AddListener(SwitchPlayers));
            objective.SteppedOnEvent.AddListener(ReportSuccess);
        }
        
        private void OnEnable()
        {
            LevelManager.Instance.Robot.Mover.OnFailedToMove += ReportWrongMove;
            LevelManager.Instance.OnSuccess.AddListener(LabyrinthSuccess);
        }

        private void OnDisable()
        {
            if (!LevelManager.Instance) return;
            
            LevelManager.Instance.Robot.Mover.OnFailedToMove -= ReportWrongMove;
            LevelManager.Instance.OnSuccess.RemoveListener(LabyrinthSuccess);
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