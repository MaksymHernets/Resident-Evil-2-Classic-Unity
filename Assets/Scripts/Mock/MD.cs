﻿using System.Collections.Generic;
using UnityEngine;

public class MD
{
    public Mesh[] meshs;

    public group[] pose;
        // 骨骼绑定状态
    public List<SkeletonBone> bone;
        // 用于传输数据到着色器, 最多 20 块骨头层级, 每个骨头 4个偏移4个旋转
    public float[] bind_bone;
        // 使用模型的高度作为与地平线零点的偏移
    public int height = 0;
        // 动作分组索引, 偶数是开始索引, 奇数是长度
    public Stack<int> pose_group;

    public MD()
    {
        this.pose = new group[100];
        this.bone = new List<SkeletonBone>(15);
        this.bind_bone = new float[20 * 8];
        this.height = 0;
        //this.pose_group = [];
    }

    //
    // anim_set 是个二维数组[动作索引][帧索引], 值是对骨骼状态的索引
    // get_frame_data(骨骼状态的索引) 可以返回该动作帧上的的全部骨骼数据.
    //
    public void addAnimSet(group[] anim_set, int get_frame_data)
    {
        //var pi = this.pose.Length;
        this.pose = new group[anim_set.Length];
        //this.pose_group.Push({ at: pi, len: anim_set.Length });
        for (var i = 0; i < anim_set.Length; ++i)
        {
            this.pose[i] = anim_set[i];
            //this.pose[pi + i].get_frame_data = get_frame_data;
        }
    }

    //
    // 返回动作第 frameId 帧上的骨骼数据(skdata)
    //
    public void getFrameData(int poseId, int frameId)
    {
        var frm = this.pose[poseId];
        if (frm == null) return;
        //var fdata = frm[frameId];
        //if (fdata == null) return;
        //return frm.get_frame_data(fdata.skidx);
    }

    public group getPose(int poseId)
    {
        return this.pose[poseId];
    }

    public int poseCount()
    {
        return this.pose.Length;
    }

    public void transformRoot(int alf, Sprite[] sprites, int count)
    {
        // this.bone[0].transform2(this.bind_bone, alf, sprites, count);
        var rootBone = this.bone[0];
        // const zeropos = rootBone._pos;
        // const m4 = mat4.create();
        // mat4.fromTranslation(m4, zeropos);
        // mat4.invert(m4, m4);
        //rootBone.transform1(alf, sprites, null);
    }

    // 从 beginIdx 开始覆盖, 默认在后面追加新动作
    public int setPoseFromMD(MD md, int beginIdx = -1, int fromIdx = 0, int copyCount = -1)
    {
        if (beginIdx < 0) beginIdx = this.pose.Length;
        if (copyCount < 0) copyCount = md.pose.Length;
        var posIdx = beginIdx;
        for (var i = fromIdx; i < copyCount; ++i)
        {
            this.pose[posIdx] = md.pose[i];
            ++posIdx;
        }
        md.height = this.getHeight();
        return beginIdx;
    }

    // 追加一个动作, 默认在末尾追加, 并返回 pose 索引
    public int addPose(group pose, int index = -1)
    {
        if (index < 0) index = this.pose.Length;
        this.pose[index] = pose;
        return index;
    }

    //public int getPose(int poseid)
    //{
    //    return this.pose[poseid];
    //}

    public void combinationDraw(int boneIdx, bool drawable)
    {
        //this.bone[boneIdx].combination = drawable;
    }

    public int getHeight()
    {
        // console.log(this.height)
        return this.height;
    }
}