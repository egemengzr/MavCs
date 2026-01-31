using System.Buffers;

using MavCs.Core.Abstractions;
using MavCs.Core.Registry;

namespace MavCs.Core.Runtime;

public class MavlinkConnection
{
    private readonly IMavTransport _transport;
    private readonly MavLinkEncoder _encoder;
    
    public byte SystemId { get; }
    public byte ComponentId { get; }
    
    private byte _sequence = 0;
    private readonly object _lock = new();
    
    public MavlinkConnection(IMavTransport transport, byte systemId = 255, byte componentId = 0)
    {
        _transport = transport;
        SystemId = systemId;
        ComponentId = componentId;
        _encoder = new MavLinkEncoder(new KnownMessages());
    }
    
    public async Task SendMessageAsync<T>(T message, CancellationToken ct = default) where T : class
    {
        var writer = new ArrayBufferWriter<byte>();
        
        byte currentSeq;
        lock (_lock)
        {
            currentSeq = _sequence++;
        }
        
        _encoder.WriteV2(message, currentSeq, SystemId, ComponentId, writer);
        await _transport.SendAsync(writer.WrittenMemory, ct);
    }
    
}
