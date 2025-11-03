namespace MavCs.Core.Abstractions;

// Serializes/deserializes a specific MAVLink message payload (without frame).
// Implementations must match MAVLink wire order and little-endian layout.

public interface IMessageSerializer<TMessage>
{
    // Writes payload to <paramref name="dst"/> and returns number of bytes written
    int Write(TMessage message, System.Span<byte> dst);
    
    // Reads payload from <paramref name="src"/> and returns a populated message
    TMessage Read(System.ReadOnlySpan<byte> src);
}
