using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class anim_frame_data
{
    static float PI2 = 2 * Mathf.PI;
    static float MAX_ANGLE = 0x1000;

    public List<sk> sk_cache;
    public int anim_offset;
    public int data_size;
    public int RLEN;

    public static anim_frame_data create_anim_frame_data(dataViewExt buf, int anim_offset, int data_size)
    {
        anim_frame_data anim_Frame_Data = new anim_frame_data();
        int xy_size = 2 * 6;
        int angle_size = data_size - xy_size;
        anim_Frame_Data.anim_offset = anim_offset;
        anim_Frame_Data.data_size = data_size;
        anim_Frame_Data.RLEN = (int)(angle_size / 9 * 2); //parseInt
        // const skdata     = { angle: [] };
        float angle_fn = Mathf.Rad2Deg; // radian & degrees
        List<sk> sk_cache = new List<sk>();//sk[] sk_cache;
        var curr_sk_idx = -1;

        if (angle_size <= 0)
        {
            console.warn("NO more anim frame data");
            return null;
        }

        //debug(" * Anim begin", anim_offset.ToString(), data_size.ToString());
        //debug(" * Anim angle", RLEN.ToString());
        // skdata.angle = new Array(RLEN);
        // for (let i=0; i<RLEN; ++i) {
        //   skdata.angle[i] = {x:0, y:0, z:0};
        // }

        //
        // sk_index - 骨骼状态索引
        //

        return anim_Frame_Data;
    }

    public sk get_frame_data(dataViewExt buf, int sk_index)
    {
        // debug(" * Frame sk", sk_index);
        // 没有改变骨骼索引直接返回最后的数据
        sk sk = sk_cache[sk_index];
        if (sk != null) return sk;

        sk = sk_cache[sk_index];
        // if (curr_sk_idx === sk_index) return skdata;
        // 整体位置偏移量
        var xy_off = anim_offset + data_size * sk_index;
        // buf.print(xy_off, 0x20);
        sk.x = buf.Short(xy_off);
        sk.y = buf.Short();
        sk.z = buf.Short();
        // spx 似乎和动画帧绝对时间有关, spy 总是0
        // spz 是移动偏移, 体现步伐之间的非线性移动
        sk.spx = buf.Short();
        sk.spy = buf.Short();
        sk.spz = buf.Short();
        // 动画帧停留时间
        // if (sk_index == 0) {
        //   sk.frameTime = Math.abs(sk.spx);
        // } else {
        //   sk.frameTime = Math.abs(sk.spx - get_frame_data(sk_index-1).spx);
        // }
        sk.frameTime = (sk.spx < 0 ? -sk.spx : sk.spx) & 0x3F;
        // 应该是某种索引, 总是有规律的递增/递减
        sk.moveStep = sk.spx >> 6;

        sk.angle = new Vector3[RLEN];
        compute_angle(sk, buf, RLEN);
        // debug(JSON.stringify(sk), RLEN);
        return sk;
    }

    public void compute_angle(sk skdata, dataViewExt buf, int RLEN)
    {
        var i = -1;
        Vector3 r;
        int a0, a1, a2, a3, a4;
        while (++i < RLEN)
        {
            r = skdata.angle[i];
            a0 = buf.Byte();
            a1 = buf.Byte();
            a2 = buf.Byte();
            a3 = buf.Byte();
            a4 = buf.Byte();
            // debug('joint', i, b2(a0), b2(a1), b2(a2), b2(a3), b2(a4));
            r.x = angle_fn(a0 | ((a1 & 0xF) << 8));
            r.y = angle_fn((a1 >> 4) | (a2 << 4));
            r.z = angle_fn(a3 | ((a4 & 0xF) << 8));
            // debug(r.x, r.y, r.z);

            if (++i >= RLEN) break;

            r = skdata.angle[i];
            a0 = a4;
            a1 = buf.Byte();
            a2 = buf.Byte();
            a3 = buf.Byte();
            a4 = buf.Byte();
            // debug('joint', i, b2(a0), b2(a1), b2(a2), b2(a3), b2(a4));
            r.x = angle_fn((a0 >> 4) | (a1 << 4));
            r.y = angle_fn(a2 | ((a3 & 0xF) << 8));
            r.z = angle_fn((a3 >> 4) | (a4 << 4));
            // debug(r.x, r.y, r.z);
        }
    }

    private float angle_fn(float n)
    {
        return (n / MAX_ANGLE) * PI2;
    }
}
