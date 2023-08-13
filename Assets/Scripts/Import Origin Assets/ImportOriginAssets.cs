using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportOriginAssets : MonoBehaviour
{
    [MenuItem("Resident Evil/Import Models")]
    public static void ImportModels()
    {
        int startId = 30;
        int endId = 89;
        for (int i = startId; i <= endId; i++)
        {
            Living living0 = Liv.fromEmd(0, i);
            SaveData.CheckFolderPLS(living0.playId, living0.emdId);
            string path = "Assets/EMD" + living0.playId + "/EM" + "_" + living0.playId + living0.emdId;
            if (living0 == null) Debug.LogWarning(path);
            else SaveData.SaveMD(living0.md, path);

            Living living1 = Liv.fromEmd(1, i);
            SaveData.CheckFolderPLS(living1.playId, living1.emdId);
            path = "Assets/EMD" + living1.playId + "/EM" + "_" + living1.playId + living1.emdId;
            if (living0 == null) Debug.LogWarning(path);
            else SaveData.SaveMD(living1.md, path);
        }
    }

    [MenuItem("Resident Evil/Import Model Test")]
    public static void ImportModelTest()
    {
        Living living1 = Liv.fromEmd(1, 0x4B);

        SaveData.CheckFolderPLS(living1.playId, living1.emdId);
        string path = "Assets/EMD" + living1.playId + "/EM" + "_" + living1.playId + living1.emdId;

        SaveData.SaveMD(living1.md, path, false);
    }

    [MenuItem("Resident Evil/Import Model Skeler Test")]
    public static void ImportModelSkeletTest()
    {
        Living living1 = Liv.fromEmd(1, 0x1E);

        SaveData.CheckFolderPLS(living1.playId, living1.emdId);
        string path = "Assets/EMD" + living1.playId + "/EM" + "_" + living1.playId + living1.emdId;

        SaveData.SaveMD(living1.md, path, false);
    }

    [MenuItem("Resident Evil/Import Textures")]
    public static void ImportTextures()
    {
        int startId = 30;
        int endId = 89;
        string path;
        for (int i = startId; i <= endId; i++)
        {
            Living living0 = Liv.fromTim(0, i);
            path = "Assets/EMD" + living0.playId + "/EM" + "_" + living0.playId + living0.emdId + "/" + living0.emdId;

            if (living0.tex == null) Debug.LogWarning(path);
            else SaveData.SaveTexture2D(living0.tex, path);

            Living living1 = Liv.fromTim(1, i);
            path = "Assets/EMD" + living1.playId + "/EM" + "_" + living1.playId + living1.emdId + "/" + living1.emdId;
           
            if (living1.tex == null) Debug.LogWarning(path);
            else SaveData.SaveTexture2D(living1.tex, path);
        }
    }

    [MenuItem("Resident Evil/Import Texture Test")]
    public static void ImportTextureTest()
    {
        Living living1 = Liv.fromTim(1, 0x4B);

        SaveData.CheckFolderPLS(living1.playId, living1.emdId);
        string path = "Assets/EMD" + living1.playId + "/EM" + "_" + living1.playId + living1.emdId + "/" + living1.emdId;

        SaveData.SaveTexture2D(living1.tex, path);
    }

    [MenuItem("Resident Evil/Import PLDs")]
    public static void ImportPLDs()
    {
        int startId = 10;
        int endId = 90;
        for (int i = startId; i <= endId; i++)
        {
            Liv.fromPld(0, i); // Leon
            Liv.fromPld(1, i); // Leon
        }
    }

    [MenuItem("Resident Evil/Import PLD Test")]
    public static void ImportPLSTest()
    {
        Living living0 = Liv.fromPld(0); // Leon

        string path = "Assets/PLD" + living0.playId + "/PL" + "_" + living0.playId + living0.emdId;

        SaveData.SaveMD(living0.md , path);
    }

    [MenuItem("Resident Evil/Import Map Test")]
    public static void ImportMapTest()
    {
        int stage = 1;
        int room_nm = 0;
        int play_mode = 0; // 0 - Leon

        rdt rdt = new rdt();
        obj map_data = rdt.from(stage, room_nm, play_mode);
    }
}
