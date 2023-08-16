using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tests : MonoBehaviour
{
    [MenuItem("Resident Evil/Test/Import Model")]
    public static void ImportModelTest()
    {
        Living living1 = Liv.fromEmd(0, 0x4B);

        SaveData.CheckFolderEMD(living1.playId, living1.emdId);
        string path = "Assets/EMD" + living1.playId + "/EM" + "_" + living1.playId + living1.emdId;

        SaveData.SaveMD(living1.md, path, false);
    }

    [MenuItem("Resident Evil/Test/Import Model Skelet")]
    public static void ImportModelSkeletTest()
    {
        Living living1 = Liv.fromEmd(1, 0x1E);

        SaveData.CheckFolderEMD(living1.playId, living1.emdId);
        string path = "Assets/EMD" + living1.playId + "/EM" + "_" + living1.playId + living1.emdId;

        SaveData.SaveMD(living1.md, path, false);
    }

    [MenuItem("Resident Evil/Test/Import Texture")]
    public static void ImportTextureTest()
    {
        Living living1 = Liv.fromTim(1, 0x4B);

        SaveData.CheckFolderEMD(living1.playId, living1.emdId);
        string path = "Assets/EMD" + living1.playId + "/EM" + "_" + living1.playId + living1.emdId + "/" + living1.emdId;

        SaveData.SaveTexture2D(living1.tex, path);
    }

    [MenuItem("Resident Evil/Test/Import PLD")]
    public static void ImportPLSTest()
    {
        Living living0 = Liv.fromPld(0); // Leon

        string path = "Assets/PLD" + living0.playId + "/PL" + "_" + living0.playId + living0.emdId;

        SaveData.SaveMD(living0.md, path);
    }

    [MenuItem("Resident Evil/Test/Import Map")]
    public static void ImportMapTest()
    {
        int stage = 1;
        int room_nm = 0;
        int play_mode = 0; // 0 - Leon

        rdt rdt = new rdt();
        obj map_data = rdt.from(stage, room_nm, play_mode);
    }
}
