using System.Buffers;

using MavCs.Core.Abstractions;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;
using MavCs.Core.Transport;

namespace MavCs.Core.Connection;

public sealed class MavConnection : IAsyncDisposable
{
    private readonly IMavTransport _transport;
    private readonly CancellationTokenSource _cts = new();
    private Task? _rxLoop;

    private readonly MavLinkEncoder _encoder;
    private readonly MavLinkDecoder _decoder;
    private readonly MessageDispatcher _dispatcher;
    private readonly KnownMessages _messages = new KnownMessages();
    
    private byte _seq;
    public int SysId { get; }
    public int CompId { get; }

    private MavConnection(IMavTransport transport, int sysId, int compId)
    {
        this._transport = transport;
        this.SysId = sysId;
        this.CompId = compId;

        this._encoder = new MavLinkEncoder(this._messages);
        this._decoder = new MavLinkDecoder(this._messages);
        this._dispatcher = new MessageDispatcher();
    }

    public static MavConnection Connect(string uriLike, int? sysId = null, int? compId = null)
    {
        var spec = ConnectionSpec.Parse(uriLike, sysId, compId);

        IMavTransport transport = spec.Kind switch
        {
            ConnectionKind.Udp => new MavLinkUdpTransport(spec.Host!, spec.RemotePort!.Value, spec.LocalPort!.Value),
            // ConnectionKind.Serial => new SerialTransport(spec.PortName!, spec.Baud!.Value),
            _ => throw new NotSupportedException("Unknown connection kind")
        };
        
        var conn = new MavConnection(transport, spec.SysId, spec.CompId);
        conn.Start();
        return conn;

    }
    
    public async ValueTask SendAsync(object message, CancellationToken ct = default)
    {
        var buffer = new ArrayBufferWriter<byte>(512);

        _ = _encoder.WriteV2(
            message,
            sequence: _seq++,
            systemId: (byte)SysId,
            componentId: (byte)CompId,
            output: buffer
        );

        // Asıl veriyi gönder: buffer.WrittenMemory
        await _transport.SendAsync(buffer.WrittenMemory, ct);

    }

    public void OnMessage<T>(Action<T> handler) where T : class
        => _dispatcher.Register(handler);

    private void Start()
        => _rxLoop = Task.Run(ReceiveDecodeDispatchLoop);

    private async Task ReceiveDecodeDispatchLoop()
    {
        var ct = _cts.Token;

        try
        {
            await _transport.StartAsync(ct);

            await foreach (var chunk in _transport.ReceiveAsync(ct))
            {
                var span = chunk.Span;
                while (span.Length > 0)
                {
                    if (_decoder.TryReadFrame(span, out var frame, out var consumed))
                    {
                        if (frame is not null)
                            _dispatcher.Dispatch(frame); // FrameBase or derivatives
                        span = span.Slice(consumed);
                    }
                    else
                    {
                        span = span.Slice(Math.Min(consumed, 1));
                    }
                }
            }
        }
        catch (OperationCanceledException) { /* normal shutdown */ }
        catch (Exception ex)
        {
            // TODO: OnError event
            Console.Error.WriteLine($"[MavConnection] RX error: {ex}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        if (_rxLoop is not null)
            await Task.WhenAny(_rxLoop, Task.Delay(5000));
        await _transport.DisposeAsync();
        _cts.Dispose();
    }
}
