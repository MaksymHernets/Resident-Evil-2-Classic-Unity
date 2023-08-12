﻿//
// https://github.com/pmandin/reevengi-tools/wiki/.RDT-%28Resident-Evil-2%29
//
using OpenCover.Framework.Model;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using JetBrains.Annotations;
using static UnityEditor.PlayerSettings;
using System.Collections.Generic;

//import hex    from '../boot/hex.js'
//import Script from './script.js'
//import Tim    from './tim.js'
//import File   from './file.js'
//import Tool   from './tool.js'
//import Node   from '../boot/node.js'
//import Coll   from './collision.js'
//import Sound  from './sound.js'
//import MD2    from './model2.js'
//import Bgm    from './bgm.js'
//const matrix = Node.load('boot/gl-matrix.js');
//const {vec3, mat4, vec2} = matrix;

//
// 不同的语言使用对应的目录
// RDP/RDS/RDF/RDT
//
//export default {
//  load,
//  from,
//};

public class rdt
{
//
// 按照编号读取
//
    public obj from(int stage, int room, string player)
    {
        string r = tool.toString(room, 16);
        if (room < 0x10) r = "0" + r;

        string file = "Pl" + player + "/Rdt/ROOM" + tool.toString(stage, 17) + r + player + ".RDT"; //.join('');

        return load(file);
    }

    //
    // 直接从文件读取
    //
    public obj load(string file)
    {
        //debug("Load RDT, room dest file", file);
        if (!file.ToLower().Contains(".rdt")) //!file.toLowerCase().endsWith('.rdt')
        {
            throw new Error("Not RDT file " + file);
        }
            
        byte[] filebuf = File2.open(file).buffer;
        int state = (int)(file.Substring(file.Length - 8, 1).Length); // , 16)
        int room = (int)(file.Substring(file.Length - 7, 2).Length); //, 16)
        obj obj = new obj() { state = state, room = room };

        int[] Head = new int[8];//Uint8Array(filebuf, 0, 8);
        int camera_count = Head[1];
        int num_obj10 = Head[2];
        //debug(Head, 'obj7', camera_count, 'obj10', num_obj10);

        off off = readOffset(filebuf);
        readCameraPos(filebuf, off, obj, camera_count);
        readCameraSwitch(filebuf, off, obj);
        readLight(filebuf, off, obj);
        readSound(filebuf, off, obj);
        readSpritesAnim(filebuf, off, obj);
        readSpritesTim(filebuf, off, obj);
        readSpace(filebuf, off, obj);

        if (off.offset22 == 0)
        {
            obj.extern_anim = readExAnim(filebuf, off.offset22);
        }
        if (num_obj10 == 0)
        {
            readTim(filebuf, off, obj);
        }
        if (off.init_script > 0)
        {
            //obj.init_script = readScript(filebuf, off.init_script);
        }
        if (off.room_script > 0)
        {
            //obj.room_script = readScript(filebuf, off.room_script);
        }
        return obj;
    }


    public MD readExAnim(byte[] buf, int off)
    {
        //debug("Ex Anim", tool.h4(off));
        dataViewExt v = new dataViewExt(new DataView(buf, off));
        int len = v.Ulong();
        int uk0 = v.Ulong();
        int uk1 = v.Ulong();
        int anim_off = v.Ulong(len + 4);
        // TODO: 有几个字节意义不明
        //debug(" . len", len, uk0, uk1, tool.h4(anim_off));
        return Model2.rbj(v, 0xC, anim_off);
    }


