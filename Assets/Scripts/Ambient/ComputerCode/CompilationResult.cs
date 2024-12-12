using System.Collections.Generic;

namespace SSpot.Ambient.ComputerCode
{
    public readonly struct CompilationResult
    {
        public readonly string Error;
        public readonly int ErrorIndex;
        
        public bool IsError => Error != null;

        public readonly List<Cube> Result;

        public CompilationResult(List<Cube> result)
        {
            Error = null;
            ErrorIndex = -1;
            Result = result;
        }
        
        public CompilationResult(string error, int errorIndex)
        {
            Error = error;
            ErrorIndex = errorIndex;
            Result = null;
        }
    }
}