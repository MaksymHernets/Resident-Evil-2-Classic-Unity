using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Array 
{
    public int[] arr;

    public Array(int lenght)
    {
        arr = new int[lenght];
    }

    public int this[int key]
    {
        get => arr[key];
        set => arr[key] = value;
    }
}
