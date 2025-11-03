using System.Collections.Concurrent;

using MavCs.Core.Abstractions;
using MavCs.Core.Protocol;
using MavCs.Core.Serialization;

namespace MavCs.Core.Runtime;

public sealed class MavMessageFactory : IMessageFactory
{
    private readonly ConcurrentDictionary<uint, Func<ReadOnlySpan<byte>, object>> _deserialized = new();

    public MavMessageFactory()
    {
        RegisterDefaults();
    }
    
    // Registers built-in message deserializers
    private void RegisterDefaults()
    {
        // For now only heartbeat
        var heartbeat = new HeartbeatSerializer();
        this._deserialized[0u] = span => heartbeat.Read(span);
    }

    public bool TryDeserializeFrame(FrameBase frame, out object? message)
    {
        if (this._deserialized.TryGetValue(frame.MessageId, out var func))
        {
            message = func(frame.Payload);
            return true;
        }

        message = null;
        return false;
    }
}
