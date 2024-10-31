using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SSpot.Ambient.ComputerCode;
using SSpot.Utilities;
using SSpot.Utilities.Attributes;

namespace SSpot.Objectives
{
    [Serializable]
    public struct ExpectedCube
    {
        public bool IsLoop => Type == Cube.CubeType.Loop;
        
        public Cube.CubeType Type;
        
        [ShowIf(nameof(IsLoop)), AllowNesting, MinValue(2)]
        public int Iterations;
        
        [ShowIf(nameof(IsLoop)), AllowNesting, Inline]
        public NestedList<Cube.CubeType> Loop;
        
        public IEnumerable<Cube.CubeType> GetCubeTypes()
        {
            if (!IsLoop)
            {
                yield return Type;
                yield break;
            }
                
            for (var i = 0; i < Iterations; i++)
                foreach (var expectedCube in Loop)
                    yield return expectedCube;
        }

        public bool IsEquivalentTo(List<CodingCell> cells, int i)
        {
            var cell = cells[i];
            if (cell.CurrentCube == null) return false;
                
            // If one is a loop but the other isn't, return false
            if (cell.HasLoop != IsLoop) return false;
                
            // If neither are loops, return true if they're the same type 
            if (!IsLoop) return cell.CurrentCube.type == Type;
                
            // Both are loops, compare data and content
            if (cell.LoopController.Iterations != Iterations) return false;
            if (cell.LoopController.Range != Loop.Count) return false;
                
            for (int j = 0; j < Loop.Count; j++)
                if (Loop[j] != cells[i + j].CurrentCube.type) 
                    return false;
                
            return true;
        }
    }
}