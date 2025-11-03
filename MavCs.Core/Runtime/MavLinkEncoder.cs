using System.Buffers;
using System.Reflection.Metadata;

using MavCs.Core.Abstractions;
using MavCs.Core.Protocol;

namespace MavCs.Core.Runtime;

// High-level MAVLink encoder.
// Wraps payload serialization and frame writing (v1/v2),
// using CRC extras from a message registry.

public sealed class MavLinkEncoder : IMavLinkEncoder
{
    private readonly IMessageRegistry _registry;

    public MavLinkEncoder(IMessageRegistry registry)
    {
        this._registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    private byte GetExtra(uint msgId)
        => this._registry.GetCrcExtra(msgId) ?? 0;
    
    // Encodes Mavlink v1 frame
    public int WriteV1<TMessage>(
        TMessage message,
        IMessageSerializer<TMessage> serializer,
        uint messageId,
        byte sequence,
        byte systemId,
        byte componentId,
        IBufferWriter<byte> output)
    {
        // 1. Serialize payload
        Span<byte> payload = stackalloc byte[255];
        int written = serializer.Write(message, payload);
        
        // 2. Build frame
        var frame = new FrameV1
        {
            Sequence = sequence,
            SystemId = systemId,
            ComponentId = componentId,
            MessageId = messageId,
            Payload = payload[..written].ToArray()
        };
        
        // 3. Write frame with correct CRC
        FrameV1.Write(frame, output, this.GetExtra);

        return 1 + Constants.HeaderV1Size + written + Constants.CrcSize;
    }
    
    // Encodes Mavlink v2 messages
    public int WriteV2<TMessage>(
        TMessage message,
        IMessageSerializer<TMessage> serializer,
        uint messageId,
        byte sequence,
        byte systemId,
        byte componentId,
        IBufferWriter<byte> output,
        byte incompatFlags = 0,
        byte compatFlags = 0,
        ReadOnlySpan<byte> signature = default)
    {
        Span<byte> payload = stackalloc byte[255];
        int written = serializer.Write(message, payload);

        var frame = new FrameV2
        {
            IncompatFlags = incompatFlags,
            CompatFlags = compatFlags,
            Sequence = sequence,
            SystemId = systemId,
            ComponentId = componentId,
            MessageId = messageId,
            Payload = payload[..written].ToArray()
        };
        
        FrameV2.Write(frame, output, this.GetExtra, signature);

        int baseLen = 1 + Constants.HeaderV2Size + written + Constants.CrcSize;
        int sigLen = signature.IsEmpty ? 0 : Constants.V2SignatureSize;
        return baseLen + sigLen;
    }
}
