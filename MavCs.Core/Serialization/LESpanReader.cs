using System;

namespace MavCs.Core.Serialization;
// Minimal little-endian reader over a ReadOnlySpan<byte>.
public ref struct LESpanReader
{
    private ReadOnlySpan<byte> _src;
    private int _pos;

    public LESpanReader(ReadOnlySpan<byte> source)
    {
        this._src = source;
        this._pos = 0;
    }

    public int BytesConsumed => this._pos;

    public byte ReadByte()
    {
        if (this._pos >= this._src.Length) throw new ArgumentOutOfRangeException(nameof(this._src));
        return this._src[this._pos++];
    }

    public uint ReadUInt32()
    {
        if (this._pos + 4 > this._src.Length) throw new ArgumentOutOfRangeException(nameof(this._src));
        uint v = (uint)(this._src[this._pos] | (this._src[this._pos + 1] << 8) | (this._src[this._pos + 2] << 16) 
                        | (this._src[this._pos + 3] << 24));
        this._pos += 4;
        return v;
    }
}
