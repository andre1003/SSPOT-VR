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

        public int Index;
        
        public ResultType Type;

        private ObjectiveResult(ResultType type, string message = "", int index = -1) =>
            (Type, Message, Index) = (type, message, index);

        public static ObjectiveResult None() => new(ResultType.None);

        public static ObjectiveResult Success() => new(ResultType.Success);

        public static ObjectiveResult Error(string message, int index = -1) => new(ResultType.Error, message, index);
    }
}