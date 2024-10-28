using System.Collections.Generic;
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

        protected void None() => ReportResult(ObjectiveResult.None());
        
        protected void Success() => ReportResult(ObjectiveResult.Success());
        
        protected void Error(string message) => ReportResult(ObjectiveResult.Error(message));

        public virtual ObjectiveResult EvaluateCubes(IReadOnlyList<Cube> cubes)
        {
            return ObjectiveResult.Success();
        }
    }
}