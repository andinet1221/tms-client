using Microsoft.AspNetCore.Authentication;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Services
        builder.Services.AddControllers();

        builder.Services
            .AddAuthentication("Training")
            .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
                "Training", null);

        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Exercise 1B
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseExceptionHandler("/error");

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.MapGet("/api/assessments/results", () =>
        {
            return Results.Ok(new
            {
                courseCode = "CS-101",
                studentId = "S-001",
                letterGrade = "A"
            });
        })
        .RequireAuthorization();

        app.Run();
    }
}