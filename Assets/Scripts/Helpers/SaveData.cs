using System;
using UnityEditor;
using UnityEngine;

public static class SaveData
{
    public static void SaveMD(MD mod, int playId, string emdId)
    {
        int index = 0;
        foreach (var mesh in mod.meshs)
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
            string namePart = Enum.GetName(typeof(person), (person)index);
            
            if (string.IsNullOrEmpty(namePart))
            {
                namePart = UnityEngine.Random.Range(0, 10).ToString();
            }
            string path = folderEM + "/" + namePart + ".mesh";
            if (AssetDatabase.LoadAssetAtPath<Mesh>(path) != null)
            {
                Mesh temp = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                temp.subMeshCount = mesh.subMeshCount;
                temp.vertices = mesh.vertices;
                temp.normals = mesh.normals;
                for (int i = 0; i < temp.subMeshCount; i++)
                {
                    temp.SetTriangles(mesh.GetTriangles(i), i);
                }
                //temp.uv = uv.ToArray();

                temp.RecalculateNormals();
                temp.RecalculateBounds();
                temp.Optimize();
            }
            else
            {
                AssetDatabase.CreateAsset(mesh, path);
            }
            ++index;
        }
    }
}
