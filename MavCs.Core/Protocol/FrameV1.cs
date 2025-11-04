using System.Buffers;
using System.Reflection.Metadata;

namespace MavCs.Core.Protocol;

/// <summary>
/// MAVLink v1 frame format:
/// [0] magic=0xFE
/// [1] len
/// [2] seq
/// [3] sysid
/// [4] compid
/// [5] msgid (1 byte)
/// [6..] payload (len bytes)
/// [end-2, end-1] CRC (little-endian)
/// CRC is CRC-16/X25 over: len..payload + crc_extra
/// </summary>

public sealed class FrameV1 : FrameBase
{
    public static bool TryParse(
        ReadOnlySpan<byte> input,
        Func<uint, byte>? crcExtraProvider,
        out FrameV1? frame,
        out int bytesConsumed)
    {
        frame = null;
        bytesConsumed = 0;

        if (input.Length < 1 || input[0] != Constants.MagicV1) return false;
        if (input.Length < 1 + Constants.HeaderV1Size) return false;

        byte len = input[1];
        int total = 1 + Constants.HeaderV1Size + len + Constants.CrcSize;
        if (input.Length < total) return false;

        byte seq = input[2];
        byte sys = input[3];
        byte comp = input[4];
        byte msgId = input[5];
        
        // CRC starts from len (exclude magic=0xFE)
        ushort crc = Crc.Reset();
        crc = Crc.AccumulateSpan(crc, input.Slice(1, Constants.HeaderV1Size));

        // payload
        var payloadSpan = input.Slice(1 + Constants.HeaderV1Size, len);
        crc = Crc.AccumulateSpan(crc, payloadSpan);

        // crc_extra
        if (crcExtraProvider is not null)
            crc = Crc.AccumulateByte(crc, crcExtraProvider(msgId));


        int crcIndex = total - Constants.CrcSize;
        ushort crcFrame = (ushort)(input[crcIndex] | (input[crcIndex + 1] << 8));
        if (crc != crcFrame) return false;

        frame = new FrameV1()
        {
            Sequence = seq,
            SystemId = sys,
            ComponentId = comp,
            MessageId = msgId,
            Payload = payloadSpan.ToArray()
        };

        bytesConsumed = total;
        return true;
    }

    public static void Write(
        in FrameV1 frame,
        IBufferWriter<byte> output,
        Func<uint, byte>? crcExtraProvider)
    {
        byte len = (byte)(frame.Payload?.Length ?? 0);
        Span<byte> header = stackalloc byte[1 + Constants.HeaderV1Size];
        header[0] = Constants.MagicV1;
        header[1] = len;
        header[2] = frame.Sequence;
        header[3] = frame.SystemId;
        header[4] = frame.ComponentId;
        header[5] = (byte)frame.MessageId;
        
        // Compute CRC
        ushort crc = Crc.Reset();
        
        crc = Crc.AccumulateSpan(crc, header.Slice(1, Constants.HeaderV1Size)); // exclude magic
        if (len > 0 && frame.Payload is not null)
            crc = Crc.AccumulateSpan(crc, frame.Payload);

        if (crcExtraProvider is not null)
            crc = Crc.AccumulateByte(crc, crcExtraProvider(frame.MessageId));

        
        // Write out
        var writer = output;
        writer.Write(header);
        if (len > 0 && frame.Payload is not null)
            writer.Write(frame.Payload);

        Span<byte> crcBytes = stackalloc byte[2];
        crcBytes[0] = (byte)(crc & 0xFF);
        crcBytes[1] = (byte)(crc >> 8);
        writer.Write(crcBytes);
    }
}
