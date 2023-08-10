using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.WSA;


public static class Liv
{
    public static Living fromEmd(int playId, int emdId)
    {
        var e = toString(emdId, 16);

        var p = playId == 0 ? 1 : 0;
        //if (emdId < 0x10) throw new Error("bad emd id");
        return loadEmd(p, e);
    }

    public static void fromTim(int playId, int emdId)
    {
        var e = toString(emdId, 16);

        var p = playId == 0 ? 1 : 0;
        //if (emdId < 0x10) throw new Error("bad emd id");
        loadTim(p, e);
    }

    private static string toString(int value, int rank)
    {
        if (rank == 16)
        {
            return value.ToString("X");
        }
        return "";
    }

    public static void loadTim(int playId, string emdId)
    {
        string key = "PL" + playId + "/EMD" + playId + "/EM" + playId + emdId;
        string texfile = key + ".TIM";

        if (File.Exists("Assets/" + texfile) == false) return;

        Texture2D tex = Tim.parseStream(File2.openDataView(texfile));

        SaveData.SaveTexture2D(tex, playId, emdId);

        //Living thiz = new Living(mod, tex);
        //thiz.texfile = texfile;
        //return thiz;
    }

    public static Living loadEmd(int playId, string emdId)
    {
        string key = "PL" + playId + "/EMD" + playId + "/EM" + playId + emdId;
        string emdfile = key + ".EMD";
        string texfile = key + ".TIM";

        //console.debug("Load EMD", emdfile, '-', texfile);
        MD mod = Model2.emd(emdfile);

        if (File.Exists("Assets/" + emdfile) == false) return null;

        SaveData.SaveMD(mod, playId, emdId);

        //Texture2D tex = Tim.parseStream(File2.openDataView(texfile));

        //SaveData.SaveTexture2D(tex, playId, emdId);

        //Living thiz = new Living(mod, tex);
        //thiz.texfile = texfile;
        return default(Living);
    }
}
public class Living
{
    
    public Living(MD md, Texture texture)
    {

    }

}
