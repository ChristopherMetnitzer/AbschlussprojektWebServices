using System.Threading;

public interface IStudentEventQueue
{
    Task EnqueueAsync(StudentEvent ev, CancellationToken ct = default);
    Task<StudentEvent> DequeueAsync(CancellationToken ct);
}
