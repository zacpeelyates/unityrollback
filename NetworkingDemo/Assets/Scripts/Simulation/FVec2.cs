using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FVec2
{
    public FInt32 x, y;

    public FVec2(FInt32 xpos, FInt32 ypos)
    {
        x = xpos;
        y = ypos;
    }

    public Vector3 ToVec3(float zPos) => new Vector3(x.ToFloat, y.ToFloat, zPos);
    public FInt32 Magnitude => FInt32.Sqrt(FInt32.Pow(x, 2) + FInt32.Pow(y, 2));

    public static FInt32 Distance(FVec2 a, FVec2 b) => FInt32.Sqrt(FInt32.Pow(a.x - b.x, 2) + FInt32.Pow(a.y - b.y, 2));


    public static FVec2 operator +(FVec2 a, FVec2 b) => new FVec2(a.x + b.x, a.y + b.y);
    public static FVec2 operator -(FVec2 a, FVec2 b) => new FVec2(a.x - b.x, a.y - b.y);
    public static FVec2 operator /(FVec2 a, FVec2 b) => new FVec2(a.x / b.x, a.y / b.y);
    public static FVec2 operator *(FVec2 a, FVec2 b) => new FVec2(a.x * b.x, a.y * b.y);
    public static bool operator ==(FVec2 a, FVec2 b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(FVec2 a, FVec2 b) => a.x != b.x || a.y != b.y;

    public static bool operator >(FVec2 a, FVec2 b) => a.Magnitude > b.Magnitude;
    public static bool operator <(FVec2 a, FVec2 b) => a.Magnitude < b.Magnitude;

    public static bool operator <=(FVec2 a, FVec2 b) => a.Magnitude <= b.Magnitude;
    public static bool operator >=(FVec2 a, FVec2 b) => a.Magnitude >= b.Magnitude;

}
