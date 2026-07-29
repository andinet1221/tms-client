using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/v2/transcripts")]
public class TranscriptsController : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("transcripts")]
    public IActionResult RequestTranscript([FromBody] object? request)
    {
        // Stub implementation.
        // Exercise 5 will replace this with the real background processing.

        return Ok(new
        {
            Message = "Transcript request received."
        });
    }
}