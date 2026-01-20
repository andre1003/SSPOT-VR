using System;
using System.Collections.Generic;
using SSPot.Utilities;
using UnityEngine;

namespace SSpot.Ambient.ComputerCode
{
    [Serializable]
    public class CubeCompiler
    {
        [SerializeField] private bool mustUseAllSlots = true;
        
        [Header("Errors")]
        [SerializeField] private string emptyError = "Preencha o algoritmo!";
        [SerializeField] private string beginError = "Deu ERRO! Verifique se o algoritmo foi iniciado corretamente!";
        [SerializeField] private string endError = "Deu ERRO! Verifique se o algoritmo foi finalizado corretamente!";
        [SerializeField] private string allSlotsError = "Deu ERRO! Você deve preencher todas as placas de programação!"; 
        [SerializeField] private string noHolesError = "Deu ERRO! Seu algoritmo não pode ter placas vazias!";
        [SerializeField] private string beginEndInMiddleError = "Deu ERRO! Início e Fim devem ser usados no lugar certo!";
        
        private static CompilationResult Error(string error, int index) => new(error, index);
        
        private static CompilationResult Error(string error) => new(error, -1);
        
        /// <summary>
        /// Compiles a list of coding cells into a sequence of cubes, checking for syntax and logical errors.
        /// </summary>
        /// <returns>A CompilationResult containing the compiled list of cubes or an error if compilation fails.</returns>
        public CompilationResult Compile(IReadOnlyList<CodingCell> cells)
        {
            int lastIndex = cells.FindLastIndex(cell => cell.CurrentCube != null);
            if (lastIndex == -1)
                return Error(emptyError);
            
            if (mustUseAllSlots && lastIndex < cells.Count - 1)
                return Error(allSlotsError);
            
            if (cells[0].CurrentCube is not {type: Cube.CubeType.Begin})
                return Error(beginError);

            if (cells[lastIndex].CurrentCube is not {type: Cube.CubeType.End})
                return Error(endError);

            var result = new List<Cube> {new(Cube.CubeType.Begin)};
            foreach (var (start, end, count) in GetCodeSlices(cells, 1, lastIndex))
            {
                var sliceResult = CompileSlice(cells, start, end);
                if (sliceResult.IsError)
                    return sliceResult;
                
                //Consider other methods of compiling. For example, assembly style go-tos, or cube metadata.
                //A cube could contain an int iterCount and int loopLen to represent a for loop.
                for (int i = 0; i < count; i++)
                {
                    result.AddRange(sliceResult.Result);
                }
            }
            result.Add(new(Cube.CubeType.End));
            
            return new CompilationResult(result);
        }
        
        /// <summary>
        /// Compiles a range of coding cells into a list of cubes. Validates the coding cells
        /// for correct placement and type usage, returning errors if any conditions are not met.
        /// </summary>
        /// <param name="codingCells">The list of coding cells to compile.</param>
        /// <param name="start">The starting index of the range to compile (inclusive).</param>
        /// <param name="end">The ending index of the range to compile (exclusive).</param>
        /// <returns>A <see cref="CompilationResult"/> containing the compiled cubes or an error if compilation fails.</returns>
        private CompilationResult CompileSlice(IReadOnlyList<CodingCell> codingCells, int start, int end)
        {
            List<Cube> result = new();

            for (int i = start; i < end; i++)
            {
                var baseCube = codingCells[i].CurrentCube;
                if (baseCube == null)
                    return Error(noHolesError, i);
                
                if (baseCube.type is Cube.CubeType.Begin or Cube.CubeType.End)
                    return Error(beginEndInMiddleError, i);
                
                var cube = new Cube(baseCube.type);
                result.Add(cube);
            }
            
            return new CompilationResult(result);
        }

        /// <summary>
        /// Given a list of LoopControllers and a range of indices, this method yields a sequence of tuples,
        /// each containing the start index, end index, and iteration count of a given slice of code.
        /// </summary>
        /// <param name="cells">The list of cells to determine loop slices in the code.</param>
        /// <param name="firstIndex">The starting index of the range to slice (inclusive).</param>
        /// <param name="lastIndex">The ending index of the range to slice (exclusive).</param>
        private static IEnumerable<(int start, int end, int count)> GetCodeSlices(
            IReadOnlyList<CodingCell> cells, int firstIndex, int lastIndex)
        {
            int sliceStartIndex = firstIndex;
            for (int i = firstIndex; i < lastIndex; i++)
            {
                if (i >= cells.Count || !cells[i].HasLoop) 
                    continue;

                // If there was non-looped code before the loop, return it as a single-iteration slice.
                if (i > sliceStartIndex)
                {
                    yield return (sliceStartIndex, i, 1);
                }

                // Return the loop
                var loop = cells[i].LoopController;
                yield return (i, i + loop.Range, loop.Iterations);

                i += loop.Range - 1;
                sliceStartIndex = i + 1;
            }

            // If there is non-looped code after the last loop, return it as a single-iteration slice.
            if (sliceStartIndex < lastIndex)
            {
                yield return (sliceStartIndex, lastIndex, 1);
            }
        }
    }
}