using Microsoft.Extensions.DependencyInjection;
using TmsApi.Application.Interfaces;

namespace TmsApi.Infrastructure.Services;

public class EnrollmentWorker(IServiceScopeFactory scopeFactory)
{
    public void ProcessBatch()
    {
        using var scope = scopeFactory.CreateScope();

        var svc = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        // Placeholder for future enrollment processing.
    }
}