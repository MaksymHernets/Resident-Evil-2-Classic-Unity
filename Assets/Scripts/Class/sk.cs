using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sk  // frame data
{
    public float x;
    public float y;
    public float z;
    public int spx;
    public int spy;
    public int spz;
    public float frameTime;
    public float moveStep;
    public Vector3[] angle;
    public List<int> child;

    public sk()
    {
        child = new List<int>();
    }
}
