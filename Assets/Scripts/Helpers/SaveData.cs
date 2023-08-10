using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class SaveData
{
    public static void SaveMD(MD mod, int playId, string emdId)
    {
        int index = 0;
        foreach (var mesh in mod.meshs)
        {
            string folderEM = CheckFolder(playId, emdId);

            string namePart = Enum.GetName(typeof(person), (person)index);
            if (string.IsNullOrEmpty(namePart))
            {
                namePart = UnityEngine.Random.Range(0, 10).ToString();
            }
            string path = folderEM + "/" + namePart + ".mesh";

            if (AssetDatabase.LoadAssetAtPath<Mesh>(path) != null)
            {
                AssetDatabase.DeleteAsset(path);

                AssetDatabase.CreateAsset(mesh, path);
                //Mesh temp = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                //temp.subMeshCount = mesh.subMeshCount;
                //temp.vertices = mesh.vertices;
                //temp.normals = mesh.normals;
                //temp.uv = mesh.uv;
                //for (int i = 0; i < temp.subMeshCount; i++)
                //{
                //    temp.SetTriangles(mesh.GetTriangles(i), i);
                //}
                ////temp.uv = uv.ToArray();

                //temp.RecalculateNormals();
                //temp.RecalculateBounds();
                //temp.Optimize();
            }
            else
            {
                AssetDatabase.CreateAsset(mesh, path);
            }
            ++index;
        }
    }

    public static void SaveTexture2D(Texture2D texture, int playId, string emdId, bool CreateMaterial = true)
    {
        string folderEM = CheckFolder(playId, emdId);

        string path = folderEM + "/" + emdId + ".asset";

        if (AssetDatabase.LoadAssetAtPath<Texture2D>(path) != null)
        {
            Texture2D temp = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            temp.Reinitialize(texture.width, texture.height, texture.format, false);
            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                    temp.SetPixel(i,j, texture.GetPixel(i, j));
                }
            }
            temp.Apply();
        }
        else
        {
            AssetDatabase.CreateAsset(texture, path);
        }

        Shader shader = Shader.Find("Standard (Specular setup)");
        Material material = new Material(shader);
        material.SetTexture("_MainTex", texture);
        string pathMat = folderEM + "/" + emdId + ".mat";

        if (AssetDatabase.LoadAssetAtPath<Material>(pathMat) != null)
        {
            Material temp = AssetDatabase.LoadAssetAtPath<Material>(pathMat);
            temp.SetTexture("_MainTex", texture);
        }
        else
        {
            AssetDatabase.CreateAsset(material, pathMat);
        }
    }

    private static string CheckFolder(int playId, string emdId)
    {
        string folderEMD = "Assets/EMD" + playId;
        if (AssetDatabase.IsValidFolder(folderEMD) == false)
        {
            AssetDatabase.CreateFolder("Assets", "EMD" + playId);
        }
        string folderEM = folderEMD + "/EM_" + playId + emdId;
        if (AssetDatabase.IsValidFolder(folderEM) == false)
        {
            AssetDatabase.CreateFolder(folderEMD, "EM_" + playId + emdId);
        }
        return folderEM;
    }
}
