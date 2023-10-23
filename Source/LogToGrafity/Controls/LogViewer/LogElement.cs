namespace LogToGrafity
{
    public sealed class LogElement
    {
        public LogElement(LogLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        public LogLevel Level { get; }
        public string Message { get; }
    }
}
