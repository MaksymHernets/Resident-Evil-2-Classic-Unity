using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportOriginAssets : MonoBehaviour
{
    [MenuItem("Resident Evil/Import Textures (TIM)", false, 0 )]
    public static void ImportTextures()
    {
        int startId = 16;
        int endId = 90;
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

    [MenuItem("Resident Evil/Create Material For Models", false, 1)]
    public static void CreateMaterialForModels()
    {
        int startId = 16;
        int endId = 90;
        string path;
        for (int i = startId; i <= endId; i++)
        {
            string playId = "0";
            string emdId = tool.toString(i, 16);
            path = "Assets/EMD" + playId + "/EM" + "_" + playId + emdId + "/" + emdId;

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path + ".png");
            
            if (texture == null) Debug.LogWarning(path);
            else SaveData.SaveMaterial(texture, path);

            Living living1 = Liv.fromTim(1, i);
            path = "Assets/EMD" + living1.playId + "/EM" + "_" + living1.playId + living1.emdId + "/" + living1.emdId;

            if (living1.tex == null) Debug.LogWarning(path);
            else SaveData.SaveTexture2D(living1.tex, path);
        }
    }

    [MenuItem("Resident Evil/Import Models (EMD)", false, 2)]
    public static void ImportModels()
    {
        int startId = 16;
        int endId = 90;
        for (int i = startId; i <= endId; i++)
        {
            Living living0 = Liv.fromEmd(0, i);

            if (living0 != null)
            {
                SaveData.CheckFolderEMD(living0.playId, living0.emdId);
                string path = "Assets/EMD" + living0.playId + "/EM" + "_" + living0.playId + living0.emdId;
                SaveData.SaveMD(living0.md, path, false);

            }

            Living living1 = Liv.fromEmd(1, i);
            if (living1 != null)
            {
                SaveData.CheckFolderEMD(living1.playId, living1.emdId);
                string path = "Assets/EMD" + living1.playId + "/EM" + "_" + living1.playId + living1.emdId;
                SaveData.SaveMD(living1.md, path, false);
            }
        }
    }

    [MenuItem("Resident Evil/Import Models (PLD)", false, 3)]
    public static void ImportPLDs()
    {
        int startId = 16;
        int endId = 90;
        for (int i = startId; i <= endId; i++)
        {
            Liv.fromPld(0, i); // Leon
            Liv.fromPld(1, i); // Leon
        }
    }
}
