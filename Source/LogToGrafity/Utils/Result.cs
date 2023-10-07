namespace LogToGrafity
{
    public interface IResult
    {
        bool IsSuccess { get; }
        bool IsFailure { get; }

        /// <summary>
        /// Only set when <see cref="IsFailure"/>.
        /// </summary>
        string Message { get; }
    }

    public struct Result : IResult
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public string Message { get; private set; }

        public static Result Ok()
        {
            return new Result { IsSuccess = true };
        }

        public static Result<T> Ok<T>(T value)
        {
            return Result<T>.Ok(value);
        }

        public static Result Fail(string message)
        {
            return new Result { IsSuccess = false, Message = message };
        }

        public static Result<T> Fail<T>(string message)
        {
            return Result<T>.Fail(message);
        }
    }

    public struct Result<T> : IResult
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public string Message { get; private set; }

        /// <summary>
        /// Only set when <see cref="IsSuccess"/>.
        /// </summary>
        public T Value { get; private set; }

        public static Result<T> Ok(T value)
        {
            return new Result<T> { IsSuccess = true, Value = value };
        }

        public static Result<T> Fail(string message)
        {
            return new Result<T> { IsSuccess = false, Message = message };
        }
    }
}
