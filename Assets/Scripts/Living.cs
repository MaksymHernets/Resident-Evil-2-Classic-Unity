using UnityEngine;

public class Living
{
    public int[] components;
    public AngleLinearFrame alf;
    public Vector3 liner_pos;     //= { x:0, y:0, z:0 };
    public Vector3 abs_pos;       //= { x:0, y:0, z:0 };
    public Vector3 liner_pos_tr;  //= Game.Pos3TransitionLine(liner_pos, 0);
    // [movestep, ?, ?, flag]
    public Vector4 move_info;     //= [0, 0, 0, 0];
    public const float DEF_SPEED     = 1;
    public const int FIRST_FRM_TIM = 3;

    // 动画索引
    public int anim_idx = 0;
    // 动画帧数
    public int anim_frame = 0;
    public int anim_dir = 0;
    public int frame_data;
    public int a = 0;
    public int speed = 1; //DEF_SPEED
    public int pose;
    public int whenAnimEnd = 1;
    public int animCallBack;
    public int animSound;
    // 当前动画帧停留时间 ms
    public int frame_time = 0;
    public int prevStep = 0;
    public int prevFrameTime = 0;

    public MD md;
    public Texture2D tex;

    public Living()
    {

    }

    public Living(MD md, Texture2D texture)
    {
        md = md;
        tex = texture;
    }

    public void free()
    {
        //for (let i = 0; i < components.length; ++i)
        //{
        //    components[i].free();
        //}
        //components.length = 0;
    }
}
