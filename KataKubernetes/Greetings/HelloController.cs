/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */

using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace KataKubernetes.Greetings;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class HelloController(ILogger<HelloController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GreetingResponse>> Hello(
        [FromQuery] [Required(ErrorMessage = "Name parameter is required")] string name,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var greetingResponse = new GreetingResponse(
                $"Hi {name.Trim()}",
                "We would like to welcome you on planet Earth."
            );

            await Task.Delay(10);

            return Ok(greetingResponse);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Request cancelled by client");

            return StatusCode(
                499,
                new ProblemDetails
                {
                    Title = "Request Cancelled",
                    Detail = "The request was cancelled by the client",
                    Status = 499,
                    Instance = HttpContext.Request.Path,
                }
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Hello greeting caused errors");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while processing your request",
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = HttpContext.Request.Path,
                    Extensions = { { "traceId", HttpContext.TraceIdentifier } },
                }
            );
        }
    }
}
