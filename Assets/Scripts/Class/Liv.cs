using System;
using System.IO;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.WSA;
using File = UnityEngine.Windows.File;

public static class Liv
{
    public static string rootpath = "sourcegame/";

    public static Living fromEmd(int playId, int emdId)
    {
        var e = tool.toString(emdId, 16);

        var p = playId == 0 ? 1 : 0;
        //if (emdId < 0x10) throw new Error("bad emd id");
        return loadEmd(p, e);
    }

    public static Living fromTim(int playId, int emdId)
    {
        var e = tool.toString(emdId, 16);

        var p = playId == 0 ? 1 : 0;
        //if (emdId < 0x10) throw new Error("bad emd id");
        return loadTim(p, e);
    }

    public static Living loadTim(int playId, string emdId)
    {
        Living living = new Living();
        string key = rootpath + "PL" + playId + "/EMD" + playId + "/EM" + playId + emdId;
        string texfile = key + ".TIM";

        living.path = key;
        living.playId = playId;
        living.emdId = emdId;

        if (File.Exists("Assets/" + texfile) == false) return living;

        Texture2D tex = Tim.parseStream(File2.openDataView(texfile));
        living.tex = tex;

        return living;
    }

    public static Living loadEmd(int playId, string emdId)
    {
        Living living = new Living();
        string key = rootpath + "PL" + playId + "/EMD" + playId + "/EM" + playId + emdId;
        string emdfile = key + ".EMD";
        string texfile = key + ".TIM";

        living.path = key;
        living.playId = playId;
        living.emdId = emdId;

        if (File.Exists("Assets/" + emdfile) == false) return null;
        //console.debug("Load EMD", emdfile, '-', texfile);
        MD mod = Model2.emd(emdfile);

        mod.playId = playId;
        mod.emdId = emdId;
        living.md = mod;

        if (File.Exists("Assets/" + texfile) == false) return living;

        return living;
    }

    public static Living fromPld(int playId, int _modid = 0)
    {
        // PL00CH.PLD PL00.PLD
        string path = rootpath + "PL" + playId + "/PLD/PL" + tool.b2(_modid);
        string file = path + ".PLD";

        if (File.Exists("Assets/" + file) == false) return null;

        MD mod = Model2.pld(file);
        mod.playId = playId;
        //mod.emdId = emdId;
        console.debug("Load PLD", file);

        string path2 = SaveData.CheckFolderPLS(playId, tool.b2(_modid));
        SaveData.SaveMD(mod, path2 + "/" + tool.b2(_modid));
        //SaveData.SaveTexture2D(mod.tex, file);

        return new Living(mod, mod.tex);
    }
}