    public void readSpace(byte[] buf, off offobj, obj obj)
    {
        int block = offobj.offset12;
        int floor = offobj.offset11;
        int collision = offobj.offset6;
        //var("Move Space", { block, floor, collision});
        DataView v = new DataView(buf, 0);
        var off = 0;

        var bcount = v.getUint32(block, true);
        List<block> barr = new List<block>(obj.block);//; //[]
        int ff = block + 4;
        for (var i = 0; i < bcount; ++i)
        {
            block b = new block(); // new 
            b.x1 = v.getInt16(off, true);
            b.y1 = v.getInt16(off + 2, true);
            b.x3 = v.getInt16(off + 4, true);
            b.y3 = v.getInt16(off + 6, true);
            b.dir = v.getUint16(off + 8, true);
            b.abut = v.getUint16(off + 10, true);
            b.x2 = b.x3;
            b.y2 = b.y1;
            b.x4 = b.x1;
            b.y4 = b.y3;

            if (b.dir > 0)
            {
                // let out = [];
                // let center = [0,0];
                // let angle = b.dir/0xFFFF * Math.PI * 2;
                // vec2.rotate(out, [b.x1, b.y1], center, angle);
                // b.x1 = out[0]; b.y1 = out[1];
                // vec2.rotate(out, [b.x3, b.y3], center, angle);
                // b.x3 = out[0]; b.y3 = out[1];
                // vec2.rotate(out, [b.x4, b.y4], center, angle);
                // b.x4 = out[0]; b.y4 = out[1];
            }

            off += 12;
            barr.Add(b);
            //debug('. Block', i, b);
        }

        int fcount = (int)v.getUint16(floor, true);
        List<floor> farr = new List<floor>(floor); // []
        off = obj.floor + 2;

        for (int i = 0; i < fcount; ++i)
        {
            floor f = new floor(); // new
            f.x = v.getInt16(off, true);
            f.y = v.getInt16(off + 2, true);
            f.w = v.getUint16(off + 4, true);
            f.d = v.getUint16(off + 6, true);
            f.se_no = v.getUint16(off + 8, true); //地面音效编号?
            f.height = v.getUint16(off + 10, true);
            farr.Add(f);
            off += 12;
            //debug(". Floor", i, f);
        }

        int cx = v.getInt16(collision, true);
        int cz = v.getInt16(collision + 2, true);
        int d0 = (int)v.getUint32(collision + 4, true) - 1;
        int d1 = (int)v.getUint32(collision + 8, true);
        int d2 = (int)v.getUint32(collision + 12, true);
        //debug(". Collision", cx, cz, d0, d1.toString(16), d2.toString(16));

        List<collision> carr = new List<collision>(obj.collision); //[]
        var GAP = 300; // 大一圈的算法有问题
        off = collision + 16;
        for (var i = 0; i < d0; ++i)
        {
            collision c = new collision(); // new C
            // 障碍加大一圈
            c.x = v.getInt16(off, true) - GAP;
            c.y = v.getInt16(off + 2, true) - GAP;
            c.w = v.getUint16(off + 4, true) + GAP + GAP;
            c.d = v.getUint16(off + 6, true) + GAP + GAP;

            var flag = v.getUint16(off + 8, true);
            c.shape = flag & 0x000F;
            c.weapon_on = (flag >> 4) & 0x08;
            c.floor_on = (flag >> 8) & 1;
            c.enemy_on = (flag >> 10) & 1;
            c.bullet_on = (flag >> 13) & 1;
            c.obj_on = (flag >> 14) & 1;
            c.play_on = (flag >> 15) & 1;

            int type = v.getUint16(off + 10, true);
            // 宽度乘数X / W（0-3）
            c.xw = type & 3;
            // 深度倍增器Z / D（0-3）
            c.yd = (type >> 2) & 3;
            // type: 楼梯/斜坡/平台访问
            // 00爬升/上升/从Z轴向下跳跃
            // 01从Z轴上升/上升/跳起
            // 02爬升/上升/从X轴向下跳跃
            // 03爬升/上升/从X轴上跳
            c.type = (type >> 4) & 3;
            // 高度乘数（0-9）
            // 此值确定阴影和
            // 子弹偏转高度
            c.floor = (type >> 6) & 0x3F;
            // c.nine   = (type >> 12) & 0xF;
            //Coll.installCollision(c);
            carr.Add(c);

            int lev_flag0 = v.getUint8(off + 12);
            int lev_flag1 = v.getUint8(off + 13);
            int lev_flag2 = v.getUint8(off + 14);
            int lev_flag3 = v.getUint8(off + 15);
            c.floor_block = makeFloorBlock(lev_flag3, lev_flag2, lev_flag1, lev_flag0);

            off += 16;
            if (((type >> 12) & 0xF) != 9)
            {
                //console.error("flag fail 0x09 !=", c);
            }
            int f = (flag & 0xff00) >> 8;
            //debug('\t', c.name, 'x,y=', c.x, c.y,
            //  'w,d=', c.w, c.d, 'x/w=', c.xw, 'z/d=', c.yd,
            //  'type=', c.type, 'floor=', c.floor,
            //  /*'f=', f.toString(2), '?=', type & 0x1F,*/
            //  '\n', c.floor_block);
        }
    }

