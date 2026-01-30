using System;
namespace MavCs.Core.Serialization;

// Minimal little-endian writer over a caller-provided Span<byte>.
// No allocations; throws if not enough space.

public ref struct LESpanWriter
{
    private Span<byte> _dst;
    private int _pos;

    public LESpanWriter(Span<byte> destination)
    {
        this._dst = destination;
        this._pos = 0;
    }

    public int BytesWritten => this._pos;

    private void Ensure(int n)
    {
        if (this._pos + n > this._dst.Length)
            throw new ArgumentOutOfRangeException(nameof(this._dst));
    }

    public void WriteByte(byte value)
    {
        this.Ensure(1);
        this._dst[this._pos++] = value;
    }

    public void WriteSByte(sbyte value)
    {
        this.Ensure(1);
        this._dst[this._pos++] = (byte)value;
    }

    public void WriteUInt16(ushort value)
    {
        this.Ensure(2);
        this._dst[this._pos++] = (byte)(value & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 8) & 0xFF);
    }

    public void WriteInt16(short value)
    {
        this.Ensure(2);
        this._dst[this._pos++] = (byte)(value & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 8) & 0xFF);
    }

    public void WriteUInt32(uint value)
    {
        this.Ensure(4);
        this._dst[this._pos++] = (byte)(value & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 8) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 16) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 24) & 0xFF);
    }
    
    public void WriteUInt64(ulong value)
    {
        this.Ensure(8);
        this._dst[this._pos++] = (byte)(value & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 8) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 16) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 24) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 32) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 40) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 48) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 56) & 0xFF);
    }

    public void WriteInt64(long value)
    {
        this.WriteUInt64(unchecked((ulong)value));
    }
    
    public void WriteBytes(ReadOnlySpan<byte> src)
    {
        this.Ensure(src.Length);
        src.CopyTo(this._dst.Slice(this._pos, src.Length));
        this._pos += src.Length;
    }
    
    public void WriteFloat(float value)
    {
        // Get IEEE-754 bytes, write as LE 
        uint bits = unchecked((uint)BitConverter.SingleToInt32Bits(value));
        this.WriteUInt32(bits);
    }
    
    public void WriteInt32(int value)
    {
        this.Ensure(4);
        this._dst[this._pos++] = (byte)(value & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 8) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 16) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 24) & 0xFF);
    }
}
