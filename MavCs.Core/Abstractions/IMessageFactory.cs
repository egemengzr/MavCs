using MavCs.Core.Protocol;

namespace MavCs.Core.Abstractions;

// Resolves and deserializes MAVLink messages by ID Using registered serializers
public interface IMessageFactory
{
    bool TryDeserializeFrame(FrameBase frame, out object? message);
}
