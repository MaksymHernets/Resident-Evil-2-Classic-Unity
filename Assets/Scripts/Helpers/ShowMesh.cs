using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class ShowMesh : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Vector3 scale = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField] static private Vector3 scaleDot = new Vector3(0.3f, 0.3f, 0.3f);

    private List<GameObject> objects = new List<GameObject>();

    [ContextMenu("Start")]
    private void Start()
    {
        foreach (var obj in objects)
        {
            DestroyImmediate(obj);
        }
        foreach (var child in mesh.vertices)
        {
            GameObject newobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newobj.transform.SetParent(transform);
            newobj.transform.localScale = scaleDot;
            child.Scale(scale);
            newobj.transform.localPosition = child;
            objects.Add(newobj);
        }
    }

    [ContextMenu("SeeIn")]
    private void SeeIn()
    {
        var t = mesh.uv;
    }

    public static void Show(Vector3[] vertices)
    {
        foreach (var child in vertices)
        {
            GameObject newobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //newobj.transform.SetParent(transform);
            newobj.transform.localScale = scaleDot;
            //child.Scale(scale);
            newobj.transform.localPosition = child;
            //objects.Add(newobj);
        }
    }
}
