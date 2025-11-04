using System.IO.Pipelines;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using MavCs.Core.Abstractions;

namespace MavCs.Tests.Integrations;

public class SerialPortIntegrationTests
{
        [Fact]
    public async Task Serial_Loopback_Roundtrip()
    {
        // This test uses 2 virtual port for tests
        // Simulating Write → Read flow.
        using var sendPipe = new AnonymousPipeServerStream(PipeDirection.Out);
        using var recvPipe = new AnonymousPipeClientStream(PipeDirection.In, sendPipe.ClientSafePipeHandle);

        var dummy = new DummySerialTransport(sendPipe, recvPipe);

        await dummy.StartAsync(CancellationToken.None);

        var payload = new byte[] { 10, 20, 30, 40, 50 };

        await dummy.SendAsync(payload, CancellationToken.None);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        await foreach (var chunk in dummy.ReceiveAsync(cts.Token))
        {
            Assert.Equal(payload, chunk.ToArray());
            return;
        }

        Assert.Fail("No data received via loopback");
    }

    private sealed class DummySerialTransport : IMavTransport
    {
        private readonly PipeWriter _writer;
        private readonly PipeReader _reader;

        public DummySerialTransport(Stream sendStream, Stream recvStream)
        {
            _writer = PipeWriter.Create(sendStream);
            _reader = PipeReader.Create(recvStream);
        }

        public Task StartAsync(CancellationToken ct) => Task.CompletedTask;

        public async IAsyncEnumerable<ReadOnlyMemory<byte>> ReceiveAsync(
            [EnumeratorCancellation] CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var result = await _reader.ReadAsync(ct);
                var buffer = result.Buffer;
                foreach (var segment in buffer)
                    yield return segment.ToArray();
                _reader.AdvanceTo(buffer.End);
            }
        }

        public async ValueTask<int> SendAsync(ReadOnlyMemory<byte> data, CancellationToken ct)
        {
            await _writer.WriteAsync(data, ct);
            await _writer.FlushAsync(ct);
            return data.Length;
        }

        public ValueTask DisposeAsync()
        {
            _writer.Complete();
            _reader.Complete();
            return ValueTask.CompletedTask;
        }
    }

}
