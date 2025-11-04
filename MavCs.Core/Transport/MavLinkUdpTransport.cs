using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;

using MavCs.Core.Abstractions;

namespace MavCs.Core.Transport;

public sealed class MavLinkUdpTransport : IMavTransport
{
    private readonly string _host;
    private readonly int _remotePort;
    private readonly int _localPort;

    private UdpClient? _rx;   // bind(local)
    private UdpClient? _tx;   // connect(remote)

    public int LocalPort => _localPort;
    public int RemotePort => _remotePort;

    public MavLinkUdpTransport(string host, int remotePort, int localPort)
    {
        _host = host;
        _remotePort = remotePort;
        _localPort = localPort;
    }

    public Task StartAsync(CancellationToken ct)
    {
        _rx = new UdpClient(new IPEndPoint(IPAddress.Any, _localPort));
        _tx = new UdpClient();
        _tx.Connect(_host, _remotePort);
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<ReadOnlyMemory<byte>> ReceiveAsync([EnumeratorCancellation] CancellationToken ct)
    {
        if (_rx is null) yield break;
        while (!ct.IsCancellationRequested)
        {
            var result = await _rx.ReceiveAsync(ct);
            yield return result.Buffer;
        }
    }

    public async ValueTask<int> SendAsync(ReadOnlyMemory<byte> frame, CancellationToken ct)
    {
        if (_tx is null) return 0;
           return await _tx.SendAsync(frame, ct);
    }

    public ValueTask DisposeAsync()
    {
        _rx?.Dispose();
        _tx?.Dispose();
        _rx = null; _tx = null;
        return ValueTask.CompletedTask;
    }
}
