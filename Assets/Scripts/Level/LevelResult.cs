namespace SSPot.Level
{
    public struct LevelResult
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

        private LevelResult(ResultType type, string message = "", int index = -1) =>
            (Type, Message, Index) = (type, message, index);

        public static LevelResult None() => new(ResultType.None);

        public static LevelResult Success() => new(ResultType.Success);

        public static LevelResult Error(string message, int index = -1) => new(ResultType.Error, message, index);
    }
}