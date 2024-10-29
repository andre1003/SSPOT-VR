using System.Collections.Generic;
using SSpot.Level;
using UnityEngine;

namespace SSpot.Objectives
{
    public abstract class LevelObjectiveSolver : MonoBehaviour
    {
        private LevelObjectiveCallback _reportCallback;
        
        public void Init(LevelObjectiveCallback callback)
        {
            _reportCallback = callback;
        }
        
        protected void ReportResult(ObjectiveResult result) => _reportCallback(result);

        protected void ReportResultOnRunningEnd(ObjectiveResult result)
        {
            LevelManager.Instance.OnFinishRunning.AddListener(ReportDelayed);
            
            return;
            
            void ReportDelayed()
            {
                ReportResult(result);
                LevelManager.Instance.OnFinishRunning.RemoveListener(ReportDelayed);
            }
        }

        public virtual void EvaluateCubes(IReadOnlyList<Cube> cubes)
        {
        }
    }
}