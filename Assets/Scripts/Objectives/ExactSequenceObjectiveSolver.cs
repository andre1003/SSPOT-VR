using System.Collections.Generic;
using System.Linq;
using SSpot.Ambient.ComputerCode;
using UnityEngine;

namespace SSpot.Objectives
{
    public class ExactSequenceObjectiveSolver : LevelObjectiveSolver
    {
        [Tooltip("If true, the solver will only report an error or success when the entire code is executed.")]
        [SerializeField] private bool waitForEndOfSequence;
        
        [Tooltip("If true, the solver will evaluate the compiled code. If false, the solver will evaluate the raw code.\n" +
                 "If evaluating compiled code, unrolled loops will be equivalent to normal loops.")]
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

        public override void EvaluatePreCompilation(IReadOnlyList<CodingCell> cells)
        {
            if (evaluateCompiledCode) return;

            // Begin and end are optional
            List<CodingCell> actual = new(cells);
            actual.RemoveAt(0);
            actual.RemoveAt(actual.Count - 1);
            
            Report(AreCellsEqualToCubes(sequence, actual));
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

        private static bool AreCellsEqualToCubes(List<ExpectedCube> expected, List<CodingCell> actual)
        {
            // We know expected.Count <= actual.Count because loops are collapsed into a single entry in expected
            for (int actualIndex = 0, expectedIndex = 0; actualIndex < actual.Count; actualIndex++, expectedIndex++)
            {
                if (!expected[expectedIndex].IsEquivalentTo(actual, actualIndex)) 
                    return false;
                
                if (expected[expectedIndex].IsLoop)
                    actualIndex += expected[expectedIndex].Loop.Count - 1;
            }

            return true;
        }
    }
}