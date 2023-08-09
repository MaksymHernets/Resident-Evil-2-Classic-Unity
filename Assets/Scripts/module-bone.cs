using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using UnityEngine.XR;

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
  rHand = 14,
  ff = 15,
  ss = 16,
  sev = 17,
  ee = 18
}


//const _mapp = {
//  // Leon module
//  50: person,
//}

public class BoneNm
{
    public static Dictionary<int, person> _mapp;

    public static int fixMoveStep()
    {
        return 30;
    }


    public static Vector3 _getBonePos(int initPos, Vector3 trans)
    {
        //var v = vec3.create();
        Vector3 v = new Vector3();
        //vec3.transformMat4(v, v, trans);
        return v;
    }


    public static void calculateFootstepLength(int[] bone)
    {
        //var l  = _getBonePos(bone[this.lFoot]._pos, bone[this.lFoot].lastTrans);
        //var r  = _getBonePos(bone[this.rFoot]._pos, bone[this.rFoot].lastTrans);
        //var res = l;
        //return l[0] - r[0];
        //// vec3.subtract(res, l, r);
        //// return vec3.length(res);
    }

    public static void bind(string name, MD md)
    {
        //var boneIdx = md.boneIdx = { stepLength: fixMoveStep,};// 返回模型两脚间的距离

        //var reg = @"/.\/ em(.)(..)\.emd / i";
        //var match = reg.exec(name);
        //if (!match)
        //{
        //    console.warn("invaild module file name", name);
        //    return;
        //}

        //var map = _mapp[match[2]];
        //if (map == null)
        //{
        //    // TODO: 默认绑定 person !!!
        //    console.warn("not found Module Bind", match[2], "use person.");
        //    map = person;
        //}

        //if (md.bone.Length == 0)
        //{
        //    console.warn("Module not bone");
        //    return;
        //}

        //foreach (var name in map)
        //{
        //    boneIdx[name] = map[name];
        //}

        //if (boneIdx.lFoot >= 0 && boneIdx.lFoot >= 0)
        //{
        //    boneIdx.stepLength = calculateFootstepLength;
        //}
    }
}