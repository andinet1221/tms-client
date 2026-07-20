using System.ComponentModel.DataAnnotations;

namespace TmsApi.Dtos;

public class EnrollStudentRequest
{
    [Required]
    public int StudentId { get; set; }
}