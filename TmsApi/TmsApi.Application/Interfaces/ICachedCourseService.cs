using TmsApi.Application.DTOs;

namespace TmsApi.Application.Interfaces;

/// <summary>
/// Provides cached read access for course data.
/// Uses Hybrid Cache to avoid repeated database queries.
/// </summary>
public interface ICachedCourseService
{
    /// <summary>
    /// Gets a course by its course code.
    /// Returns null if the course does not exist.
    /// </summary>
    Task<CourseDto?> GetCourseAsync(
        string courseCode,
        CancellationToken ct);

    /// <summary>
    /// Removes cached course data after a course is created or updated.
    /// </summary>
    Task InvalidateCourseAsync(
        string courseCode,
        CancellationToken ct);


        Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(
    PagedRequest request,
    CancellationToken ct);
}