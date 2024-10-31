using System.Collections.Generic;
using System.Linq;
using SSpot.Ambient.ComputerCode;
using UnityEngine;

namespace SSpot.Objectives
{
    public class ExactSequenceObjectiveSolver : LevelObjectiveSolver
    {
        [SerializeField] private bool waitForEndOfSequence;
        [SerializeField] private bool evaluateCompiledCode;
        [SerializeField, Multiline] private string errorMessage = "Deu ERRO!\nCódigo incorreto! Tente novamente!";
        [SerializeField] private List<ExpectedCube> sequence = new();
        

        private void Awake()
        {
            // Begin and end are optional in the inspector
            if (sequence[0].Type == Cube.CubeType.Begin) sequence.RemoveAt(0);
            if (sequence[^1].Type == Cube.CubeType.End) sequence.RemoveAt(sequence.Count - 1);
        }


        private void Report(bool isCorrect)
        {
            var result = isCorrect
                ? ObjectiveResult.Success()
                : ObjectiveResult.Error(errorMessage);
            
            if (waitForEndOfSequence)
                ReportResultOnRunningEnd(result);
            else
                ReportResult(result);
        }

        public override void EvaluatePreCompilation(IReadOnlyList<CodingCell> cubes)
        {
            if (evaluateCompiledCode) return;

            var expected = sequence;
            var actual = cubes
                .SkipWhile(c => c.CurrentCube.type == Cube.CubeType.Begin)
                .TakeWhile(c => c.CurrentCube.type != Cube.CubeType.End)
                //.Where(c => c.CurrentCube is {type: not Cube.CubeType.Begin and not Cube.CubeType.End})
                .ToList();
            
            Report(AreCellsEqualToCubes(actual, expected));
        }

        public override void EvaluatePostCompilation(IReadOnlyList<Cube> cubes)
        {
            if (!evaluateCompiledCode) return;

            var expected = sequence
                .SelectMany(c => c.GetCubeTypes());
            
            // Begin and end are optional
            var actual = cubes
                .Select(c => c.type)
                .Skip(1).SkipLast(1);

            Report(expected.SequenceEqual(actual));
        }

        private static bool AreCellsEqualToCubes(List<CodingCell> cells, List<ExpectedCube> cubes)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (!cubes[i].IsEquivalentTo(cells, i)) 
                    return false;
                
                if (cubes[i].IsLoop)
                    i += cubes[i].Loop.Count - 1;
            }

            return true;
        }
    }
}