using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Model2
{
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

    public static MD pld(string file)
    {
        dataViewExt buf = _open(file);
        MD md = new MD();

        group[] am_idx = animation(buf, buf._offset(0));
        if (am_idx != null && am_idx.Length != 0) skeleton(md, am_idx, buf, buf._offset(1));

        md.meshs = mesh(buf, buf._offset(2));

        DataView timbuf = new DataView(buf.buffer, buf._offset(3));
        //md.tex = Tim.parseStream(timbuf);
        return md;
    }

    public static MD rbj(dataViewExt buf, int sk_off, int anim_off)
    {
        MD md = new MD();
        group[] am_idx = animation(buf, anim_off);
        skeleton(md, am_idx, buf, sk_off);
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

        int[] am_idx;
        group[] groups = new group[1];

        for (var i = 0; i < total; ++i)
        {
            var ec = buf.Ushort(am_off + i * 4);
            var offset = buf.Ushort();
            //var group = am_idx[i];
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
        var get_frame_data = anim_frame_data.create_anim_frame_data(buf, anim_offset, size);

        md.addAnimSet(am_idx, get_frame_data);
        debug(" * POSE count", md.pose.Count.ToString());
    }

    public static void __bone_bind(MD md, int count, int xyoff, int ref_offset, dataViewExt buf)
    {
        // 复用骨骼
        List<SkeletonBone> bone = md.bone;
        //const bind = { };
        List<SkeletonBone> bind = new List<SkeletonBone>(count);
        for (int i = 0; i < count; i++)
        {
            bind.Add(null);
        }
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
                sk.child.Add(chref); //push
                bind[chref] = bone[i];
            }
            debug(" ** ", i.ToString(), sk.ToString());
        }

        for (var i=0; i<count; ++i) {
            if (bind[i] != null) {
                bone[i].parent = bind[i];
                bind[i].child.Add(bone[i]); // push
                //debug(bone[i].toString());
            }
        }

        md.height = Math.Abs(maxy - miny);
        debug(" & Height", md.height.ToString());
    }

    private static Mesh[] mesh(dataViewExt buf, int offset)
    {
        List<Vector3> vector3s = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> Triangles = new List<int>();

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
            vertex = DataView.buildBufferInt16Array(buf, oo, cc, VERTEX_LEN); // Int16Array

            int[] normal;
            o = buf.Ulong() + beginAt;
            c = buf.Ulong();
            normal = DataView.buildBufferInt16Array(buf, o, c, NORMAL_LEN); // Int16Array

            int[] index;
            o = buf.Ulong() + beginAt;
            c = buf.Ulong();
            index = DataView.buildBufferUint16Array(buf, o, c, TRI_IDX_LEN); //Uint16Array

            int[] tex;
            o = buf.Ulong() + beginAt;
            tex = DataView.buildBufferUint8Array(buf, o, c, TRI_TEX_LEN); //Uint8Array
            //debug(" % T end", i, tri.vertex.count, h4(tri.vertex.offset));

            // 四边形
            //var qua = { };
            int[] vertex2;
            o = buf.Ulong() + beginAt;
            c = buf.Ulong();
            vertex2 = DataView.buildBufferInt16Array(buf, o, c, VERTEX_LEN); // Int16Array

            int[] normal2;
            o = buf.Ulong() + beginAt;
            c = buf.Ulong();
            normal2 = DataView.buildBufferInt16Array(buf, o, c, NORMAL_LEN); // Int16Array

            int[] index2;
            o = buf.Ulong() + beginAt;
            c = buf.Ulong(); 
            index2 = DataView.buildBufferUint16Array(buf, o, c, QUA_IDX_LEN); //Uint16Array

            int[] tex2;
            o = buf.Ulong() + beginAt;
            tex2 = DataView.buildBufferUint8Array(buf, o, c, QUA_TEX_LEN); //Uint8Array
            //debug(' % Q end', i, qua.vertex.count, h4(qua.vertex.offset));

            offset += 56;

            vector3s.Clear();
            normals.Clear();
            Triangles.Clear();

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

            mesh.vertices = vector3s.ToArray();
            if ( normals.Count <= vector3s.Count ) mesh.normals = normals.ToArray();

            mesh.SetTriangles(Triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            var uvss = GetUVs(buf, vector3s, index, index2, tex, tex2 );
            mesh.SetUVs(0, uvss[0].ToArray());
            mesh.SetUVs(1, uvss[1].ToArray());
            //mesh.SetUVs(1, uv2.ToArray());
            meshObj.Add(mesh);
        }

        return meshObj.ToArray();
    }

    public static List<List<Vector2>> GetUVs(dataViewExt buf, List<Vector3> vector3s, int[] index, int[] index2, int[] tex, int[] tex2)
    {
        List<List<Vector2>> uvss = new List<List<Vector2>>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector2> uv2 = new List<Vector2>();

        Vector3Int param = GetTextureParams(buf.path);

        Dictionary<int, Vector2> uvs = new Dictionary<int, Vector2>();
        Dictionary<int, Vector2> uvs2 = new Dictionary<int, Vector2>();
        int off_unit = param.x / param.z;

        for (int k = 0; k < index.Length - 1; k += 6)
        {
            int ui = k * 2;

            Vector2 x = GetUV(param, ui);
            Vector2 y = GetUV(param, ui + 4);
            Vector2 z = GetUV(param, ui + 8);

            Vector2 x1 = new Vector2(Mathf.Abs(1 - x.x), Mathf.Abs(1 - x.y));
            Vector2 y1 = new Vector2(Mathf.Abs(1 - y.x), Mathf.Abs(1 - y.y));
            Vector2 z1 = new Vector2(Mathf.Abs(1 - z.x), Mathf.Abs(1 - z.y));

            //uvs.TryAdd(index[k + 1], x);
            //uvs.TryAdd(index[k + 3], z);
            //uvs.TryAdd(index[k + 5], z1);

            uvs.TryAdd(index[k + 1], x);
            uvs.TryAdd(index[k + 3], y);
            uvs.TryAdd(index[k + 5], z);
        }

        for (int k = 0; k < index2.Length - 1; k += 8)
        {
            int ui = k * 2;

            Vector2 x = GetUV2(param, ui);

            Vector2 y = GetUV2(param, ui + 4);
            Vector2 z = GetUV2(param, ui + 8);
            Vector2 q = GetUV2(param, ui + 12);

            uvs.TryAdd(index2[k + 1], x);
            uvs.TryAdd(index2[k + 3], y);
            uvs.TryAdd(index2[k + 5], q);
            uvs.TryAdd(index2[k + 7], z);

            

            //uvs.TryAdd(index2[k + 1], x);
            //uvs.TryAdd(index2[k + 5], z);
            //uvs.TryAdd(index2[k + 7], q);
        }

        Vector2 GetUV(Vector3Int param, int uii)
        {
            int offx = off_unit * (tex[uii + 3] & 3);
            float u = (float)(tex[uii] + offx) / (float)param.x;
            float v = (float)(tex[uii + 1]) / (float)param.y;
            return new Vector2(u, v);
        }

        Vector2 GetUV2(Vector3Int param, int uii)
        {
            //int offx = off_unit * (tex2[uii + 3] & 3);
            float u = (float)(tex2[uii] + 0) / (float)param.x;
            float v = (float)(tex2[uii + 1]) / (float)param.y;
            return new Vector2(u, v);
        }

        Debug.Log(vector3s.Count.ToString() + " " + uvs.Count().ToString());
        for (int iii = 0; iii < vector3s.Count; iii++)
        {
            if (uvs.ContainsKey(iii))
            {
                uv.Add(uvs[iii]);
            }
            else
            {
                uv.Add(new Vector2());
            }
        }

        for (int iii = 0; iii < vector3s.Count; iii++)
        {
            if (uvs2.ContainsKey(iii))
            {
                uv2.Add(uvs2[iii]);
            }
            else
            {
                uv2.Add(new Vector2());
            }
        }

        uvss.Add(uv);
        uvss.Add(uv2);
        return uvss;
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
            palettes.Add(DataView.buildBufferUint16Array(buf, buf.byteOffset + vi, palette_colors, 1).ToList());
            vi += palette_colors * 2;
            // console.debug("Palette", p);
            // h.printHex(palettes[p]);
        }

        paras.x = Tim._width(type, buf.getUint16(vi + 8, true)); // width
        paras.y = buf.getUint16(vi + 10, true); // height
        paras.z = buf.getUint16(18, true); // nb_palettes

        return paras;
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
