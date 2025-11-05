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

    private void Ensure(int n)
    {
        if (this._pos + n > this._src.Length)
            throw new ArgumentOutOfRangeException(nameof(_src));
    }
    public byte ReadByte()
    {
        this.Ensure(1);
        return this._src[this._pos++];
    }

    public sbyte ReadSByte()
    {
        this.Ensure(1);
        return unchecked((sbyte)this._src[this._pos++]);
    }

    public ushort ReadUInt16()
    {
        this.Ensure(2);
        ushort v = (ushort)(this._src[this._pos] | (this._src[this._pos + 1] << 8));
        this._pos += 2;
        return v;
    }

    public short ReadInt16()
    {
        this.Ensure(2);
        short v = (short)(this._src[this._pos] | (this._src[this._pos + 1] << 8));
        this._pos += 2;
        return v;
    }

    public uint ReadUInt32()
    {
        this.Ensure(4);
        uint v = (uint)(this._src[this._pos] | 
                        (this._src[this._pos + 1] << 8) | 
                        (this._src[this._pos + 2] << 16) | 
                        (this._src[this._pos + 3] << 24));
        this._pos += 4;
        return v;
    }
    
    public ReadOnlySpan<byte> ReadBytes(int count)
    {
        this.Ensure(count);
        var slice = _src.Slice(_pos, count);
        this._pos += count;
        return slice;
    }
}
