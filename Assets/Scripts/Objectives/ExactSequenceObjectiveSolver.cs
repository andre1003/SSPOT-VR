using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SSpot.Objectives
{
    public class ExactSequenceObjectiveSolver : LevelObjectiveSolver
    {
        [SerializeField] private string errorMessage = "Deu ERRO!\nCódigo incorreto! Tente novamente!";
        [SerializeField] private CubeClass[] sequence;

        public override ObjectiveResult EvaluateCubes(IReadOnlyList<Cube> cubes)
        {
            var expected = sequence.SelectMany(c => c.GetCommandList()).Select(c => c.type);
            var actual = cubes.Select(c => c.type);
            return expected.SequenceEqual(actual)
                ? ObjectiveResult.Success()
                : ObjectiveResult.Error(errorMessage);
        }
            
    }
}