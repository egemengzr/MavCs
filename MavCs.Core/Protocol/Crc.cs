using System.Runtime.CompilerServices;

namespace MavCs.Core.Protocol;

public static class Crc
{
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Reset() => 0xFFFF;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort AccumulateByte(ushort current, byte b)
    {
        ushort crc = current;
        crc ^= b;
        for (int i = 0; i < 8; i++)
        {
            bool lsb = (crc & 1) != 0;
            crc >>= 1;
            if (lsb) crc ^= 0x8408; // reflected 0x1021
        }
        return crc;
    }

    public static ushort AccumulateSpan(ushort current, ReadOnlySpan<byte> data)
    {
        ushort crc = current;
        foreach (var b in data)
            crc = AccumulateByte(crc, b);
        return crc;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Finalize(ushort current) => (ushort)~current;

    public static ushort Compute(ReadOnlySpan<byte> data)
        => Finalize(AccumulateSpan(Reset(), data));
}
