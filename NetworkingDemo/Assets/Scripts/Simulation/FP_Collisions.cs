using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FP_AABB
{

    public FInt32 x, y, w, h; //xpos, ypos, width, height
    public void SetBounds(FInt32 xpos, FInt32 ypos, FInt32 width, FInt32 height)
    {
        x = xpos;
        y = ypos;
        w = width;
        h = height;
    }
    public static bool Colliding(FP_AABB a, FP_AABB b) => a.x < b.x + b.w && a.x + a.w > b.x && b.y < b.y + b.h && a.y + a.h > b.y;
}