    // 返回数组元素的位置对应楼层数, 1表示阻碍, 0表示通过
    public int[] makeFloorBlock(int f3, int f2, int f1, int f0)
    {
        int[] b = new int[32];
        for (int i = 0; i < 8; ++i)
        {
            b[i] = (f0 & (1 << i)) >> i;
            b[i + 8] = (f1 & (1 << i)) >> i;
            b[i + 16] = (f2 & (1 << i)) >> i;
            b[i + 24] = (f3 & (1 << i)) >> i;
        }
        return b;
    }

    public void readScript(byte[] filebuf, int off)
    {
        // debug("read script from", off);
        var script_buf = new DataView(filebuf, off);
        // hex.printHex(new Uint8Array(filebuf, off, 0x20));
        //return Script.compile(script_buf);
    }


    public void readSound(byte[] filebuf, off off, obj ret)
    {
        var v = new dataViewExt(new DataView(filebuf));
        //debug("VAB Edt0/SND?", off.offset0);
        //debug("VAB Vh0", off.vab);
        //debug("VAB Vb0", off.offset2);
        //debug("VAB Vh1", off.offset3);
        //debug("VAB Vb1", off.offset4);

        vab vab = bgm.parse_vab_header(v, off.vab, off.offset2);
        //debug('RAW count', vab.raw.length);
        //hex.printHex(new Uint8Array(filebuf, off.offset0, 100));
        ret.vab = vab;
    }


    public void readTim(byte[] filebuf, off off, obj obj)
    {
        //debug("Obj10 TIM", new Uint8Array(filebuf, off, 333));
    }

    public void readSpritesTim(byte[] buf, off off, obj obj)
    {
        int count = obj.sprites_anim.Length;
        Texture2D[] tim = obj.sprites_tim; // []
        int file_offset = off.list_tim;
        //debug("Sprites Tim count", count);

        for (int i = 0; i < count; ++i)
        {
            //debug("Read TIM on", file_offset);
            DataView v = new DataView(buf, file_offset);
            tim[i] = Tim.parseStream(v);
            file_offset += tim[i].width * tim[i].height; //byteLength
        }
    }


