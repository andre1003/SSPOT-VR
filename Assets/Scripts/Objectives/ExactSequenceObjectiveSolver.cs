using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SSpot.Objectives
{
    public class ExactSequenceObjectiveSolver : LevelObjectiveSolver
    {
        [SerializeField] private bool waitForEndOfSequence;
        [SerializeField] private string errorMessage = "Deu ERRO!\nCódigo incorreto! Tente novamente!";
        [SerializeField] private CubeClass[] sequence;

        public override void EvaluateCubes(IReadOnlyList<Cube> cubes)
        {
            var expected = sequence.SelectMany(c => c.GetCommandList()).Select(c => c.type);
            var actual = cubes.Select(c => c.type);

            var result = expected.SequenceEqual(actual)
                ? ObjectiveResult.Success()
                : ObjectiveResult.Error(errorMessage);

            if (waitForEndOfSequence)
                ReportResultOnRunningEnd(result);
            else
                ReportResult(result);
        }

    }
}