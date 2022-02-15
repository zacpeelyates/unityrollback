//32 bit fixed point implementation
//should allow for cross-architecture determinism
//uses a lot of unsafe maths. faster (and easier to write) but requires user to not be dividing by zero or over/underflowing
using System;

public class FInt32
{
    readonly Int32 m_raw;

    public static readonly FInt32 MAX = new FInt32(Int32.MaxValue);
    public static readonly FInt32 MIN = new FInt32(Int32.MinValue);
    const int POINT = 16; // 1 sign place, 15 integer places, 16 fractional places

    public static FInt32 MakeFromRaw(Int32 raw)
    {
        return new FInt32(raw);
    }

    public static FInt32 MakeFromParts(Int16 integral, Int32 fractional)
    {
        return new FInt32((integral << POINT) + fractional);
    }


    public Int16 Integral => (Int16)(m_raw >> POINT);
    public Int16 Decimal => (Int16)(m_raw & 0x00000000FFFFFFFF);

    private FInt32(Int32 raw)
    {
        m_raw = raw;
    }

    public static FInt32 operator *(FInt32 a, FInt32 b)
    {

        Int32 decResult = (a.Decimal * b.Decimal) >> POINT;
        Int32 intResult = (a.Integral * b.Integral) << POINT;
        Int32 m1Result = (a.Decimal * b.Integral);
        Int32 m2Result = (a.Integral * b.Decimal);

        return new FInt32(decResult + intResult + m1Result + m2Result);
    }
    
    public static FInt32 operator /(FInt32 a, FInt32 b)
    {
        //TODO
        return new FInt32(Int32.MinValue);
    }


    public static FInt32 operator +(FInt32 a, FInt32 b) => new FInt32(a.m_raw + b.m_raw);
    public static FInt32 operator -(FInt32 a, FInt32 b) => new FInt32(a.m_raw - b.m_raw);
    public static FInt32 operator %(FInt32 a, FInt32 b) => new FInt32(a.m_raw & b.m_raw);
    public static bool operator <(FInt32 a, FInt32 b) => a.m_raw < b.m_raw;
    public static bool operator >(FInt32 a, FInt32 b) => a.m_raw > b.m_raw;
    public static bool operator >=(FInt32 a, FInt32 b) => a.m_raw >= b.m_raw;
    public static bool operator <=(FInt32 a, FInt32 b) => a.m_raw <= b.m_raw;









}