    public void readSpritesAnim(byte[] filebuf, off off, obj obj)
    {
        //debug("Sprites Anim:", tool.h4(off.sprites_anim));
        // hex.printHex(new Uint8Array(filebuf, off.sprites_anim, 1500));

        var v = new DataView(filebuf, off.sprites_anim);
        var p = new DataView(filebuf);
        List<AnimHead> header = new List<AnimHead>(obj.sprites_anim.Length); //[]
        // hex.printHex(p);

        for (int i = 0; i < 8; ++i)
        {
        int c = v.getUint8(i);
            if (c == 0xFF || c == 0) break;
            int point = (int)p.getUint32(off.offset19 - i * 4, true);
            //debug("Sprites Anim Block", i, c, point);
            parseSp(i, c, point);
        }

        //
        // obj.sprites_anim: { frames, sprites, height, width, 
        //                     num_frames, num_sprites }
        //
        void parseSp(int id, int cc, int begin_at)
        {
            int vi = begin_at;
            AnimHead h = new AnimHead();
            h.num_frames = v.getUint16(vi, true);
            h.num_sprites = v.getUint16(vi + 2, true);
            h.height = v.getUint8(vi + 4);
            h.width = v.getUint8(vi + 5);
            h.unknow = v.getUint16(vi + 6, true);
            h.frames = new List<frame>();
            h.sprites = new List<sprite>();

            vi += 8;
            header.Add(h);
            //debug("Anim Head:", id, J(h));

            for (int f = 0; f < h.num_frames; ++f)
            {
                frame frm = new frame();
                frm.sp_idx = v.getUint8(vi + 0);
                frm.x= v.getUint8(vi + 1);
                frm.y= v.getUint8(vi + 2);
                frm.z= v.getUint8(vi + 3);
                frm.un1 = v.getUint8(vi+ 1);
                frm.un2 = v.getUint8(vi+ 2);
                frm.un3 = v.getUint8(vi+ 3);
                frm.un4 = v.getUint8(vi+ 4);
                frm.un5 = v.getUint8(vi+ 5);
                frm.un6 = v.getUint8(vi+ 6);
                frm.un7 = v.getUint8(vi+ 7);

                vi += 8;
                h.frames.Add(frm);
                //debug("   Frames:", J(frm));
            }

            for (int s = 0; s < h.num_sprites; ++s)
            {
                // x和y是TIM图像中精灵的左上角位置。偏移x，y是精灵中心的有符号值
                // 用于在动画缓冲区中正确偏移图像部分。
                sprite sp = new sprite();
                sp.x = v.getUint8(vi + 0);
                sp.y = v.getUint8(vi + 1);
                sp.offset_x = v.getInt8(vi + 2);
                sp.offset_y = v.getInt8(vi + 3);

                vi += 4;
                h.sprites.Add(sp);
                //debug("  Sprites:", J(sp));
            }

            // let unknow_offsets = [];
            // let max_offset = 0;
            // for (let o=0; o<8; ++o) {
            //   var _off = v.getUint16(vi+o*2, true);
            //   unknow_offsets.push(_off);
            //   max_offset = _off > max_offset ? _off : max_offset;
            // }
            // // vi += 16;
            // let blk_len = unknow_offsets[1] 
            //             ? (unknow_offsets[1] - unknow_offsets[0]) * 4
            //             : 0x38;
            // debug(" Unknow offset:", unknow_offsets, max_offset, blk_len);

            // for (let un=0; un<unknow_offsets.length; ++un) {
            //   if (unknow_offsets[un] == 0) break;
            //   let x = vi + (unknow_offsets[un]) * 4;
            //   let unknow_block = [];
            //   for (let b = 0; b < blk_len; b+=1) {
            //     unknow_block.push(v.getInt8(x + b, true));
            //   }
            //   debug("  Unknow block:", unknow_block);
            // }
            // vi += (max_offset) * 4 + blk_len;

            // var total_long = v.getInt32(vi, true);
            // debug("  total:", total_long, begin_at, vi);
            // if (total_long + begin_at != vi) {
            //   console.warn('bad total count or offset !!');
            //   // throw new Error("bad offset");
            // }
        }
    }


    public void readCameraPos(byte[] filebuf, off off, obj ret, int count)
    {
        int len = 4 + 6 * 4 + 4;
        List<cameras> cameras = new List<cameras>(); //ret.cameras
        DataView v = new DataView(filebuf, off.cam_pos, len * count);
        cameras.Capacity = count;
        //debug("camera count", count, '(' + off.cam_pos + ')');

        for (int i = 0; i < count; ++i)
        {
            int vi = len * i;
            v = new DataView(filebuf, off.cam_pos + len*i, len);
            cameras c = new cameras();
            c.unk0   = v.getUint16(0 + vi, true);
            // c.const0 : v.getUint16(2 +vi, true);
            c.fov= v.getUint8(2) & 0x7F;
            c.unk1 = v.getUint8(3);
            c.from_x= v.getInt32(4 + vi, true);
            c.from_y= v.getInt32(8 + vi, true);
            c.from_z = v.getInt32(12 + vi, true);
            c.to_x = v.getInt32(16 + vi, true);
            c.to_y = v.getInt32(20 + vi, true);
            c.to_z = v.getInt32(24 + vi, true);
            int mask_off = (int)v.getUint32(28 + vi, true);
            // debug("camera", i, J(c));
            if (mask_off != 0xffffffff)
            {
                c.mask = readMask(filebuf, mask_off);
            }
            cameras[i] = c;
        }

        ret.cameras = cameras;
    }

