﻿using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Linq;
//using System.Numerics;

//import node from '../boot/node.js'
//const matrix = node.load('boot/gl-matrix.js');
//const {vec3, mat4} = matrix;

public enum person {
  body = 0,
  waist = 1,
  lThigh = 2,
  lCalf = 3,
  lFoot = 4,
  rThigh = 5,
  rCalf = 6,
  rFoot = 7,
  head = 8,
  lBigarm = 9,
  lForearm = 10,
  lHand = 11,
  rBigarm = 12,
  rForearm = 13,
  rHand = 14
}

public class BoneNm
{
    //const _mapp = {
    //  // Leon module
    //  50: person,
    //}
    public static Dictionary<string, List<string>> _mapp;


    public BoneNm()
    {
        var map = Enum.GetNames(typeof(person));
        _mapp.Add(50.ToString(), map.ToList());
    }

    public static int fixMoveStep()
    {
        return 30;
    }

    public static Vector3 _getBonePos(Vector3 initPos, Matrix4x4 Utrans)
    {
        System.Numerics.Vector3 v = new System.Numerics.Vector3(); // var v = vec3.create();
        System.Numerics.Matrix4x4 trans = new System.Numerics.Matrix4x4();
        trans.M11 = Utrans.m00; trans.M21 = Utrans.m10; trans.M31 = Utrans.m20; trans.M41 = Utrans.m30;
        trans.M12 = Utrans.m01; trans.M22 = Utrans.m11; trans.M32 = Utrans.m21; trans.M42 = Utrans.m31;
        trans.M13 = Utrans.m02; trans.M21 = Utrans.m12; trans.M31 = Utrans.m22; trans.M41 = Utrans.m32;
        trans.M14 = Utrans.m03; trans.M22 = Utrans.m13; trans.M32 = Utrans.m23; trans.M42 = Utrans.m33;
        System.Numerics.Vector3.Transform(v, trans); //vec3.transformMat4(v, v, trans);
        Vector3 Uv = new Vector3(v.X, v.Y, v.Z);
        return Uv;
    }

    public static float calculateFootstepLength(Dictionary<string, SkeletonBone> bone)
    {
        Vector3 l = _getBonePos(bone["lFoot"]._pos, bone["lFoot"].lastTrans);
        Vector3 r = _getBonePos(bone["rFoot"]._pos, bone["rFoot"].lastTrans);
        //return new Vector3(l.x - r.x, l.y - r.y, l.z - r.z); // l[0] - r[0]
        return Vector3.Distance(l, r);
        //Vector3 res = l;
        // vec3.subtract(res, l, r);
        // return vec3.length(res);
    }

    public static void bind(string name, MD md)
    {
        var boneIdx = md.boneIdx; //{ stepLength: fixMoveStep,};// 返回模型两脚间的距离

        Regex reg = new Regex(@"/.\/em(.)(..)\.emd/i");
        name = name.Replace("sourcegame/", "");
        MatchCollection match = reg.Matches(name); //reg.exec(name);
        if (match.Count == 0)
        {
            console.warn("invaild module file name", name);
            return;
        }

        var map = _mapp[match[2].Name];
        if (map != null)
        {
            // TODO: 默认绑定 person !!!
            console.warn("not found Module Bind", match[2].Name, "use person.");
            //map = person;
        }

        if (md.bone.Count == 0)
        {
            console.warn("Module not bone");
            return;
        }

        int index = 0;
        foreach (string namee in map)
        {
            boneIdx.Add(namee, new SkeletonBone(new sk(), index));
             ++index;
        }
    }
}