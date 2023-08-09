using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBone
{
    public sk dat;
    public int parent;
    public object idx;
    public int[] child;
    public Vector3 _pos;
    // 骨头可以组合一个绘制对象
    public object _combination = null;
    // 最后一次骨骼状态变换矩阵的值
    public Vector4 lastTrans;

    public SkeletonBone(sk dat, int i)
    {
        //this.dat = dat;
        //this.parent = null;
        //this.idx = i;
        //this.child = [];
        //this._pos = [dat.x, dat.y, dat.z];
        //// 骨头可以组合一个绘制对象
        //this._combination = null;
        //// 最后一次骨骼状态变换矩阵的值
        //this.lastTrans = mat4.create();
    }


    // 
    // 自动释放之前关联的对象
    //
    public void combination(int c)
    {
        //if (this._combination)
        //{
        //    this._combination.free();
        //}
        //this._combination = c;
    }


    public string toString()
    {
        //return JSON.stringify(this.dat);
        return "good";
    }


    //
    // 把骨骼数据(旋转偏移) 传送到着色器
    //
    public void transform2(int[] bind_bone, Vector4 alf, int[] sprites, int count)
    {
        // 骨骼索引和3d面组索引对应
        //alf.index(this.idx);

        int boffset = count << 3;
        bind_bone[0 + boffset] = this.dat.x;
        bind_bone[1 + boffset] = this.dat.y;
        bind_bone[2 + boffset] = this.dat.z;
        bind_bone[3 + boffset] = 1;
        bind_bone[4 + boffset] = (int)alf.x;
        bind_bone[5 + boffset] = (int)alf.y;
        bind_bone[6 + boffset] = (int)alf.z;
        bind_bone[7 + boffset] = (int)alf.w;

        //Shader.bindBoneOffset(bind_bone, ++count);
        //sprites[this.idx].draw();
        //if (this._combination)
        //{
        //    this._combination.draw();
        //}

        //for (var i = 0, len = this.child.Length; i < len; ++i)
        //{
        //    this.child[i].transform2(bind_bone, alf, sprites, count);
        //}
    }


    //
    // 把骨骼变换矩阵传送到着色器(测试)
    //
    public void transform1(Vector4 alf, int[]  sprites, int parent_convert)
    {
        //Vector4 modmat = new Vector4();// mat4.create();
        //var qu = alf.index(this.idx);
        //mat4.fromRotationTranslation(modmat, qu, this._pos);

        //if (parent_convert)
        //{
        //    mat4.multiply(modmat, parent_convert, modmat);
        //}

        //Shader.setBoneConvert(modmat);
        //sprites[this.idx].draw();
        //mat4.copy(this.lastTrans, modmat);
        //if (this._combination)
        //{
        //    this._combination.draw();
        //}

        //for (int i = 0, len = this.child.Length; i < len; ++i)
        //{
        //    this.child[i].transform1(alf, sprites, modmat);
        //}
    }
}
