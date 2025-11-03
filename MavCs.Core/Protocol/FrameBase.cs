namespace MavCs.Core.Protocol;

/// <summary>
/// Common information shared by MAVLink frames.
/// </summary>
public abstract class FrameBase
{
    public byte Sequence { get; init; }
    public byte SystemId { get; init; }
    public byte ComponentId { get; init; }
    public uint MessageId { get; init; }
    public byte[] Payload { get; init; } = Array.Empty<byte>();
}
