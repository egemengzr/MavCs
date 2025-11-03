using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;

namespace MavCs.Core.Transport;

public sealed class MavLinkUdpTransport : IAsyncDisposable
{
    private readonly UdpClient _client;
    private readonly IPEndPoint _remoteEndPoint;
    private readonly CancellationTokenSource _cts = new();
    private Task? _listenerTask;

    public event EventHandler<ReadOnlyMemory<byte>>? FrameReceived;
    
    public int LocalPort { get; }
    public int RemotePort { get; }
    public string RemoteHost { get; }

    public MavLinkUdpTransport(int localPort, string remoteHost, int remotePort)
    {
        this.LocalPort = localPort;
        this.RemoteHost = remoteHost;
        this.RemotePort = remotePort;
        
        this._client = new UdpClient(localPort);
        this._remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteHost), remotePort);
    }

    public IPEndPoint? LocalEndpoint => _client.Client.LocalEndPoint as IPEndPoint;
    public IPEndPoint RemoteEndpoint => _remoteEndPoint;

    public async ValueTask StartAsync()
    {
        if (this._listenerTask != null)
            throw new InvalidOperationException("Listener already running");
        this._listenerTask = Task.Run(() => ListenAsync(this._cts.Token));
        await Task.Yield();
    }

    private async Task ListenAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var result = await this._client.ReceiveAsync(token);
                FrameReceived?.Invoke(this, result.Buffer);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MavLinkUdpTransport] Receive error: {ex.Message}");
            }
        }
    }

    public async ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken token = default)
    {
        await _client.SendAsync(data.ToArray(), data.Length, _remoteEndPoint);
    }

    public async ValueTask DisposeAsync()
    {
        this._cts.Cancel();

        if (this._listenerTask != null)
            await _listenerTask;
        
        this._client.Dispose();
        this._cts.Dispose();
    }
}
