using System.IO.Ports;
using System.Runtime.CompilerServices;
using MavCs.Core.Abstractions;

namespace MavCs.Core.Transport;

public sealed class MavLinkSerialTransport : IMavTransport
{
    private readonly SerialPort _port;

    public MavLinkSerialTransport(string portName, int baudRate)
    {
        _port = new SerialPort(portName, baudRate)
        {
            ReadTimeout = 100,
            WriteTimeout = 100
        };
    }
    
    public async Task StartAsync(CancellationToken ct)
    {
        _port.Open();
        await Task.CompletedTask;
    }

    public async IAsyncEnumerable<ReadOnlyMemory<byte>> ReceiveAsync([EnumeratorCancellation] CancellationToken ct)
    {
        var buffer = new byte[512];
        while (!ct.IsCancellationRequested)
        {
            int read = await _port.BaseStream.ReadAsync(buffer.AsMemory(), ct);
            if (read > 0)
                yield return buffer.AsMemory(0, read);
        }
    }

    public async ValueTask<int> SendAsync(ReadOnlyMemory<byte> frame, CancellationToken ct)
    {
        await _port.BaseStream.WriteAsync(frame, ct);
        return frame.Length;
    }

    public ValueTask DisposeAsync()
    {
        this._port.Dispose();
        return ValueTask.CompletedTask;
    }
}
