using Application.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Dynamic;

namespace API.Controllers;

[EnableRateLimiting("fixed")]
[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (!result.IsSuccess)
        {
            if (result.Code == 404)
                return NotFound(new { message = result.Error, code = result.Code });

            return BadRequest(new { message = result.Error, code = result.Code });
        }

        var type = typeof(T);

        if (type == typeof(string) || type == typeof(Guid) || type == typeof(int))
        {
            var actionName = ControllerContext.ActionDescriptor.ActionName?.ToLower();

            var key = actionName switch
            {
                "login" => "token",
                "register" => "token",
                _ => "id"
            };

            dynamic obj = new ExpandoObject();
            ((IDictionary<string, object>)obj)[key] = result.Value!;
            obj.message = result.Message;

            return Ok(obj);
        }

        if (result.Message == null)
        {
            return Ok(result.Value);
        }

        return Ok(new
        {
            data = result.Value,
            message = result.Message
        });
    }
}
