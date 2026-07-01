using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    [Route("error")]
    public IActionResult HandleError()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        return Problem(
            title: "An unexpected error occurred.",
            detail: exceptionFeature?.Error.Message,
            statusCode: StatusCodes.Status500InternalServerError);
    }
}