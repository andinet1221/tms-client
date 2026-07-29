using System.ComponentModel.DataAnnotations;

namespace TmsApi.Application.DTOs;

public class EnrollStudentRequest
{
    [Required]
    public int StudentId { get; set; }
}
