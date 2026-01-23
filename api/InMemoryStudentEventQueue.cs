using System.Threading;
using System.Threading.Channels;

public class InMemoryStudentEventQueue : IStudentEventQueue
{
    private readonly Channel<StudentEvent> _queue =
        Channel.CreateUnbounded<StudentEvent>();

    public async Task EnqueueAsync(StudentEvent ev, CancellationToken ct = default)
    {
        await _queue.Writer.WriteAsync(ev, ct);
    }

    public async Task<StudentEvent> DequeueAsync(CancellationToken ct)
    {
        return await _queue.Reader.ReadAsync(ct);
    }
}
