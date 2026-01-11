using Microsoft.AspNetCore.Mvc;
using Vibe.Test.Servcie.Enums;
using Vibe.Test.Servcie.Interfaces;
using Vibe.Test.Servcie.ViewModel;

namespace Vibe.Test.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CacheController : ControllerBase
{
    private readonly ICacheService _cacheService;

    public CacheController(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    /// <summary>
    /// 設定快取
    /// </summary>
    [HttpPost("set")]
    public async Task<ActionResult<APIResult>> SetCache([FromBody] CacheRequest request)
    {
        var result = new APIResult();
        try
        {
            var expiry = request.ExpirySeconds.HasValue 
                ? TimeSpan.FromSeconds(request.ExpirySeconds.Value) 
                : (TimeSpan?)null;

            var success = await _cacheService.SetAsync(request.Key, request.Value, expiry);
            
            if (success)
            {
                result.Success("快取設定成功");
                result.Data = $"Key: {request.Key}";
            }
            else
            {
                result.Fail("快取設定失敗", ApiReturnCode.General_Error);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            result.Fail($"快取設定錯誤: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }

    /// <summary>
    /// 取得快取
    /// </summary>
    [HttpGet("{key}")]
    public async Task<ActionResult<APIResult>> GetCache(string key)
    {
        var result = new Result<StringData>();
        try
        {
            var value = await _cacheService.GetAsync(key);
            
            if (value != null)
            {
                result.Data = new StringData { Value = value };
                result.Success("取得快取成功");
            }
            else
            {
                result.Fail("快取不存在", ApiReturnCode.Not_Found);
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            result.Fail($"取得快取錯誤: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }

    /// <summary>
    /// 刪除快取
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult<APIResult>> DeleteCache(string key)
    {
        var result = new APIResult();
        try
        {
            var success = await _cacheService.DeleteAsync(key);
            
            if (success)
            {
                result.Success("快取刪除成功");
                result.Code = ApiReturnCode.No_Content;
            }
            else
            {
                result.Fail("快取不存在", ApiReturnCode.Not_Found);
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            result.Fail($"刪除快取錯誤: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }

    /// <summary>
    /// 檢查快取是否存在
    /// </summary>
    [HttpGet("exists/{key}")]
    public async Task<ActionResult<APIResult>> ExistsCache(string key)
    {
        var result = new Result<BoolData>();
        try
        {
            var exists = await _cacheService.ExistsAsync(key);
            result.Data = new BoolData { Value = exists };
            result.Success(exists ? "快取存在" : "快取不存在");
            return Ok(result);
        }
        catch (Exception ex)
        {
            result.Fail($"檢查快取錯誤: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}

public class CacheRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int? ExpirySeconds { get; set; }
}
