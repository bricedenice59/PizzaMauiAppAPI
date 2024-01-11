namespace PizzaMauiApp.API.Errors;

public class ApiException : ApiResponse
{
    public string Exception { get; set; }

    public ApiException(int statusCode, string exception): base(statusCode)
    {
        Exception = exception;
    }
    
    public ApiException(int statusCode, string? message, string exception) : base(statusCode,message)
    {
        Exception = exception;
    }
}