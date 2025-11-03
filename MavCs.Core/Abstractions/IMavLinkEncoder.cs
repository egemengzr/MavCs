using System.Buffers;

namespace MavCs.Core.Abstractions;

public interface IMavLinkEncoder
{
    // Encodes a MAVLink v1 frame and writes it to the provided output buffer.
    // <typeparam name="TMessage">Type of the message being encoded.</typeparam>
    // <param name="message">Message object to serialize into the payload.</param>
    // <param name="serializer">Serializer responsible for packing the payload bytes.</param>
    // <param name="messageId">Numeric MAVLink message ID.</param>
    // <param name="sequence">Frame sequence number.</param>
    // <param name="systemId">System ID of the sender.</param>
    // <param name="componentId">Component ID of the sender.</param>
    // <param name="output">Buffer writer to write the final frame bytes.</param>
    // <returns>Total number of bytes written to the buffer.</returns>
    int WriteV1<TMessage>(
        TMessage message,
        IMessageSerializer<TMessage> serializer,
        uint messageId,
        byte sequence,
        byte systemId,
        byte componentId,
        IBufferWriter<byte> output);
    
    // Encodes a MAVLink v2 frame and writes it to the provided output buffer.
    
    // <typeparam name="TMessage">Type of the message being encoded.</typeparam>
    // <param name="message">Message object to serialize into the payload.</param>
    // <param name="serializer">Serializer responsible for packing the payload bytes.</param>
    // <param name="messageId">Numeric MAVLink message ID.</param>
    // <param name="sequence">Frame sequence number.</param>
    // <param name="systemId">System ID of the sender.</param>
    // <param name="componentId">Component ID of the sender.</param>
    // <param name="output">Buffer writer to write the final frame bytes.</param>
    // <param name="incompatFlags">MAVLink v2 incompatibility flags (bitmask).</param>
    // <param name="compatFlags">MAVLink v2 compatibility flags (bitmask).</param>
    // <param name="signature">Optional 13-byte signature for signed messages.</param>
    // <returns>Total number of bytes written to the buffer.</returns>
    int WriteV2<TMessage>(
        TMessage message,
        IMessageSerializer<TMessage> serializer,
        uint messageId,
        byte sequence,
        byte systemId,
        byte componentId,
        IBufferWriter<byte> output,
        byte incompatFlags = 0,
        byte compatFlags = 0,
        ReadOnlySpan<byte> signature = default);
}
