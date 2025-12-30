namespace HRM.ServiceLayer.DTOs.Common;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ServiceResult<T> SuccessResult(T data, string message = "Operation successful")
    {
        return new ServiceResult<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ServiceResult<T> FailureResult(string message, List<string>? errors = null)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static ServiceResult SuccessResult(string message = "Operation successful")
    {
        return new ServiceResult
        {
            Success = true,
            Message = message
        };
    }

    public static ServiceResult FailureResult(string message, List<string>? errors = null)
    {
        return new ServiceResult
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}
