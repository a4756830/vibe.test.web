using System.Net;
using System.Text.Json;
using Vibe.Test.Servcie.Enums;
using Vibe.Test.Servcie.ViewModel;

namespace Vibe.Test.Web.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發生未處理的異常: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var result = new APIResult
        {
            IsSuccess = false,
            Code = ApiReturnCode.General_Error,
            Message = $"伺服器錯誤: {exception.Message}"
        };

        // 開發環境可以顯示詳細錯誤
        if (context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true)
        {
            result.Errors.Add(new ErrorView
            {
                ID = "Exception",
                DataType = exception.GetType().Name,
                ErrorObj = new Dictionary<string, string>
                {
                    { "Message", exception.Message },
                    { "StackTrace", exception.StackTrace ?? string.Empty }
                }
            });
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var jsonResult = JsonSerializer.Serialize(result, jsonOptions);
        await context.Response.WriteAsync(jsonResult);
    }
}
