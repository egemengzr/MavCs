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
        
        // Compute CRC over header (from len) + payload
        ushort crc = 0xFFFF;
        for (int i = 1; i < 1 + Constants.HeaderV1Size; i++)
            crc = Crc.Accumulate(crc, input[i]);

        ReadOnlySpan<byte> payloadSpan = input.Slice(1 + Constants.HeaderV1Size, len);
        foreach (byte b in payloadSpan)
            crc = Crc.Accumulate(crc, b);
        if (crcExtraProvider is not null)
        {
            byte extra = crcExtraProvider(msgId);
            crc = Crc.Accumulate(crc, extra);
        }
        
        // Finalize CRC (one's complement)
        crc = (ushort)~crc;
        
        // Extract CRC from frame (little-endian)
        int crcIndex = total - Constants.CrcSize;
        ushort crcFrame = (ushort)(input[crcIndex] | (input[crcIndex + 1] << 8));
        if (crc != crcFrame) return false;
        
        // Slice payload
        byte[] payload = payloadSpan.ToArray();

        frame = new FrameV1()
        {
            Sequence = seq,
            SystemId = sys,
            ComponentId = comp,
            MessageId = msgId,
            Payload = payload
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
        ushort crc = 0xFFFF;
        for (int i = 1; i < header.Length; i++)
            crc = Crc.Accumulate(crc, header[i]);

        if (len > 0 && frame.Payload is not null)
        {
            foreach (byte b in frame.Payload)
                crc = Crc.Accumulate(crc, b);
        }

        if (crcExtraProvider is not null)
        {
            byte extra = crcExtraProvider(frame.MessageId);
            crc = Crc.Accumulate(crc, extra);
        }
        crc = (ushort)~crc;
        
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