    public List<mask> readMask(byte[] filebuf, int off)
    {
        DataView v = new DataView(filebuf, off);
        int c_offset = v.getUint16(0, true);
        int c_masks  = v.getUint16(2, true);
        //debug("Mask OFFset", c_offset, c_masks, off);
        if (c_offset == 0xFFFF || c_masks == 0xFFFF)
        {
            return null;
        }

        List<mask> offset_obj = new List<mask>();
        int to_count = 0;
        off = 4;

        for (int i = 0; i < c_offset; ++i)
        {
            mask mask = new mask();
            int ct = mask.count = v.getUint16(off, true);
            mask.unknow = v.getUint16(off + 2, true);
            // 要添加的背景图像/屏幕上的目的地位置块偏移
            mask.x = v.getInt16(off + 4, true);
            mask.y = v.getInt16(off + 6, true);
            // debug(off-8, ct, mask.x, mask.y);
            to_count += ct;
            off += 8;
            // hex.printHex(new Uint8Array(filebuf, mask.unknow, 1000));
            offset_obj.Add(mask);
        }

        if (to_count > c_masks)
        {
            throw new Error("bad mask count");
        }

        List<chip> ret = new List<chip>(); //[]
        for (int i = 0; i < c_offset; ++i)
        {
            mask mask = offset_obj[i];
            // debug('Mask info', mask.count, J(mask));

            for (int j = 0; j < mask.count; ++j)
            {
                chip chip = new chip();
                ret.Add(chip); //push

                // 蒙版图像存储于房间图片的下方
                chip.src_x = v.getUint8(off + 0);
                chip.src_y = v.getUint8(off + 1);
                // 背景图像/屏幕上的目的地位置
                chip.dst_x = v.getUint8(off + 2) + mask.x;
                chip.dst_y = v.getUint8(off + 3) + mask.y;
                // “深度”值是掩模与相机的Z距离（低值=近，高值=远）。
                chip.depth = v.getUint16(off + 4, true);
                int w = v.getUint16(off + 6, true);

                if (w == 0)
                {
                    chip.w = v.getUint16(off + 8, true);
                    chip.h = v.getUint16(off + 10, true);
                    off += 12;
                }
                else
                {
                    chip.w = w;
                    chip.h = w;
                    off += 8;
                }
                if (chip.dst_x + chip.w > 320 || chip.dst_y + chip.h > 240)
                {
                    //console.error("bad size")
                }
                // debug("Mask chip", J(chip));
            }
        }
        return offset_obj;
    }


    public void readLight(byte[] filebuf, off off, obj ret)
    {
        List<cameras> cameras = ret.cameras;
        int len = 2 * 2 + 3 * 3 + 3 + 3 * 6 + 3 * 2;
        DataView v;
        cameras c;

        Color rcolor(int i)
        {
            Color color = new Color();
            color.r = v.getUint8(i);
            color.g = v.getUint8(i + 1);
            color.b = v.getUint8(i + 2);
            return color;
        }

        Vector3 rpos(int i)
        {
            Vector3 pos = new Vector3();
            pos.x = v.getInt16(i, true);
            pos.y = v.getInt16(i + 2, true);
            pos.z = v.getInt16(i + 4, true);
            return pos;
        }

        for (int i = 0; i < cameras.Count; ++i)
        {
            v = new DataView(filebuf, off.lights + len * i, len);
            c = cameras[i];
            c.light1 = new light();
            c.light1.type = v.getUint16(0, true);
            c.light1.color = rcolor(4);
            c.light1.pos = rpos(16);
            c.light1.bright = v.getUint16(34, true);
            c.light2 = new light();
            c.light2.type = v.getUint16(2, true);
            c.light2.color = rcolor(7);
            c.light2.pos = rpos(22);
            c.light2.bright = v.getUint16(36, true);
            c.light0 = new light();
            c.light0.type = 0;
            c.light0.color = rcolor(10);
            c.light0.pos = rpos(28);
            c.light0.bright = v.getUint16(38, true);
            c.env_color = rcolor(13);

            //debug("light", i, c.env_color);
            /*, new Uint8Array(filebuf, off.lights + len*i, len)*/
            // debug('  0:', c.light0);
            // debug('  1:', c.light1);
            // debug('  2:', c.light2);
        }
    }

