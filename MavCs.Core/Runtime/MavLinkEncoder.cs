using System.Buffers;
using MavCs.Core.Abstractions;
using MavCs.Core.Messages;
using MavCs.Core.Protocol;
using MavCs.Tests.Runtime;

namespace MavCs.Core.Runtime;

// High-level MAVLink encoder.
// Wraps payload serialization and frame writing (v1/v2),
// using CRC extras from a message registry.

public sealed partial class MavLinkEncoder : IMavLinkEncoder
{
    private readonly IMessageRegistry _registry;

    public MavLinkEncoder(IMessageRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    private byte GetExtra(uint msgId) => _registry.GetCrcExtra(msgId) ?? 0;

    // Encodes MAVLink v1 frame (auto-discovery of serializer & metadata)
    public int WriteV1<TMessage>(
        TMessage message,
        IMessageSerializer<TMessage> _,
        uint __,
        byte sequence,
        byte systemId,
        byte componentId,
        IBufferWriter<byte> output)
    {
        (uint resolvedId, byte resolvedCrc) = SerializerResolver.GetMetadata(typeof(TMessage));
        
        var serializerObj = SerializerResolver.GetSerializer(typeof(TMessage))
                           ?? throw new InvalidOperationException($"Serializer not found for {typeof(TMessage).Name}");
        var serializer = (IMessageSerializer<TMessage>)serializerObj;

        Span<byte> payload = stackalloc byte[255];
        int written = serializer.Write(message!, payload);

        var frame = new FrameV1
        {
            Sequence = sequence,
            SystemId = systemId,
            ComponentId = componentId,
            MessageId = resolvedId,
            Payload = payload[..written].ToArray()
        };

        FrameV1.Write(frame, output, _ => resolvedCrc);
        return 1 + Constants.HeaderV1Size + written + Constants.CrcSize;
    }

    // Method overload
    public int WriteV1<TMessage>(
        TMessage message,
        byte sequence,
        byte systemId,
        byte componentId,
        IBufferWriter<byte> output)
    {
        return WriteV1(message, (IMessageSerializer<TMessage>)null!, 0u, sequence, systemId, componentId, output);
    }
    
    // Encodes MAVLink v2 frame (auto-discovery of serializer & metadata)
    public int WriteV2<TMessage>(
        TMessage message,
        IMessageSerializer<TMessage> _ /*ignored*/,
        uint _msgId /*ignored*/,
        byte sequence,
        byte systemId,
        byte componentId,
        IBufferWriter<byte> output,
        byte incompatFlags = 0,
        byte compatFlags = 0,
        ReadOnlySpan<byte> signature = default)
    {
        (uint resolvedId, byte resolvedCrc) = SerializerResolver.GetMetadata(typeof(TMessage));

        var serializerObj = SerializerResolver.GetSerializer(typeof(TMessage))
                           ?? throw new InvalidOperationException($"Serializer not found for {typeof(TMessage).Name}");
        var serializer = (IMessageSerializer<TMessage>)serializerObj;

        Span<byte> payload = stackalloc byte[255];
        int written = serializer.Write(message!, payload);

        if (typeof(TMessage) == typeof(StatustextMessage))
        {
            Console.WriteLine($"[DBG] STATUSTEXT written={written} (expected 54)");
            // quick dump of first bytes
            var span = payload[..Math.Min(written, 64)];
            Console.WriteLine("[DBG] payload: " + BitConverter.ToString(span.ToArray()));
        }
        
        var frame = new FrameV2
        {
            IncompatFlags = incompatFlags,
            CompatFlags = compatFlags,
            Sequence = sequence,
            SystemId = systemId,
            ComponentId = componentId,
            MessageId = resolvedId,
            Payload = payload[..written].ToArray()
        };

        FrameV2.Write(frame, output, _ => resolvedCrc, signature);

        int baseLen = 1 + Constants.HeaderV2Size + written + Constants.CrcSize;
        int sigLen = signature.IsEmpty ? 0 : Constants.V2SignatureSize;
        return baseLen + sigLen;
    }
    
    // Method overload
    public int WriteV2<TMessage>(
        TMessage message,
        byte sequence,
        byte systemId,
        byte componentId,
        IBufferWriter<byte> output,
        byte incompatFlags = 0,
        byte compatFlags = 0,
        ReadOnlySpan<byte> signature = default)
    {
        return WriteV2(message, (IMessageSerializer<TMessage>)null!, 0u, sequence, systemId, componentId, output, incompatFlags, compatFlags, signature);
    }
    
}
