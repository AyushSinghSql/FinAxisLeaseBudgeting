using System;

namespace FinAxisLeaseBudgeting.Exceptions
{
    public class ToolExecutionException : Exception
    {
        public ToolExecutionException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}