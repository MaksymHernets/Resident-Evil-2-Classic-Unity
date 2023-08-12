using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vab
{
    public int version;
    public int bankid;
    public int filesize;
    public int reserved0;
    public int num_progs;
    public int num_tones;
    public int num_vags;

    public int master_vol;
    public int master_pan;

    public int attr1;
    public int attr2;

    public int reserved1;
    public List<prog> prog;
    public List<int> raw;
}
