using Microsoft.AspNetCore.Mvc;
using TmsApi.Dtos;
using TmsApi.Entities;
using TmsApi.Services;

using Microsoft.AspNetCore.Routing;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/courses")]
[Tags("Courses")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class CoursesController(ICourseService courseService,LinkGenerator linkGenerator) : ControllerBase
{

    //------Exercise 5: GetCourseById with HATEOAS links------
[HttpGet("{id:int}", Name = nameof(GetCourseById))]
[ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[EndpointSummary("Get a course by ID")]
[EndpointDescription("Returns course details with HATEOAS links. Returns 404 if the course does not exist.")]
public async Task<IActionResult> GetCourseById(int id, CancellationToken ct)
{
    var course = await courseService.GetByIdAsync(id, ct);

    if (course == null)
    {
        return NotFound();
    }

    // Build the list of HATEOAS links
    var links = new List<LinkDto>();

    var selfLink = linkGenerator.GetPathByName(
        HttpContext,
        nameof(GetCourseById),
        new { id = course.Id });

    var enrollmentsLink = "/api/courses/" + course.Id + "/enrollments";

    links.Add(new LinkDto(selfLink!, "self", "GET"));
    links.Add(new LinkDto(selfLink!, "update", "PUT"));
    links.Add(new LinkDto(selfLink!, "delete", "DELETE"));
    links.Add(new LinkDto(enrollmentsLink, "enrollments", "GET"));

    // Only show the enroll link if the course is not full
    if (course.Enrollments.Count < course.MaxCapacity)
    {
        links.Add(new LinkDto(enrollmentsLink, "enroll", "POST"));
    }

    var detailDto = new CourseDetailDto
    {
        Id = course.Id,
        Code = course.Code,
        Title = course.Title,
        MaxCapacity = course.MaxCapacity,
        EnrollmentCount = course.Enrollments.Count,
        Links = links
    };

    return Ok(detailDto);
}
//---------------EXERCISE 6-------------
    [HttpGet]
[ProducesResponseType(typeof(PagedResponse<CourseResponseDto>), StatusCodes.Status200OK)]
[EndpointSummary("List courses with pagination")]
[EndpointDescription("Returns a paginated, optionally filtered list of TMS courses. PageSize is capped at 50.")]
public async Task<IActionResult> GetCourses(
    [FromQuery] PagedRequest request,
    CancellationToken ct)
{
    var result = await courseService.GetCoursesAsync(request, ct);

    return Ok(result);
}

    [HttpPost]
[ProducesResponseType(typeof(CourseResponseDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[EndpointSummary("Create a new course")]
[EndpointDescription("Creates a course with a unique code. Returns 409 if the course code already exists.")]
    public async Task<IActionResult> CreateCourse(CreateCourseDto dto, CancellationToken ct)
    {
        // ==============================
        // NEW CODE (Exercise 3)
        // ==============================
        if (await courseService.CodeExistsAsync(dto.Code, ct))
        {
            return Conflict(new ProblemDetails
            {
                Title = "Course code already exists",
                Detail = $"A course with code '{dto.Code}' is already registered.",
                Status = StatusCodes.Status409Conflict
            });
        }
        // ===========================

        var course = new Course
        {
            Code = dto.Code,
            Title = dto.Title,
            MaxCapacity = dto.MaxCapacity
        };

        var result = await courseService.CreateAsync(course, ct);

        var courseDto = new CourseDto
        {
            Id = result.Id,
            Code = result.Code,
            Title = result.Title,
            MaxCapacity = result.MaxCapacity
        };

        return CreatedAtAction(
            nameof(GetCourseById),
            new { id = result.Id },
            courseDto);
    }
}