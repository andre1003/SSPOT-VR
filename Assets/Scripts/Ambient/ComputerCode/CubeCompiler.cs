using System.Collections.Generic;
using UnityEngine;

namespace SSpot.ComputerCode
{
    public class CubeCompiler : MonoBehaviour
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
        
        private CompilationResult CompileSlice(List<GameObject> codingCells, int start, int end)
        {
            List<Cube> result = new();

            for (int i = start; i < end; i++)
            {
                if (codingCells[i].transform.childCount == 0)
                    return Error(noHolesError, i);
                
                var baseCube = codingCells[i].transform.GetChild(0).GetComponent<CloningCube>().Cube;
                if (baseCube.type is Cube.CubeType.Begin or Cube.CubeType.End)
                    return Error(beginEndInMiddleError, i);
                
                var cube = new Cube(baseCube.type);
                result.Add(cube);
            }
            
            return new CompilationResult(result);
        }
        
        /// <summary>
        /// Compiles a list of coding cells into a sequence of cubes, checking for syntax and logical errors.
        /// </summary>
        /// <param name="codingCells">The list of GameObjects representing the coding cells.</param>
        /// <param name="loopControllers">The list of LoopControllers to determine loop slices in the code.</param>
        /// <returns>A CompilationResult containing the compiled list of cubes or an error if compilation fails.</returns>
        public CompilationResult Compile(List<GameObject> codingCells, List<LoopController> loopControllers)
        {
            int lastIndex = codingCells.FindLastIndex(cell => cell.transform.childCount > 0);
            if (lastIndex == -1)
                return Error(emptyError);
            
            if (mustUseAllSlots && lastIndex < codingCells.Count - 1)
                return Error(allSlotsError);
            
            if (!TryExtractBaseCube(codingCells[0], out var firstCube) || firstCube.type is not Cube.CubeType.Begin)
                return Error(beginError);

            if (!TryExtractBaseCube(codingCells[lastIndex], out var lastCube) || lastCube.type is not Cube.CubeType.End)
                return Error(endError);

            var result = new List<Cube> {new(Cube.CubeType.Begin)};
            foreach (var (start, end, count) in GetCodeSlices(loopControllers, 1, lastIndex))
            {
                var sliceResult = CompileSlice(codingCells, start, end);
                if (sliceResult.IsError)
                    return sliceResult;
                
                for (int i = 0; i < count; i++)
                {
                    result.AddRange(sliceResult.Result);
                }
            }
            result.Add(new(Cube.CubeType.End));
            
            return new CompilationResult(result);
        }


        /// <summary>
        /// Tries to extract a <see cref="CubeClass"/> from a <see cref="GameObject"/> coding cell.
        /// </summary>
        /// <param name="cell">The cell to extract the cube from.</param>
        /// <param name="baseCube">The extracted cube, or <see langword="null"/> if no cube was found.</param>
        /// <returns><see langword="true"/> if a cube was found, <see langword="false"/> otherwise.</returns>
        private static bool TryExtractBaseCube(GameObject cell, out CubeClass baseCube)
        {
            if (cell.transform.childCount == 0 || !cell.transform.GetChild(0).TryGetComponent(out CloningCube cloningCube))
            {
                baseCube = null;
                return false;
            }
            
            baseCube = cloningCube.Cube;
            return true;
        }

        /// <summary>
        /// Given a list of LoopControllers and a range of indices, this method yields a sequence of tuples,
        /// each containing the start index, end index, and iteration count of a given slice of code.
        /// </summary>
        private static IEnumerable<(int start, int end, int count)> GetCodeSlices(List<LoopController> loopControllers, 
            int firstIndex, int lastIndex)
        {
            int sliceStartIndex = firstIndex;
            for (int i = firstIndex; i < lastIndex; i++)
            {
                if (!loopControllers[i].gameObject.activeSelf) continue;

                // If there was non-looped code before the loop, return it as a single-iteration slice.
                if (i > sliceStartIndex)
                {
                    yield return (sliceStartIndex, i, 1);
                }
                
                // Return the loop
                var loop = loopControllers[i];
                yield return (i, i + loop.curRange, loop.iterations);

                i += loop.curRange;
                sliceStartIndex = i;
            }

            // If there is non-looped code after the last loop, return it as a single-iteration slice.
            if (sliceStartIndex < lastIndex)
            {
                yield return (sliceStartIndex, lastIndex, 1);
            }
        }
    }
}