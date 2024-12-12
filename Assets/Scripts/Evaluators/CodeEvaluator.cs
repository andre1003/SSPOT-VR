using System.Collections.Generic;
using SSpot.Ambient.ComputerCode;
using SSpot.Level;
using SSPot.Level;
using UnityEngine;

namespace SSpot.Evaluators
{
    public abstract class CodeEvaluator : MonoBehaviour
    {
        public abstract void EvaluatePreCompilation(IReadOnlyList<CodingCell> cubes);

        public abstract void EvaluatePostCompilation(IReadOnlyList<Cube> cubes);
        
        protected static void ReportResult(LevelResult result) => LevelManager.Instance.ReportResult(result);

        protected static void ReportResultOnRunningEnd(LevelResult result)
        {
            LevelManager.Instance.OnFinishRunning.AddListener(ReportDelayed);
            
            return;
            
            void ReportDelayed()
            {
                ReportResult(result);
                LevelManager.Instance.OnFinishRunning.RemoveListener(ReportDelayed);
            }
        }
    }
}