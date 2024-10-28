namespace SSpot.Objectives
{
    public struct ObjectiveResult
    {
        public enum ResultType
        {
            None,
            Success,
            Error
        };
       
        public string Message;
        
        public ResultType Type;
        
        public static ObjectiveResult None() => new() {Type = ResultType.None, Message = null};
        
        public static ObjectiveResult Success() => new() {Type = ResultType.Success, Message = null};

        public static ObjectiveResult Error(string message) => new() {Type = ResultType.Error, Message = message};
    }
}