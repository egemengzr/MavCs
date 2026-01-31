namespace MavCs.Core.Abstractions;

public interface IMavTransport : IAsyncDisposable
{
    Task StartAsync(CancellationToken ct);

    IAsyncEnumerable<ReadOnlyMemory<byte>> ReceiveAsync(CancellationToken ct);

    ValueTask<int> SendAsync(ReadOnlyMemory<byte> frame, CancellationToken ct);
}
