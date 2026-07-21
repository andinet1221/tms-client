using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("RequestLoggingMiddleware is running");
        // Generate correlation ID
        var correlationId = Guid.NewGuid().ToString("N")[..8];

        // Add response header BEFORE next()
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        // Start timer
        var stopwatch = Stopwatch.StartNew();

        // Log request
        _logger.LogInformation(
            "Request {Method} {Path} CorrelationId={CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            correlationId);

        // Continue pipeline
        await _next(context);

        // Stop timer
        stopwatch.Stop();

        // Log response
        _logger.LogInformation(
            "Response {StatusCode} {Elapsed}ms CorrelationId={CorrelationId}",
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            correlationId);
    }
}