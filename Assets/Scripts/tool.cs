using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class tool
{
    //public int randomInt(int x)
    //{
    //    return parseInt(Math.random() * x);
    //}

    public static string bit(int b)
    {
        var s = toString(b, 2);
        switch (s.Length)
        {
            case 1: return "0000000" + s;
            case 2: return "000000" + s;
            case 3: return "00000" + s;
            case 4: return "0000" + s;
            case 5: return "000" + s;
            case 6: return "00" + s;
            case 7: return "0" + s;
        }
        return s;
    }


    // 格式不可动
    public static string b2(int a)
    {
        if (a < 0x10)
        {
            return "0" + toString(a, 16);
        }
        else
        {
            return toString(a, 16);
        }
    }


    // 格式不可动
    public static string d3(int a)
    {
        if (a < 10) return "00" + a;
        if (a < 100) return "0" + a;
        return "" + a;
    }


    // 格式不可动
    public static string d2(int a)
    {
        if (a < 10) return "0" + a;
        return "" + a;
    }


    public static string d4(int a)
    {
        if (a < 0)
        {
            if (a > -10) return "-000" + -a;
            if (a > -100) return "-00" + -a;
            if (a > -1000) return "-0" + -a;
        }
        else
        {
            if (a < 10) return "000" + a;
            if (a < 100) return "00" + a;
            if (a < 1000) return "0" + a;
        }
        return "" + a;
    }


    public static string h4(int x)
    {
        if (x < 0) return "-0x" + toString((-x),16);
        if (x < 0x10) return "0x000" + toString(x, 16);
        if (x < 0x100) return "0x00" + toString(x, 16);
        if (x < 0x1000) return "0x0" + toString(x, 16);
        return "0x" + toString(x, 16);
    }


    public static string h2(int x)
    {
        if (x < 0) return "-0x" + toString((-x), 16);
        if (x < 0x10) return "0x0" + toString(x, 16);
        return "0x" + toString(x, 16);
    }

    public static string toString(int value, int rank)
    {
        if (rank == 16)
        {
            return value.ToString("X");
        }
        return "";
    }

    //public bool inRange(Vector4 r, Vector4 x, Vector4 y)
    //{
    //    // const w = who.where();
    //    // const x = w[0], y = w[2];
    //    int a, b, c, d;

    //    a = (r.x2 - r.x1) * (y - r.y1) - (r.y2 - r.y1) * (x - r.x1);
    //    b = (r.x3 - r.x2) * (y - r.y2) - (r.y3 - r.y2) * (x - r.x2);
    //    c = (r.x4 - r.x3) * (y - r.y3) - (r.y4 - r.y3) * (x - r.x3);
    //    d = (r.x1 - r.x4) * (y - r.y4) - (r.y1 - r.y4) * (x - r.x4);

    //    if ((a > 0 && b > 0 && c > 0 && d > 0) ||
    //        (a < 0 && b < 0 && c < 0 && d < 0))
    //    {
    //        return true;
    //    }
    //    return false;
    //}
}
