using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Domain.Consts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using ActionResult = CoreGoDelivery.Domain.Response.BaseResponse.ActionResult;

namespace CoreGoDelivery.Api.Controllers.Base;

[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class BaseApiController : ControllerBase
{
    private readonly ILogger<BaseApiController> _logger;

    protected BaseApiController(ILogger<BaseApiController> logger)
    {
        _logger = logger;
    }

    protected new IActionResult Response(ActionResult response)
    {
        var data = response.GetData();

        if (response.HasError())
        {
            _logger.LogInformation(
                "API operation rejected by validation or a business rule. TraceId={TraceIdentifier}",
                HttpContext.TraceIdentifier);
            return StatusCode((int)HttpStatusCode.BadRequest, response.GetError());
        }
        else if (response.HasData())
        {
            return StatusCode((int)HttpStatusCode.OK, data);
        }

        return StatusCode((int)HttpStatusCode.NotFound);
    }

    protected IActionResult ResponseError(Exception exception)
    {
        _logger.LogError(
            exception,
            "API operation failed with {ExceptionType} TraceId={TraceIdentifier}",
            exception.GetType().Name,
            HttpContext.TraceIdentifier);

        var apiResponse = new ActionResult();
        apiResponse.SetError(CommomMessagesConst.MESSAGE_INTERNAL_ERROR);

        return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse.GetError());
    }

    public static string IdBuild(string? id)
    {
        var result = string.IsNullOrEmpty(id) ? Ulid.NewUlid().ToString() : id;

        return result;
    }

    public static string IdBuild()
    {
        return Ulid.NewUlid().ToString();
    }

    protected static string? IdParamValidator(string? id)
    {
        bool isNotValid = id == ":id" || string.IsNullOrEmpty(id);

        if (isNotValid)
        {
            return "id param".AppendError();
        }

        return null;
    }
}
