namespace CSX.Common.Data.Primitives;

public readonly partial struct Byte16
{
    // Components : Bytes

    public byte Byte0 => (byte)(High >> 56);
    public byte Byte1 => (byte)(High >> 48);
    public byte Byte2 => (byte)(High >> 40);
    public byte Byte3 => (byte)(High >> 32);

    public byte Byte4 => (byte)(High >> 24);
    public byte Byte5 => (byte)(High >> 16);
    public byte Byte6 => (byte)(High >> 8);
    public byte Byte7 => (byte)(High >> 0);

    public byte Byte8 => (byte)(Low >> 56);
    public byte Byte9 => (byte)(Low >> 48);
    public byte Byte10 => (byte)(Low >> 40);
    public byte Byte11 => (byte)(Low >> 32);

    public byte Byte12 => (byte)(Low >> 24);
    public byte Byte13 => (byte)(Low >> 16);
    public byte Byte14 => (byte)(Low >> 8);
    public byte Byte15 => (byte)(Low >> 0);

    // Components : Unsigned Shorts

    // Components : Unsigned Ints
}