using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class SaveData
{
    public static void SaveMD(MD mod, string key, bool IsRecreateAsset = true)
    {
        int index = 0;
        int newindex = 0;
        List<Mesh> meshAssets = new List<Mesh>(); 
        foreach (Mesh mesh in mod.meshs)
        {
            string namePart = Enum.GetName(typeof(person), (person)index);
            if (string.IsNullOrEmpty(namePart))
            {
                namePart = newindex.ToString();
                ++newindex;
            }
            string path = key + "/" + namePart + ".mesh";

            SaveData.SaveMesh(mesh, path, IsRecreateAsset);
            meshAssets.Add( AssetDatabase.LoadAssetAtPath<Mesh>(path));
            index++;
        }

        List<GameObject> components = new List<GameObject>();
        GameObject mainGameObject = default(GameObject);

        string pathMaterial= key + "/" + mod.emdId + ".mat";
        Material mainMaterial = AssetDatabase.LoadAssetAtPath<Material>(pathMaterial);

        string[] namesBones = Enum.GetNames(typeof(person));

        foreach (Mesh mesh in meshAssets)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
            gameObject.GetComponent<MeshRenderer>().sharedMaterial = mainMaterial;
            gameObject.name = mesh.name;
            components.Add(gameObject);
            //if ( )
        }

        index = 0;
        foreach (SkeletonBone sk in mod.bone)
        {
            if (index >= components.Count)
            {
                break;
            }
            if (sk.parent == null)
            {
                mainGameObject = components[index];
            }
            GameObject currentObject = components[index];
            currentObject.transform.localPosition = -sk._pos * 0.01f;
            foreach (var child in sk.child)
            {
                components[child.idx].transform.SetParent(currentObject.transform, false);
            }
            ++index;
        }

        string pathPrefab = key + "/" + mod.emdId + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(mainGameObject, pathPrefab);
        //SaveData.SavePrefab(mainGameObject, pathPrefab);
    }

    public static void SaveMesh(Mesh mesh, string path, bool IsRecreateAsset = true)
    {
        if (AssetDatabase.LoadAssetAtPath<Mesh>(path) != null)
        {
            if ( IsRecreateAsset == true) 
            {
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.CreateAsset(mesh, path);
            }
            else
            {
                Mesh temp = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                temp.subMeshCount = mesh.subMeshCount;
                temp.vertices = mesh.vertices;
                temp.normals = mesh.normals;
                temp.uv = mesh.uv;
                temp.uv2 = mesh.uv2;
                for (int i = 0; i < temp.subMeshCount; i++)
                {
                    temp.SetTriangles(mesh.GetTriangles(i), i);
                }

                temp.RecalculateNormals();
                temp.RecalculateBounds();
                temp.Optimize();
                AssetDatabase.SaveAssetIfDirty(temp);
            }
        }
        else
        {
            AssetDatabase.CreateAsset(mesh, path);
        }
    }

    public static void SavePrefab(GameObject gameObject, string path)
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            GameObject temp = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        else
        {
            AssetDatabase.CreateAsset(gameObject, path);
        }
    }

    public static void SaveTexture2D(Texture2D texture, string path, bool CreateMaterial = true, bool IsRecreateAsset = true)
    {
        string pathTex = path + ".asset";
        Texture2D tempTexture = default(Texture2D);

        if (AssetDatabase.LoadAssetAtPath<Texture2D>(pathTex) != null)
        {
            tempTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(pathTex);

            tempTexture.Reinitialize(texture.width, texture.height, texture.format, false);
            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                    tempTexture.SetPixel(i,j, texture.GetPixel(i, j));
                }
            }
            tempTexture.Apply();
            AssetDatabase.SaveAssetIfDirty(tempTexture);
        }
        else
        {
            tempTexture = texture;
            AssetDatabase.CreateAsset(texture, pathTex);
        }

        Shader shader = Shader.Find("Standard (Specular setup)");
        string pathMat = path + ".mat";

        if (AssetDatabase.LoadAssetAtPath<Material>(pathMat) != null)
        {
            Material temp = AssetDatabase.LoadAssetAtPath<Material>(pathMat);
            temp.shader = shader;
            temp.SetTexture("_MainTex", tempTexture);
            AssetDatabase.SaveAssetIfDirty(temp);
        }
        else
        {
            Material material = new Material(shader);
            material.SetTexture("_MainTex", tempTexture);
            AssetDatabase.SaveAssetIfDirty(material);
            AssetDatabase.CreateAsset(material, pathMat);
        }

        byte[] bytes = tempTexture.EncodeToJPG();
        string pathPng = path + ".jpg";

        // For testing purposes, also write to a file in the project folder
        File.WriteAllBytes(pathPng, bytes);
    }

    private static string CheckFolderEMD(int playId, string emdId)
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

    public static string CheckFolderPLS(int playId, string emdId)
    {
        string folderEMD = "Assets/PLD" + playId;
        if (AssetDatabase.IsValidFolder(folderEMD) == false)
        {
            AssetDatabase.CreateFolder("Assets", "PLD" + playId);
        }
        string folderEM = folderEMD + "/PL" + emdId;
        if (AssetDatabase.IsValidFolder(folderEM) == false)
        {
            AssetDatabase.CreateFolder(folderEMD, "PL" + emdId);
        }
        return folderEM;
    }
}
