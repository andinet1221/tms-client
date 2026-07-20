namespace TmsApi.Dtos;

public class EnrollmentResponseDto
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int StudentId { get; set; }

    public DateTime EnrolledAt { get; set; }
}