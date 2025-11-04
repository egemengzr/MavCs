using System.Runtime.CompilerServices;

namespace MavCs.Core.Protocol;

public static class Crc
{
    // 256-entry lookup table for reflected poly 0x8408 (X25)
    private static readonly ushort[] Table = new ushort[256];

    static Crc()
    {
        // build table once (can hardcode if istersen)
        for (int i = 0; i < 256; i++)
        {
            ushort crc = (ushort)i;
            for (int bit = 0; bit < 8; bit++)
            {
                bool lsb = (crc & 1) != 0;
                crc >>= 1;
                if (lsb) crc ^= 0x8408; // reflected 0x1021
            }
            Table[i] = crc;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Reset() => 0xFFFF;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Finalize(ushort crc) => (ushort)~crc;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort AccumulateByte(ushort crc, byte b)
    {
        int idx = (crc ^ b) & 0xFF;
        return (ushort)((crc >> 8) ^ Table[idx]);
    }

    public static ushort AccumulateSpan(ushort crc, ReadOnlySpan<byte> data)
    {
        for (int i = 0; i < data.Length; i++)
            crc = AccumulateByte(crc, data[i]);
        return crc;
    }
}
