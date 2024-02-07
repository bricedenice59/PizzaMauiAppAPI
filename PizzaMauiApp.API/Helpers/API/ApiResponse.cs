namespace PizzaMauiApp.API.Helpers.API;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    
    public string? Message { get; set; }
    public T? Data { get; set; }

    public ApiResponse()
    {
        
    }
    
    public ApiResponse(int statusCode, bool success, string? message = null)
    {
        StatusCode = statusCode;
        Success = success;
        Message = message;
    }
    
    public ApiResponse(int statusCode, string? message = null)
    {
        StatusCode = statusCode;
        Success = false;
        Message = message;
    }
    
    public ApiResponse(int statusCode, T data)
    {
        Success = true;
        StatusCode = statusCode;
        Data = data;
    }
}