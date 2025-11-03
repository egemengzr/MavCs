namespace MavCs.Core.Abstractions;

using MavCs.Core.Protocol;

/// <summary>
/// Attempts to parse a MAVLink frame from the input buffer.
/// Returns true if a complete frame was consumed.
/// </summary>

public interface IMavLinkDecoder
{
    
    bool TryReadFrame(
        ReadOnlySpan<byte> input,
        out FrameBase? frame,
        out int bytesConsumed);
}
