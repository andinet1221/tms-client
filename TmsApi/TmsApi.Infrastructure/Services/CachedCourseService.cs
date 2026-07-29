using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using TmsApi.Application.DTOs;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Caching;

namespace TmsApi.Infrastructure.Services;

public class CachedCourseService(
    ICourseService courseService,
    HybridCache cache,
    ILogger<CachedCourseService> logger) : ICachedCourseService
{

    public async Task<CourseDto?> GetCourseAsync(
        string courseCode,
        CancellationToken ct)
    {
        var cacheKey = $"course:{courseCode}";

        return await cache.GetOrCreateAsync(
            cacheKey,
            async cancel =>
            {
                logger.LogInformation(
                    "Cache miss for course {Code}. Loading from database.",
                    courseCode);

                var course = await courseService
                    .GetByCodeAsync(courseCode, cancel);

                if (course is null)
                {
                    return null;
                }

                return MapToDto(course);

            },
            cancellationToken: ct);
    }


    public async Task InvalidateCourseAsync(
        string courseCode,
        CancellationToken ct)
    {
        var cacheKey = $"course:{courseCode}";

        await cache.RemoveAsync(
            cacheKey,
            ct);

        logger.LogInformation(
            "Cache invalidated for course {Code}.",
            courseCode);
    }


    private static CourseDto MapToDto(Course course)
    {
        return new CourseDto
        {
            Id = course.Id,
            Code = course.Code,
            Title = course.Title,
            MaxCapacity = course.MaxCapacity
        };
    }

    public async Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(
    PagedRequest request,
    CancellationToken ct)
{
    var key =
        $"v1:courses:{request.Page}:{request.PageSize}:{request.Search}:{request.OrderBy}:{request.Descending}";

    var result = await cache.GetOrCreateAsync(
        key,
        request,
        async (state, token) =>
        {
            logger.LogInformation(
                "Cache MISS for {Key}. Fetching courses from database.",
                key);

            return await courseService.GetCoursesAsync(
                state,
                token);
        },
        tags: [CacheKeys.CoursesTag],
        cancellationToken: ct);


    logger.LogInformation(
        "Cache HIT for {Key}",
        key);


    return result;
}
}