    public void readCameraSwitch(byte[] filebuf, off off, obj ret)
    {
        int len = 4 + 8 * 2;
        List<CameraSwitch> cameras = new List<CameraSwitch>(ret.cameras_sw); //[]

        for (int i = 0; i < 100; ++i)
        {
            int beg = off.cam_sw + len * i;
            DataView v = new DataView(filebuf, beg, len);
            if (v.getUint32(0) == 0xFFFFFFFF)
            {
                break;
            }
            // TODO: floor 它大部分时间是FF或十进制的255。 在地板上它将起作用的高度意味着。 
            // 将其设置为255意味着它将适用于房间中的所有高度
            // 将它设置到特定的楼层意味着它只能在那里工作。
            CameraSwitch cam = new CameraSwitch();
            cam.flag = v.getUint8(0);
            cam.floor= v.getUint8(1);
            cam.cam0 = v.getUint8(2);
            cam.cam1 = v.getUint8(3);
            cam.x1 = v.getInt16(4, true);  cam.y1 = v.getInt16(6, true);
            cam.x2 = v.getInt16(8, true);  cam.y2 = v.getInt16(10, true);
            cam.x3 = v.getInt16(12, true); cam.y3 = v.getInt16(14, true);
            cam.x4 = v.getInt16(16, true); cam.y4 = v.getInt16(18, true);
            cameras.Add(cam);
            // debug("Camera Switch", i, cam);
        }
    }


    public int J(int o, int x, int n)
    {
        // 注释 debug 的同时, 注释这里
        // return JSON.stringify(o, x, n);
        return o;
    }


    public void debug()
    {
        //tool.debug.apply(null, arguments);
    }


    public void showOffsetBuf(dataViewExt filebuf, int[] off)
    {
      //for (var n in off)
      //  {
      //      //debug("================== BUFFER", n, '0x' + off[n].toString(16), "==================");
      //      try
      //      {
      //          hex.printHex(new Uint8Array(filebuf, off[n], 500));
      //      }
      //      catch (e)
      //      {
      //          debug(e.message);
      //      }
      //  }
    }

    public off readOffset(byte[] filebuf)
    {
        int len = 4 * 23;
        DataView Offset = new DataView(filebuf, 8, len);
        off off = new off();
        off.offset0 = (int)Offset.getUint32(0 * 4, true); // Soundbank?
        off.vab = (int)Offset.getUint32(1 * 4, true); // Soundbank
        off.offset2 = (int)Offset.getUint32(2 * 4, true); // Soundbank?
        off.offset3 = (int)Offset.getUint32(3 * 4, true); // Soundbank?
        off.offset4 = (int)Offset.getUint32(4 * 4, true);
        off.offset5 = (int)Offset.getUint32(5 * 4, true);
        off.offset6 = (int)Offset.getUint32(6 * 4, true); // collision
        off.cam_pos = (int)Offset.getUint32(7 * 4, true);
        off.cam_sw = (int)Offset.getUint32(8 * 4, true);
        off.lights = (int)Offset.getUint32(9 * 4, true);
        off.tim = (int)Offset.getUint32(10 * 4, true); // 房间3d模型, 最多17个
        off.offset11 = (int)Offset.getUint32(11 * 4, true); // Floor
        off.offset12 = (int)Offset.getUint32(12 * 4, true); // Block
        off.lang1 = (int)Offset.getUint32(13 * 4, true);
        off.lang2 = (int)Offset.getUint32(14 * 4, true);
        off.offset15 = (int)Offset.getUint32(15 * 4, true); // Scroll Texture?
        off.init_script = (int)Offset.getUint32(16 * 4, true);
        off.room_script = (int)Offset.getUint32(17 * 4, true);
        off.sprites_anim = (int)Offset.getUint32(18 * 4, true);
        off.offset19 = (int)Offset.getUint32(19 * 4, true); // sprites_anim offset
        off.list_tim = (int)Offset.getUint32(20 * 4, true);
        off.another_tim = (int)Offset.getUint32(21 * 4, true); // 房间3d模型纹理tim
        off.offset22 = (int)Offset.getUint32(22 * 4, true); // Player Animation?

        //debug(J(off, 0, 2));
        // showOffsetBuf(filebuf, off);
        return off;
    }
}