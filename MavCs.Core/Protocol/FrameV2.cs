using System.Buffers;

namespace MavCs.Core.Protocol;


/// <summary>
/// MAVLink v2 frame format:
/// [0] magic=0xFD
/// [1] len
/// [2] incompat_flags
/// [3] compat_flags
/// [4] seq
/// [5] sysid
/// [6] compid
/// [7..9] msgid (3 bytes LE)
/// [10..] payload (len bytes)
/// [end-2, end-1] CRC (little-endian)
/// [optional 13 bytes signature]
/// CRC is CRC-16/X25 over: len..payload + crc_extra
/// </summary>

public sealed class FrameV2 : FrameBase
{
    public byte IncompatFlags { get; init; }
    public byte CompatFlags { get; init; }
    public ReadOnlyMemory<byte> Signature { get; init; } // empty if not present

    public static bool TryParse(
        ReadOnlySpan<byte> input,
        Func<uint, byte>? crcExtraProvider,
        out FrameV2? frame,
        out int bytesConsumed)
    {
        frame = null;
        bytesConsumed = 0;

        if (input.Length < 1 || input[0] != Constants.MagicV2) return false;
        if (input.Length < 1 + Constants.HeaderV2Size) return false;

        byte len = input[1];
        byte incompat = input[2];
        byte compat = input[3];
        byte seq = input[4];
        byte sys = input[5];
        byte comp = input[6];
        uint msgId = (uint)(input[7] | (input[8] << 8) | (input[9] << 16));

        int baseTotal = 1 + Constants.HeaderV2Size + len + Constants.CrcSize;
        if (input.Length < baseTotal) return false;

        bool hasSignature = (incompat & 0x01) != 0; // MAVLINK_IFLAG_SIGNED
        int total = baseTotal + (hasSignature ? Constants.V2SignatureSize : 0);
        if (input.Length < total) return false;
        
        ushort crc = Crc.Reset();
        // Start from incompat_flags (skip magic and len)
        crc = Crc.AccumulateSpan(crc, input.Slice(1, Constants.HeaderV2Size));

        var payloadSpan = input.Slice(1 + Constants.HeaderV2Size, len);
        crc = Crc.AccumulateSpan(crc, payloadSpan);

        if (crcExtraProvider is not null)
            crc = Crc.AccumulateByte(crc, crcExtraProvider(msgId));
        
        int crcIndex = 1 + Constants.HeaderV2Size + len;
        ushort crcFrame = (ushort)(input[crcIndex] | (input[crcIndex + 1] << 8));
        if (crc != crcFrame) return false;
        
        ReadOnlyMemory<byte> sig = ReadOnlyMemory<byte>.Empty;
        if (hasSignature)
            sig = input.Slice(crcIndex + 2, Constants.V2SignatureSize).ToArray();

        frame = new FrameV2()
        {
            IncompatFlags = incompat,
            CompatFlags = compat,
            Sequence = seq,
            SystemId = sys,
            ComponentId = comp,
            MessageId = msgId,
            Payload = payloadSpan.ToArray(),
            Signature = sig
        };
        bytesConsumed = total;
        return true;
    }

    public static void Write(
        in FrameV2 frame,
        IBufferWriter<byte> output,
        Func<uint, byte>? crcExtraProvider,
        ReadOnlySpan<byte> signature = default)
    {
        byte len = (byte)(frame.Payload?.Length ?? 0);
        Span<byte> header = stackalloc byte[1 + Constants.HeaderV2Size];
        header[0] = Constants.MagicV2;
        header[1] = len;
        header[2] = frame.IncompatFlags;
        header[3] = frame.CompatFlags;
        header[4] = frame.Sequence;
        header[5] = frame.SystemId;
        header[6] = frame.ComponentId;
        header[7] = (byte)(frame.MessageId & 0xFF);
        header[8] = (byte)((frame.MessageId >> 8) & 0xFF);
        header[9] = (byte)((frame.MessageId >> 16) & 0xFF);
        
        // CRC
        ushort crc = Crc.Reset();
        crc = Crc.AccumulateSpan(crc, header.Slice(1, Constants.HeaderV2Size)); // len..msgid (9 bayt)
        if (len > 0 && frame.Payload is not null)
            crc = Crc.AccumulateSpan(crc, frame.Payload);
        if (crcExtraProvider is not null)
            crc = Crc.AccumulateByte(crc, crcExtraProvider(frame.MessageId));
        
        // Write
        var writer = output;
        writer.Write(header);
        if (len > 0 && frame.Payload is not null)
            writer.Write(frame.Payload);

        Span<byte> crcBytes = stackalloc byte[2];
        crcBytes[0] = (byte)(crc & 0xFF);
        crcBytes[1] = (byte)(crc >> 8);
        writer.Write(crcBytes);

        if (!signature.IsEmpty)
            writer.Write(signature);
    }
}
