using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Error : Exception
{
    public Error(string message) : base(message)
    {

    }

    public Error(string message, string message2) : base(message)
    {

    }
}
