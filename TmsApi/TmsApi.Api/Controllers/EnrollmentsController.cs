using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.DTOs;
using TmsApi.Application.Enrollments.Commands;
using TmsApi.Application.Enrollments;
using TmsApi.Application.Interfaces;
using TmsApi.Application.Enrollments.Queries;

namespace TmsApi.Api.Controllers;

[ApiController]

// ===================== NEW =====================
// Exercise 2 introduces API Version 2.
// All new CQRS enrollment endpoints live under:
// POST /api/v2/enrollments
// GET  /api/v2/enrollments/{studentId}/schedule
// ===============================================
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/enrollments")]

[Tags("Enrollments")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class EnrollmentsController(
    IMediator mediator,
    ICourseService courseService,
    IEnrollmentService enrollmentService) : ControllerBase
{
    // ===================================================
    // Existing GET endpoints from Module 6
    // These can remain because the lab only replaces
    // the enrollment POST endpoint.
    // ===================================================

    [HttpGet("~/api/courses/{courseId:int}/enrollments", Name = "ListCourseEnrollments")]
    [ProducesResponseType(typeof(IReadOnlyList<EnrollmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("List enrollments for a course")]
    public async Task<IActionResult> GetEnrollments(int courseId, CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(courseId, ct);

        if (course == null)
            return NotFound();

        var enrollments = await enrollmentService.GetByCourseAsync(courseId, ct);

        return Ok(enrollments);
    }

    [HttpGet("~/api/courses/{courseId:int}/enrollments/{id:int}", Name = nameof(GetEnrollment))]
    [ProducesResponseType(typeof(EnrollmentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get one enrollment for a course")]
    public async Task<IActionResult> GetEnrollment(
        int courseId,
        int id,
        CancellationToken ct)
    {
        var enrollment = await enrollmentService.GetByIdAsync(courseId, id, ct);

        return enrollment is not null
            ? Ok(enrollment)
            : NotFound();
    }

    // ==========================================================
    // OLD MODULE 6 ENDPOINT
    // Commented out because Exercise 2 replaces it with
    // POST /api/v2/enrollments using CQRS + MediatR.
    // ==========================================================

    /*
    [HttpPost]
    public async Task<IActionResult> EnrollStudent(
        int courseId,
        EnrollStudentRequest request,
        CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(courseId, ct);

        if (course is null)
        {
            return NotFound();
        }

        if (course.Enrollments.Count >= course.MaxCapacity)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Course is full",
                Detail = $"Course '{course.Title}' has reached its maximum capacity of {course.MaxCapacity}.",
                Status = StatusCodes.Status409Conflict
            });
        }

        var createdEnrollment = await enrollmentService.CreateAsync(courseId, request, ct);

        return CreatedAtAction(
            nameof(GetEnrollment),
            new { courseId, id = createdEnrollment.Id },
            createdEnrollment);
    }
    */

    // ==========================================================
    // NEW CQRS WRITE ENDPOINT
    // Uses MediatR instead of calling services directly.
    // The controller simply sends the command to its handler.
    // ==========================================================

    [HttpPost]
    public async Task<IActionResult> Enroll(
        EnrollStudentCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.Match<IActionResult>(
            onSuccess: created => CreatedAtAction(
                nameof(GetSchedule),
                new { studentId = created.StudentId },
                created),

            onFailure: error =>
            {
                var status = error.Code switch
                {
                    "course_not_found" => StatusCodes.Status404NotFound,

                    "course_full" or "already_enrolled"
                        => StatusCodes.Status409Conflict,

                    _ => StatusCodes.Status400BadRequest
                };

                return Problem(
                    statusCode: status,
                    title: "Enrollment rejected",
                    detail: error.Message,
                    type: $"https://tms.local/errors/{error.Code}");
            });
    }

    // ==========================================================
    // NEW CQRS READ ENDPOINT
    // Uses MediatR to send a query instead of accessing
    // the database or services directly.
    // ==========================================================

    [HttpGet("{studentId}/schedule")]
    public async Task<IActionResult> GetSchedule(
        int studentId,
        CancellationToken ct)
    {
        var schedule = await mediator.Send(
            new GetStudentScheduleQuery(studentId),
            ct);

        return Ok(schedule);
    }
    [HttpGet]
public IActionResult GetAll()
{
    return Ok(new[]
    {
        new
        {
            id = "1",
            studentId = 1,
            studentName = "Liya",
            courseId = 1,
            courseName = "Advanced Java Services",
            status = "Pending",
            enrolledAt = DateTime.UtcNow
        },
        new
        {
            id = "2",
            studentId = 2,
            studentName = "Dawit",
            courseId = 2,
            courseName = "Angular UI Lab",
            status = "Approved",
            enrolledAt = DateTime.UtcNow
        }
    });
}
[HttpPost("{id}/approve")]
public IActionResult Approve(string id)
{
    return NoContent();
}
}