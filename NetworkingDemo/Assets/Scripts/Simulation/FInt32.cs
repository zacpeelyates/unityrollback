//32 bit fixed point implementation
//should allow for cross-architecture determinism
//uses some unsafe math. faster (and easier to write) but requires user to not be dividing by zero or over/underflowing
using System;

public class FInt32
{
    public Int32 m_raw { get; private set; }

    public static readonly FInt32 MAX = new FInt32(Int32.MaxValue);
    public static readonly FInt32 MIN = new FInt32(Int32.MinValue);
    const int POINT = 16; 
    public static readonly int MAX_DECIMAL = (1 << POINT)-1;
    public static readonly int MAX_INTEGRAL = ~MAX_DECIMAL;
    
    public static FInt32 MakeFromParts(int integral, int fractional) => new FInt32((integral << POINT) + fractional);

    public Int32 Integral => m_raw >> POINT;
    public Int32 Decimal => m_raw & MAX_DECIMAL;

    new public string ToString => ToDouble.ToString();
    public double ToDouble => (double)m_raw / (1 << POINT);
    private FInt32(Int32 raw) => m_raw = raw;
    public static FInt32 MakeFromRaw(Int32 raw) => new FInt32(raw);


    public static FInt32 operator +(FInt32 a, FInt32 b) => new FInt32(a.m_raw + b.m_raw);
    public static FInt32 operator -(FInt32 a, FInt32 b) => new FInt32(a.m_raw - b.m_raw);
    public static FInt32 operator %(FInt32 a, FInt32 b) => new FInt32(a.m_raw % b.m_raw);
    public static FInt32 operator *(FInt32 a, FInt32 b) => new FInt32((a.m_raw * b.m_raw) >> POINT);
    public static FInt32 operator /(FInt32 a, FInt32 b) => new FInt32((a.m_raw << POINT) / b.m_raw);

    public static FInt32 operator *(FInt32 f, int i) => new FInt32(f.m_raw * i);
    public static bool operator <(FInt32 a, FInt32 b) => a.m_raw < b.m_raw;
    public static bool operator >(FInt32 a, FInt32 b) => a.m_raw > b.m_raw;
    public static bool operator >=(FInt32 a, FInt32 b) => a.m_raw >= b.m_raw;
    public static bool operator <=(FInt32 a, FInt32 b) => a.m_raw <= b.m_raw;
    public static bool operator ==(FInt32 a, FInt32 b) => a.m_raw == b.m_raw;
    public static bool operator !=(FInt32 a, FInt32 b) => a.m_raw != b.m_raw;


}


