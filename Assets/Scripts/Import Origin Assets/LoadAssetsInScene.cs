using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAssetsInScene : MonoBehaviour
{
    [MenuItem("Resident Evil/Load Models In Scene")]
    public static void LoadModelsInScene()
    {
        int startId = 16;
        int endId = 90;

        int start = 4;
        int wigth = 2;

        for (int i = startId; i <= endId; i++)
        {
            if (i > 60) { start = 20; startId = 90; }
            string emdId = tool.toString(i, 16);
            string playId = 0.ToString();

            string path = "Assets/EMD" + playId + "/EM" + "_" + playId + emdId + "/" + emdId + ".prefab";
            GameObject prefab0 = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if ( prefab0 != null)
            {
                GameObject gameObject0 = GameObject.Instantiate(prefab0);
                gameObject0.transform.position = new Vector3(start, 0, wigth * i - startId);
            }
            EditorSceneManager.SaveOpenScenes();
            //emdId = tool.toString(i, 16);
            //playId = 1.ToString();

            //path = "Assets/EMD" + playId + "/EM" + "_" + playId + emdId + "/" + emdId + ".prefab";
            //GameObject prefab1 = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            //if (prefab1 != null)
            //{
            //    GameObject gameObject1 = GameObject.Instantiate(prefab0);
            //    gameObject1.transform.position = new Vector3(-start, 0, wigth * i - startId);
            //}
        }
    }
}
