using MavCs.Core.Protocol;
using MavCs.Core.Abstractions;

namespace MavCs.Core.Runtime;

/// <summary>
/// Version-aware decoder that dispatches to V1/V2 based on magic byte.
/// </summary>

public sealed class MavLinkDecoder : IMavLinkDecoder
{
    private readonly Func<uint, byte>? _crcExtraProvider;

    // New: accept registry and wrap it as provider
    public MavLinkDecoder(IMessageRegistry? registry = null)
    {
        if (registry is not null)
        {
            _crcExtraProvider = id => registry.GetCrcExtra(id) ?? 0;
        }
    }
    
    // Back-compat ctor.
    public MavLinkDecoder(Func<uint, byte>? crcExtraProvider)
    {
        _crcExtraProvider = crcExtraProvider;
    }
    
    public bool TryReadFrame(ReadOnlySpan<byte> input, out FrameBase? frame, out int bytesConsumed)
    {
        frame = null;
        bytesConsumed = 0;
        if (input.Length == 0) return false;

        byte magic = input[0];
        switch (magic)
        {
            case Constants.MagicV1:
                if (FrameV1.TryParse(input, this._crcExtraProvider, out var v1, out bytesConsumed))
                {
                    frame = v1;
                    return true;
                }
                return false;
            case Constants.MagicV2:
                if (FrameV2.TryParse(input, this._crcExtraProvider, out var v2, out bytesConsumed))
                {
                    frame = v2;
                    return true;
                }
                return false;
            default:
                // Unknown leading byte; caller should drop one byte and retry.
                bytesConsumed = 1;
                return false;
        }
    }
}
