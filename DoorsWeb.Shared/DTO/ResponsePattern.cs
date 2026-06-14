// class for handling internal service errors
// Based on this
// https://www.youtube.com/watch?v=5emVIkthkDg
namespace DoorsWeb.Shared.DTO
{
    public class Result<T>
    {
        private Result(T value)
        {
            Value = value;
            Error = null;
        }
        private Result(Error value)
        {
            Value = default(T);
            Error = value;
        }

        public T? Value { get; }
        public Error? Error { get; }
        public bool IsSuccess => Error == null;
        public static Result<T> Success(T value) => new Result<T>(value);
        public static Result<T> Failure(Error error) => new Result<T>(error);

        // when using Result.Map force you to handle both success and failure cases!
        public TResult Map<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
        {
            return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
        }
    }

    public enum ErrorType
    {
        NullError,
        NotFound,
        Validation,
        Unauthorized,
        NameAlreadyExists,
    }

    public class Error
    {
        public Error(string message, ErrorType type)
        {
            Message = message;
            Type = type;
        }
        public string Message { get; }
        public ErrorType Type { get; }
        // Maps ErrorType to the correct HTTP status code
        public int StatusCode => Type switch
        {
            ErrorType.NullError => 400,
            ErrorType.NotFound => 404,
            ErrorType.Unauthorized => 401,
            _ => 400
        };
    }

    public class ClientProblemDetails
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Detail { get; set; }
        public string? Instance { get; set; }
        public string? TraceId { get; set; }
        public string? RequestId { get; set; }
    }
}
