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

    public void WriteByte(byte value)
    {
        if (this._pos >= this._dst.Length) throw new ArgumentOutOfRangeException(nameof(this._dst));
        this._dst[this._pos++] = value;
    }

    public void WriteUIint32(uint value)
    {
        if (_pos + 4 > _dst.Length) throw new ArgumentOutOfRangeException(nameof(this._dst));
        this._dst[this._pos + 0] = (byte)(value & 0xFF);
        this._dst[this._pos + 1] = (byte)((value >> 8) & 0xFF);
        this._dst[this._pos + 2] = (byte)((value >> 16) & 0xFF);
        this._dst[this._pos + 3] = (byte)((value >> 24) & 0xFF);
        this._pos += 4;
    }
}
