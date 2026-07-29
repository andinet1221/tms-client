using Microsoft.AspNetCore.Mvc;
using TmsApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestController(TmsDbContext context, IServiceScopeFactory scopeFactory) : ControllerBase
{
    [HttpGet("deferred")]
    public IActionResult TestDeferred()
    {
        Console.WriteLine("\n>>> STEP 1: Building the query object (no database contact)...");
        var query = context.Students.Where(s => s.GPA >= 3.0m);

        Console.WriteLine(">>> STEP 2: Appending a sorting clause...");
        var orderedQuery = query.OrderBy(s => s.Name);

        Console.WriteLine(">>> STEP 3: Materializing query into a C# List...");
        var results = orderedQuery.ToList();

        Console.WriteLine(">>> STEP 4: Materialization finished. List populated.\n");
        return Ok(results);
    }

    private static bool IsHonorRoll(decimal gpa)
    {
        return gpa >= 3.5m;
    }

    [HttpGet("translation-fail")]
    public IActionResult TestTranslationFail()
    {
        Console.WriteLine("\n>>> STEP 1: Running non-translatable query...");
        try
        {
            var students = context.Students
                .Where(s => IsHonorRoll(s.GPA))
                .ToList();
            return Ok(students);
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> EXCEPTION CAUGHT: {ex.Message}\n");
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("n-plus-one")]
public async Task<IActionResult> TestNPlusOne(CancellationToken cancellationToken)
{
    Console.WriteLine("\n>>> N+1 EXPERIMENT: one query for students, then one query PER student...\n");

    var students = await context.Students.AsNoTracking().ToListAsync(cancellationToken);

    var results = new List<string>();
    foreach (var s in students)
    {
        var count = await context.Enrollments
            .AsNoTracking()
            .CountAsync(e => e.StudentId == s.Id, cancellationToken);

        Console.WriteLine($"{s.Name}: {count} enrollments");
        results.Add($"{s.Name}: {count} enrollments");
    }

    return Ok(results);
}

[HttpGet("shaped-query")]
public async Task<IActionResult> TestShapedQuery(CancellationToken cancellationToken)
{
    Console.WriteLine("\n>>> SHAPED QUERY: one single SQL statement for everything...\n");

    var report = await context.Students
        .AsNoTracking()
        .Select(s => new
        {
            s.Name,
            EnrollmentCount = s.Enrollments.Count
        })
        .ToListAsync(cancellationToken);

    var results = report.Select(r => $"{r.Name}: {r.EnrollmentCount} enrollments").ToList();
    foreach (var line in results)
        Console.WriteLine(line);

    return Ok(results);
}
[HttpGet("concurrency-demo")]
    public async Task<IActionResult> ConcurrencyDemo(CancellationToken cancellationToken)
    {
        // Simulate two SEPARATE requests/staff members with two SEPARATE DbContext instances
        using var scopeA = scopeFactory.CreateScope();
        using var scopeB = scopeFactory.CreateScope();

        var contextA = scopeA.ServiceProvider.GetRequiredService<TmsDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<TmsDbContext>();

        // Both "load" the same student, each in their own context
        var studentA = await contextA.Students.OrderBy(s => s.Id).FirstAsync(cancellationToken);
        var studentB = await contextB.Students.OrderBy(s => s.Id).FirstAsync(cancellationToken);

        // Tab A updates and saves first
        studentA.Name = "Updated By A";
        await contextA.SaveChangesAsync(cancellationToken);

        // Tab B still has the STALE RowVersion, tries to save a different field
        studentB.GPA = 3.99m;

        try
        {
            await contextB.SaveChangesAsync(cancellationToken);
            return Ok("No conflict detected — this should NOT happen if RowVersion is configured correctly.");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Console.WriteLine($">>> CONCURRENCY CONFLICT CAUGHT: {ex.Message}");
            return Conflict(new { Message = "Concurrency conflict detected as expected.", Detail = ex.Message });
        }
}

[HttpPost("bulk-archive")]
public async Task<IActionResult> BulkArchiveOldEnrollments(CancellationToken cancellationToken)
{
    var cutoff = DateTime.UtcNow.AddDays(-1); // adjust cutoff as needed for testing

    Console.WriteLine("\n>>> BULK ARCHIVE: single UPDATE statement, no row loading...\n");

    var rowsAffected = await context.Enrollments
        .Where(e => e.EnrolledAt < cutoff && !e.IsArchived)
        .ExecuteUpdateAsync(s => s.SetProperty(e => e.IsArchived, true), cancellationToken);

    return Ok(new { ArchivedCount = rowsAffected });
}

[HttpGet("students/admin-restore-view")]
public async Task<IActionResult> AdminRestoreView(CancellationToken cancellationToken)
{
    // Bypasses the soft-delete filter — shows ALL students, including IsDeleted ones
    var all = await context.Students
        .IgnoreQueryFilters()
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    return Ok(all);
}
}

