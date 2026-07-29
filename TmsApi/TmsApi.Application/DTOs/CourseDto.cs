namespace TmsApi.Application.DTOs;

public class CourseDto
{
    public int Id { get; set; }

    public required string Code { get; set; }

    public required string Title { get; set; }

    public int MaxCapacity { get; set; }
}
