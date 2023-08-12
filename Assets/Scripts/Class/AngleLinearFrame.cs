using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Playables;

public class AngleLinearFrame
{
    public Quaternion[] endVal;
    public Quaternion[] currVal;
    public Quaternion[] beginVal;
    public float time;
    public Quaternion qa;
    public Quaternion qb;
    public float x;//-
    public float y;//-
    public float z;//-
    public float w;


    public AngleLinearFrame()
    {
        // time = 1 旋转值
        //this.endVal = [];
        // 基于 time 插值得到的旋转
        //this.currVal = [];
        // time = 0 旋转值
        //this.beginVal = [];
        this.time = 1;
        this.qa = new Quaternion();
        this.qb = new Quaternion();
        this.w = 1;
    }

    public void setPercentage(int percentage)
    {
        // if (percentage < 0) percentage = 0;
        // else if (percentage > 1) percentage = 1;
        this.time = percentage;
    }

    public Quaternion getQuat(Quaternion[] arr, int index)
    {
        if (arr[index] == null)
        {
            arr[index] = new Quaternion(); //quat.create();
        }
        return arr[index];
    }

    public void setDestination(sk frame_data)
    {
        var len = frame_data.angle.Length;
        for (var i = 0 ; i < len; ++i)
        {
            if (this.endVal.Length < frame_data.angle.Length)
            {
                this.getQuat(this.endVal, i);
                this.getQuat(this.currVal, i);
                this.getQuat(this.beginVal, i);
            }
            //Quaternion.(this.endVal[i]); //identity

            //!这种方法结果错误, 动画奇葩
            // quat.fromEuler(a[i], c.x, c.y, c.z);

            var _end = this.endVal[i];
            var _tar = frame_data.angle[i];
            Quaternion.RotateTowards(_end, _end, _tar.x); // rotateX
            Quaternion.RotateTowards(_end, _end, _tar.y); // rotateX
            Quaternion.RotateTowards(_end, _end, _tar.z); // rotateX
        }
    }

    public void reset()
    {
        //this.zero(this.currVal);
    }

    // 当帧数据改变时调用
    public void update()
    {
        //this.copy(this.currVal, this.beginVal);
    }

    public void zero(int[,] arr)
    {
        for (var i = arr.Length - 1; i >= 0; --i)
        {
            arr[i,0] = 0;
            arr[i,1] = 0;
            arr[i,2] = 0;
            arr[i,3] = 0;
        }
    }

    public void copy(int[,] src, int[,] dsc)
    {
        for (var i = src.Length - 1; i >= 0; --i)
        {
            dsc[i,0] = src[i,0];
            dsc[i,1] = src[i,1];
            dsc[i,2] = src[i,2];
            dsc[i,3] = src[i,3];
        }
    }

    // 获取 x,y,z 之前先调用
    public Quaternion index(int boneIdx)
    {
        var _curr = this.currVal[boneIdx];
        _curr = Quaternion.Slerp(this.beginVal[boneIdx], this.endVal[boneIdx], this.time);

        this.x = _curr[0];
        this.y = _curr[1];
        this.z = _curr[2];
        this.w = _curr[3];
        return _curr;

        // 欧拉角(测试用)
        // this.x = this.curr[boneIdx].x * Math.PI / 180;
        // this.y = this.curr[boneIdx].y * Math.PI / 180;
        // this.z = this.curr[boneIdx].z * Math.PI / 180;
        // this.w = 0;
    }
}

