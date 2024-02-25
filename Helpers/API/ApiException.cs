namespace PizzaMauiApp.API.Helpers.API;

public class ApiException<T> : ApiResponse<T>
{
    public string? ExceptionMessage { get; set; }
    public string? StackTrace { get; set; }

    public ApiException(int statusCode, string? exception = null, string? stacktrace = null): base(statusCode, false)
    {
        ExceptionMessage = exception ?? GetDefaultMessage(statusCode);
        StackTrace = stacktrace;
    }
    
    private string? GetDefaultMessage(int statusCode)
    {
        return statusCode switch
        {
            400 => "You have made a bad request",
            401 => "Authorized error",
            404 => "Resource has not been found",
            500 => "Internal Server Error...",
            _ => null
        };
    }
}