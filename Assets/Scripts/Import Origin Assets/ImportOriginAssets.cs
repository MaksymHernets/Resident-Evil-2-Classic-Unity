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

            SaveData.CheckFolderPLS(living0.md.playId, living0.md.emdId);
            string path = "Assets/EMD" + living0.md.playId + "/EM" + "_" + living0.md.playId + living0.md.emdId;

            SaveData.SaveMD(living0.md, path);

            Living living1 = Liv.fromEmd(1, i);

            SaveData.CheckFolderPLS(living1.md.playId, living1.md.emdId);
            path = "Assets/EMD" + living1.md.playId + "/EM" + "_" + living1.md.playId + living1.md.emdId;

            SaveData.SaveMD(living1.md, path);
        }
    }

    [MenuItem("Resident Evil/Import Model Test")]
    public static void ImportModelTest()
    {
        Living living1 = Liv.fromEmd(1, 0x4B);

        SaveData.CheckFolderPLS(living1.md.playId, living1.md.emdId);
        string path = "Assets/EMD" + living1.md.playId + "/EM" + "_" + living1.md.playId + living1.md.emdId;

        SaveData.SaveMD(living1.md, path, false);
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
        Liv.fromTim(1, 0x4B);
    }

    [MenuItem("Resident Evil/Import PLDs")]
    public static void ImportMaps()
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
    public static void ImportMapTest()
    {
        Living liv = Liv.fromPld(0); // Leon

        //SaveData.SaveMD(liv.tex , playId, emdId);
    }
}