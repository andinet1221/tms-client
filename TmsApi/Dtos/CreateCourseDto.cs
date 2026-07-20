using System.ComponentModel.DataAnnotations;

namespace TmsApi.Dtos;

public class CreateCourseDto
{
    [Required]
    [StringLength(10)]
    public required string Code { get; set; }

    [Required]
    [StringLength(200)]
    public required string Title { get; set; }

    [Range(1, 1000)]
    public int MaxCapacity { get; set; }
}