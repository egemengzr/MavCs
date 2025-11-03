namespace MavCs.Core.Protocol;

public class Crc
{
    // Mavlink uses CRC-16/X25 (polly 0x1021), init 0xFFFF, xor out 0x0000
    public static ushort Compute(ReadOnlySpan<byte> data)
    {
        ushort crc = 0xFFFF;
        foreach (byte b in data)
        {
            crc ^= b;
            for (int i = 0; i < 8; i++)
            {
                bool lsb = (crc & 1) != 0;
                crc >>= 1;
                if (lsb)
                    crc ^= 0x8408;  // reversed 0x1021
            }
        }
        return (ushort)~crc;        // ones-complement
    }
    
    // For adding "extra CRC" byte for Mavlink
    public static ushort Accumulate(ushort current, byte b)
    {
        ushort crc = current;
        crc ^= b;
        for (int i = 0; i < 8; i++)
        {
            bool lsb = (crc & 1) != 0;
            crc >>= 1;
            if (lsb) crc ^= 0x8408;
        }

        return crc;
    }
}
