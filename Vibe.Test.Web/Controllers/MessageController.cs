using Microsoft.AspNetCore.Mvc;
using Vibe.Test.Servcie.Enums;
using Vibe.Test.Servcie.Interfaces;
using Vibe.Test.Servcie.ViewModel;

namespace Vibe.Test.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// 發送訊息到 RabbitMQ
    /// </summary>
    [HttpPost("publish")]
    public async Task<ActionResult<APIResult>> PublishMessage([FromBody] MessageRequest request)
    {
        var result = new APIResult();
        try
        {
            var success = await _messageService.PublishMessageAsync(request.QueueName, request.Message);
            
            if (success)
            {
                result.Success("訊息發送成功");
                result.Data = $"Queue: {request.QueueName}";
            }
            else
            {
                result.Fail("訊息發送失敗", ApiReturnCode.General_Error);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            result.Fail($"訊息發送錯誤: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }

    /// <summary>
    /// 從 RabbitMQ 消費訊息
    /// </summary>
    [HttpGet("consume/{queueName}")]
    public async Task<ActionResult<APIResult>> ConsumeMessage(string queueName)
    {
        var result = new Result<StringData>();
        try
        {
            var message = await _messageService.ConsumeMessageAsync(queueName);
            
            if (message != null)
            {
                result.Data = new StringData { Value = message };
                result.Success("訊息接收成功");
            }
            else
            {
                result.Fail("佇列中沒有訊息", ApiReturnCode.Not_Found);
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            result.Fail($"訊息接收錯誤: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }

    /// <summary>
    /// 測試 RabbitMQ 連線
    /// </summary>
    [HttpGet("test")]
    public async Task<ActionResult<APIResult>> TestConnection()
    {
        var result = new APIResult();
        try
        {
            var testQueue = "test_queue";
            var testMessage = $"Test message at {DateTime.UtcNow}";
            
            // 發送測試訊息
            var published = await _messageService.PublishMessageAsync(testQueue, testMessage);
            
            if (!published)
            {
                result.Fail("RabbitMQ 連線失敗", ApiReturnCode.General_Error);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            // 接收測試訊息
            var consumed = await _messageService.ConsumeMessageAsync(testQueue);
            
            if (consumed == testMessage)
            {
                result.Success("RabbitMQ 連線測試成功");
                result.Data = $"發送並接收訊息: {testMessage}";
            }
            else
            {
                result.Fail("RabbitMQ 測試失敗", ApiReturnCode.General_Error);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            result.Fail($"RabbitMQ 測試錯誤: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}

public class MessageRequest
{
    public string QueueName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
