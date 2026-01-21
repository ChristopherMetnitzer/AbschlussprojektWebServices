using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class StudentEventWorker : BackgroundService
{
    private readonly IStudentEventQueue _queue;
    private readonly ILogger<StudentEventWorker> _logger;

    public StudentEventWorker(
        IStudentEventQueue queue,
        ILogger<StudentEventWorker> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var ev = await _queue.DequeueAsync(stoppingToken);

            _logger.LogInformation(
                "ASYNC EVENT: {Type} | StudentId={Id} | {Time}",
                ev.Type,
                ev.StudentId,
                ev.TimestampUtc
            );
        }
    }
}
