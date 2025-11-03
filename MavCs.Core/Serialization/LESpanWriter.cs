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

    public void WriteUIint32(uint value)
    {
        this.Ensure(4);
        this._dst[this._pos++] = (byte)(value & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 8) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 16) & 0xFF);
        this._dst[this._pos++] = (byte)((value >> 24) & 0xFF);
    }
}
