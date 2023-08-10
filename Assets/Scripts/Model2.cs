using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class Model2
{
    static float PI2         = 2 * Mathf.PI;
    static float MAX_ANGLE   = 0x1000;
    static int VERTEX_LEN  = 2 * 4;
    static int NORMAL_LEN  = 2 * 4; //VERTEX_LEN;
    static int TRI_IDX_LEN = 2 * 6;
    static int TRI_TEX_LEN = (1 + 1 + 2) * 3;
    static int QUA_IDX_LEN = 2 * 8;
    static int QUA_TEX_LEN = (1 + 1 + 2) * 4;

    public static MD emd(string file)
    {
        dataViewExt buf = _open(file);
        if ( buf == null ) return null;
        var md = new MD();
        group[] am_idx;

        am_idx = animation(buf, buf._offset(1));
        if (am_idx != null && am_idx.Length != 0) skeleton(md, am_idx, buf, buf._offset(2));

        am_idx = animation(buf, buf._offset(3));
        if (am_idx != null && am_idx.Length != 0) skeleton(md, am_idx, buf, buf._offset(4));

        am_idx = animation(buf, buf._offset(5));
        if (am_idx != null && am_idx.Length != 0) skeleton(md, am_idx, buf, buf._offset(6));

        md.meshs = mesh(buf, buf._offset(7));
        BoneNm.bind(file, md);
        //debug("Anim Group", md.pose_group.Count.ToString());
        return md;
    }

    private static dataViewExt _open(string file)
    {
        dataViewExt buf = File2.openDataView(file);
        if ( buf == null) return null;
        buf.h_dir_offset = buf.Ulong();
        buf.h_dir_count = buf.Ulong();

        //buf._offset = offset(0);
        debug(file, "DIR", buf.h_dir_offset.ToString(), buf.h_dir_offset.ToString());
        return buf;

        
    }

    public static group[] animation(dataViewExt buf, int am_off)
    {
        // 从第一个元素计算数量
        int count = buf.Ushort(am_off);
        int aoff  = buf.Ushort(am_off + 2);
        int total = aoff >> 2;
        debug("Anim", total.ToString(), am_off.ToString());
        if (total <= 0)
        {
            debug(" > No Anim");
            return null;
        }

        //const am_idx = [];
        group[] groups = new group[1];

        for (var i = 0; i < total; ++i)
        {
            var ec = buf.Ushort(am_off + i * 4);
            var offset = buf.Ushort();
            //var group = am_idx[i] = [];
            groups = new group[ec];
            buf.setpos(am_off + offset);
            debug(" >", i.ToString(), ec.ToString(), (am_off + offset).ToString());

            for (var j = 0; j < ec; ++j)
            {
                uint t = (uint)buf.Ulong();
                uint uint1 = 0xFFFFF800; uint flag = (t & uint1) >> 11;
                uint uint2 = 0x7FF; uint sk_idx = t & uint2;
                groups[j] = new group() { flag = (int)flag, sk_idx = (int)sk_idx };
                if (groups[j].flag == 0)
                {
                    debug("  - Frame", j.ToString(), "\tFlag", groups[j].flag.ToString(), "\tSK", groups[j].sk_idx.ToString());
                }
            }
        }
        return groups;
    }

    public static void skeleton(MD md, group[] am_idx, dataViewExt buf, int sk_offset)
    {
        debug("SK", sk_offset.ToString());
        // buf.print(sk_offset, 500);
        int ref_val     = buf.Ushort(sk_offset);
        int anim_val    = buf.Ushort();
        int count       = buf.Ushort();
        int size        = buf.Ushort();
        int ref_offset  = ref_val + sk_offset;
        int anim_offset = anim_val + sk_offset;
        int xyoff = sk_offset + 8;

        debug(" * Header", ref_val.ToString(), anim_val.ToString(), count.ToString(), size.ToString());
        if (size == 0)
        {
            debug(" * NO skeleton");
            return;
        }

        if (ref_val > 0)
        {
            __bone_bind(md, count, xyoff, ref_offset, buf);
        }

        //
        // 生化危机用的是关节骨骼模型, 有一个整体的 xyz 偏移和每个关节的角度.
        // 一个关节的转动会牵连子关节的运动
        //
        var get_frame_data = create_anim_frame_data(buf, anim_offset, size);

        md.addAnimSet(am_idx, get_frame_data);
        debug(" * POSE count", md.pose.Length.ToString());
    }

    public static void __bone_bind(MD md, int count, int xyoff, int ref_offset, dataViewExt buf)
    {
        // 复用骨骼
        List<SkeletonBone> bone = md.bone;
        //const bind = { };
        int[] bind = new int[count];
        int miny = 99999, maxy = 0;

        for (int i = 0; i < count; ++i)
        {
            //var sk = { child: [] };
            sk sk = new sk();
            sk.x = buf.Short(xyoff);
            sk.y = buf.Short();
            sk.z = buf.Short();
            xyoff += 6;
            miny = (int)Math.Min(miny, Math.Abs(sk.y));
            maxy = (int)Math.Max(maxy, Math.Abs(sk.y));

            // 子节点的数量
            var num_mesh = buf.Ushort(ref_offset + (i << 2));
            // 子节点引用数组偏移
            var ch_offset = buf.Ushort() + ref_offset;
            // debug('ch_offset', ch_offset, ref_offset);
            // 只有骨骼偏移, 复用绑定
            if (num_mesh >= count)
            {
                debug(" *! No bone bind", i.ToString());
                return;
            }

            bone.Add(new SkeletonBone(sk, i));
            for (var m = 0; m < num_mesh; ++m)
            {
                var chref = buf.Byte(ch_offset + m);
                //sk.child.push(chref);
                //bind[chref] = bone[i];
            }
            debug(" ** ", i.ToString(), sk.ToString());
        }

        for (var i=0; i<count; ++i) {
            if (bind[i] == 0) {
                //bone[i].parent = bind[i];
                //bind[i].child.push(bone[i]);
                debug(bone[i].toString());
            }
        }

        md.height = Math.Abs(maxy - miny);
        debug(" & Height", md.height.ToString());
    }

    public static int create_anim_frame_data(dataViewExt buf, int anim_offset, int data_size)
    {
        int xy_size    = 2 * 6;
        int angle_size = data_size - xy_size;
        int RLEN       = (int)(angle_size / 9 * 2); //parseInt
        // const skdata     = { angle: [] };
        float angle_fn   = Mathf.Rad2Deg; // radian & degrees
        sk[] sk_cache;
        // let curr_sk_idx  = -1;

        if (angle_size <= 0)
        {
            console.warn("NO more anim frame data");
            return 0;
        }

        debug(" * Anim begin", anim_offset.ToString(), data_size.ToString());
        debug(" * Anim angle", RLEN.ToString());
        // skdata.angle = new Array(RLEN);
        // for (let i=0; i<RLEN; ++i) {
        //   skdata.angle[i] = {x:0, y:0, z:0};
        // }

        //
        // sk_index - 骨骼状态索引
        //

        sk get_frame_data(dataViewExt buf, int sk_index)
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

        return 0;
    }

    public static void compute_angle(sk skdata, dataViewExt buf, int RLEN)
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

    private static float angle_fn(float n)
    {
        return (n / MAX_ANGLE) * PI2;
    }

    private static Mesh[] mesh(dataViewExt buf, int offset)
    {
        List<Vector3> vector3s = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> Triangles = new List<int>();
        List<int> Triangles2 = new List<int>();
        List<int> Triangles3 = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        int length    = buf.Ulong(offset);
        int uk        = buf.Ulong(offset + 4);
        int obj_count = buf.Ulong(offset + 8) >> 1;
        List<Mesh> meshObj = new List<Mesh>();
        int beginAt = buf.getpos();
        offset += 3 * 4;

        debug("MESH", beginAt.ToString(), "count", obj_count.ToString(), length.ToString(), uk.ToString());
        int o, c;

        // TODO: 艾达的面分配错误
        for (int i = 0; i < obj_count; i++)
        {
            Mesh mesh = new Mesh();
            mesh.subMeshCount = 1;
            // 三角形 index_offset 为顶点索引, tex 数量与 index 数量相同
            int[] vertex;
            int oo = buf.Ulong(offset) + beginAt;
            int cc = buf.Ulong();
            vertex = buildBufferInt16Array(buf, oo, cc, VERTEX_LEN); // Int16Array

            int[] normal;
            o = buf.Ulong() + beginAt;
            c = buf.Ulong();
            normal = buildBufferInt16Array(buf, o, c, NORMAL_LEN); // Int16Array

            int[] index;
            o = buf.Ulong() + beginAt;
            c = buf.Ulong();
            index = buildBufferUint16Array(buf, o, c, TRI_IDX_LEN); //Uint16Array

            int[] tex;
            o = buf.Ulong() + beginAt;
            tex = buildBufferUint8Array(buf, o, c, TRI_TEX_LEN); //Uint8Array
            //debug(" % T end", i, tri.vertex.count, h4(tri.vertex.offset));

            // 四边形
            //var qua = { };
            int[] vertex2;
            o = buf.Ulong() + beginAt;
            c = buf.Ulong();
            vertex2 = buildBufferInt16Array(buf, o, c, VERTEX_LEN); // Int16Array

            int[] normal2;
            o = buf.Ulong() + beginAt;
            c = buf.Ulong();
            normal2 = buildBufferInt16Array(buf, o, c, NORMAL_LEN); // Int16Array

            int[] index2;
            o = buf.Ulong() + beginAt;
            c = buf.Ulong(); 
            index2 = buildBufferUint16Array(buf, o, c, QUA_IDX_LEN); //Uint16Array

            int[] tex2;
            o = buf.Ulong() + beginAt;
            tex2 = buildBufferUint8Array(buf, o, c, QUA_TEX_LEN); //Uint8Array
            //debug(' % Q end', i, qua.vertex.count, h4(qua.vertex.offset));

            offset += 56;

            vector3s.Clear();
            normals.Clear();
            Triangles.Clear();
            Triangles2.Clear();
            Triangles3.Clear();
            uv.Clear();

            for (int k = 0; k < vertex.Length; k += 4)
            {
                Vector3 newvec = new Vector3(vertex[k] * -0.01f, vertex[k + 1] * -0.01f, vertex[k + 2] * -0.01f);
                vector3s.Add(newvec);
            }

            for (int k = 0; k < normal.Length; k += 4)
            {
                Vector3 newvec = new Vector3(normal[k] * -0.01f, normal[k + 1] * -0.01f, normal[k + 2] * -0.01f);
                normals.Add(newvec);
            }

            for (int k = 0; k < index.Length; k += 6)
            {
                Triangles.Add(index[k + 1]);
                Triangles.Add(index[k + 3]);
                Triangles.Add(index[k + 5]);
            }

            for (int k = 0; k < index2.Length; k += 8)
            {
                Triangles.Add(index2[k + 1]);
                Triangles.Add(index2[k + 3]);
                Triangles.Add(index2[k + 7]);
                
                Triangles.Add(index2[k + 1]);
                Triangles.Add(index2[k + 7]);
                Triangles.Add(index2[k + 5]);
            }

            Vector3Int param = GetTextureParams(buf.path);

            Dictionary<int,Vector2> uvs = new Dictionary<int, Vector2>();

            //int indexV = 0;
            //foreach (var triangle in Triangles)
            //{
            //    if (!uvs.ContainsKey(triangle))
            //    {
            //        float u = (float)tex[indexV] / (float)param.x;
            //        float v = (float)tex[indexV + 1] / (float)param.y;
            //        uvs.Add(triangle, new Vector2(u, v));
            //    }
            //    ++indexV;
            //    if (indexV % 12 == 3)
            //    {
            //        indexV = (((indexV - 3) / 12) + 1) * 12;
            //    }
            //}

            for (int k = 0; k < index.Length/12; k++)
            {
                int ui = k * 12;

                uv.Add(GetUV(param, ui ));
                uv.Add(GetUV(param, ui + 4));
                uv.Add(GetUV(param, ui + 8));
            }

            Vector2 GetUV(Vector3Int param, int ui)
            {
                int off_unit = param.x / param.z;
                int offx = off_unit * (tex[ui + 6] & 3);
                float u = (float)(tex[ui] + offx) / (float)param.x;
                float v = (float)tex[ui + 1] / (float)param.y;
                return new Vector2(u, v);
            }

            //ShowMesh.Show(vector3s.ToArray());

            //uv = uvs.OrderBy(w => w.Key).Select(w=>w.Value).ToList();

            if ( uv.Count < vector3s.Count)
            {
                for (; uv.Count < vector3s.Count; )
                {
                    uv.Add(new Vector2());
                }
            }

            mesh.vertices = vector3s.ToArray();
            if ( normals.Count <= vector3s.Count ) mesh.normals = normals.ToArray();
            
            //if (max > vector3s.Count)
            //{
            //    Debug.LogWarning(i.ToString() + " " + buf.path);
            //}

            mesh.SetTriangles(Triangles, 0);
            //mesh.SetTriangles(Triangles2, 1);
            //mesh.SetTriangles(Triangles3, 2);

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.SetUVs(0, uv.ToArray());
            meshObj.Add(mesh);
        }

        return meshObj.ToArray();
    }

    private static Vector3Int GetTextureParams(string path)
    {
        Vector3Int paras = new Vector3Int();
        //DataView buf = default(DataView);
        path = path.Remove(path.Length - 3, 3);
        path += "TIM";
        dataViewExt buf = File2.openDataView(path);

        int type = (int)buf.getUint32(4, true);
        int offset = (int)buf.getUint32(8, true);
        int pal_x = buf.getUint16(12, true);
        int pal_y = buf.getUint16(14, true);
        int palette_colors = buf.getUint16(16, true);
        int nb_palettes = buf.getUint16(18, true);
        int vi = 20;

        //console.debug('TIM palettes color', palette_colors, 'nb', nb_palettes, 
        //'pal-x', pal_x, 'pal-y', pal_y, 'offset', offset);

        // 调色板被纵向平均应用到图像上
        List<List<int>> palettes = new List<List<int>>(nb_palettes);
        for (int p = 0; p < nb_palettes; ++p)
        {
            //palettes[p] = new Uint16Array(buf.buffer, buf.byteOffset + vi, palette_colors);
            palettes.Add(buildBufferUint16Array(buf, buf.byteOffset + vi, palette_colors, 1).ToList());
            vi += palette_colors * 2;
            // console.debug("Palette", p);
            // h.printHex(palettes[p]);
        }

        paras.x = Tim._width(type, buf.getUint16(vi + 8, true)); // width
        paras.y = buf.getUint16(vi + 10, true); // height
        paras.z = buf.getUint16(18, true); // nb_palettes

        return paras;
    }

    public static int[] buildBufferInt16Array(DataView buf, int offset, int count, int stride)
    {
        Int16[] array = buf.build_Int16Array(buf, offset, count * stride);
        int[] arrayInt = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            arrayInt[i] = array[i];
        }
        return arrayInt;
    }

    public static float[] buildBufferFloat16Array(DataView buf, int offset, int count, int stride)
    {
        float[] array = buf.build_Float16Array(buf, offset, count * stride);
        float[] arrayInt = new float[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            if (Single.IsNaN(array[i])) arrayInt[i] = 0;
            arrayInt[i] = array[i];
        }
        return arrayInt;
    }

    public static int[] buildBufferUint16Array(DataView buf, int offset, int count, int stride)
    {
        UInt16[] array = buf.build_UInt16Array(buf, offset, count * stride);
        int[] arrayInt = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            arrayInt[i] = array[i];
        }
        return arrayInt;
    }

    public static int[] buildBufferUint8Array(DataView buf, int offset, int count, int stride)
    {
        UInt16[] array = buf.build_UInt8Array(buf, offset, count * (int)stride);
        int[] arrayInt = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            arrayInt[i] = array[i];
        }
        return arrayInt;
    }

    private static void debug(params string[] messages)
    {
        string message = "";
        for (int i = 0; i < messages.Length; i++)
        {
            message += messages[i] + " ";
        }
        //Debug.Log(message);
    }
}

public class group
{
    public int flag;
    public int sk_idx;

    //public group(int flag, int sk_idx) 
    //{ 
    //    this.flag = flag;
    //    this.sk_idx = sk_idx;
    //}
}

public class sk
{
    public int x;
    public int y;
    public int z;
    public int spx;
    public int spy;
    public int spz;
    public int frameTime;
    public int moveStep;
    public Vector3[] angle;
}