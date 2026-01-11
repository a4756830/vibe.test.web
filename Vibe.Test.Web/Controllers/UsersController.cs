using Microsoft.AspNetCore.Mvc;
using Vibe.Test.Model.Entities;
using Vibe.Test.Servcie.Enums;
using Vibe.Test.Servcie.Interfaces;
using Vibe.Test.Servcie.ViewModel;

namespace Vibe.Test.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<APIResult>> Get()
    {
        var result = new Result<List<User>>();
        try
        {
            var users = await _userService.GetAllAsync();
            result.Data = users.ToList();
            result.Count = result.Data.Count;
            result.Success("取得成功");
            return Ok(result);
        }
        catch (Exception ex)
        {
            result.Fail($"取得使用者失敗: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<APIResult>> GetById(int id)
    {
        var result = new Result<User>();
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null)
            {
                result.Fail("資料不存在", ApiReturnCode.Not_Found);
                return NotFound(result);
            }

            result.Data = user;
            result.ID = user.Id;
            result.Success("取得成功");
            return Ok(result);
        }
        catch (Exception ex)
        {
            result.Fail($"取得使用者失敗: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }

    [HttpPost]
    public async Task<ActionResult<APIResult>> Create(User input)
    {
        var result = new Result<User>();
        try
        {
            var user = await _userService.CreateAsync(input);
            result.Data = user;
            result.ID = user.Id;
            result.Success("建立成功");
            result.Code = ApiReturnCode.Created;
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, result);
        }
        catch (Exception ex)
        {
            result.Fail($"建立使用者失敗: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<APIResult>> Update(int id, User input)
    {
        var result = new Result<User>();
        try
        {
            var user = await _userService.UpdateAsync(id, input);
            if (user is null)
            {
                result.Fail("資料不存在", ApiReturnCode.Not_Found);
                return NotFound(result);
            }

            result.Data = user;
            result.ID = user.Id;
            result.Success("更新成功");
            return Ok(result);
        }
        catch (Exception ex)
        {
            result.Fail($"更新使用者失敗: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<APIResult>> Delete(int id)
    {
        var result = new APIResult();
        try
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
            {
                result.Fail("資料不存在", ApiReturnCode.Not_Found);
                return NotFound(result);
            }

            result.Success("刪除成功");
            result.Code = ApiReturnCode.No_Content;
            return Ok(result);
        }
        catch (Exception ex)
        {
            result.Fail($"刪除使用者失敗: {ex.Message}", ApiReturnCode.General_Error);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}
