using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class Scenes : MonoBehaviour
{
    [MenuItem("Resident Evil/Import Models")]
    public static void ImportModels()
    {
        int startId = 30;
        int endId = 89;
        for (int i = startId; i <= endId; i++)
        {
            Liv.fromEmd(0, i);
            Liv.fromEmd(1, i);
        }
    }

    [MenuItem("Resident Evil/Import Model Test")]
    public static void ImportModelTest()
    {
        var liv = Liv.fromEmd(1, 0x1E);
    }

    [MenuItem("Resident Evil/Import Textures")]
    public static void ImportTextures()
    {
        int startId = 30;
        int endId = 89;
        for (int i = startId; i <= endId; i++)
        {
            Liv.fromTim(0, i);
            Liv.fromTim(1, i);
        }
    }

    [MenuItem("Resident Evil/Import Texture Test")]
    public static void ImportTextureTest()
    {
        Liv.fromTim(1, 0x1E);
    }

    //[MenuItem("Resident Evil/Import Models2")]
    public static void begin_level()
    {
        // 游戏参数初始化
        // 难度
        //set_bitarr(0, 0x19, EASY);
        // 角色类型
        //set_bitarr(1, 0x00, play_mode);
        // 二周目 0:A, 1:B
        //set_bitarr(1, 0x01, ab);
        // 游戏模式 0:Leon/Claire, 1:Hunk/Tofu, or vice versa
        //set_bitarr(1, 0x06, 0);

        // TODO: 加载玩家角色模型, pld 的部分动作来自 plw.

        int Leon = 0;
        int play_mode = Leon;
        var liv = Liv.fromEmd(1, 0x4B);

        // let liv = Liv.fromPld(play_mode);
        //p1 = Ai.player(liv, window, draw_order, gameState, camera);
        //set_weapon(p1, liv, 7);
        //window.add(Tool.EnemyCollision(p1, enemy));

        // 玩家初始位置
        //init_pos(0);
        //liv.moveImmediately();

        //while (window.notClosed())
        //{
        //    p1.able_to_control(false);
        //    goto_next_door = false;
        //    load_map();
        //    load_bgm();
        //    switch_camera();
        //    script_context = map_data.room_script.createContext(gameState, 0);
        //    p1.able_to_control(true);
        //    // vm.gc();

        //    // TODO: 这里是补丁, 房间脚本应该不会退出?
        //    while (!goto_next_door && window.notClosed())
        //    {
        //        window.nextFrame();
        //        for (let i = 0; i < 100; ++i)
        //        {
        //            // 单步执行
        //            // while (!window.input().isKeyPress(gl.GLFW_KEY_Z)) {
        //            //   window.nextFrame();
        //            //   console.line("press z to continue.");
        //            // }
        //            if (script_context.frame(window.usedTime()) == 1)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //}
    }

    private void Start()
    {
        begin_level();
    }
}
