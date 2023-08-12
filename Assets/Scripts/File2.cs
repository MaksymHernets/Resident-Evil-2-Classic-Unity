using System;
using UnityEngine.Windows;

public class File2
{
    //bool LITTLE_ENDIAN = true;
    //bool BIG_ENDIAN  = false;
    //bool defaultEndian = true;

    public static dataViewExt open(string file, bool isAbsolute = true)
    {
        byte[] buf = File.ReadAllBytes("Assets/" + file);
        //int fd = fs.open(file, 'rb');
        //fs.read(fd, buf, 0, size, 0);
        //fs.close(fd);

        //return { buf, size, };

        DataView dataView = new DataView(buf);
        dataView.path = file;
        dataViewExt dataViewExt = new dataViewExt(dataView);
        dataViewExt.path = file;
        return dataViewExt;

        //if (!isAbsolute) file = pwd + file;
        try
        {
            //int size = fs.fileSize(file);
            //byte[] buf = new ArrayBuffer(size);
            
        }
        catch (Exception e)
        {
            //throw new Error("open file " + file + "," + e.message);
        }
    }

    public static dataViewExt openDataView(string file)
    {
        //AssetDatabase.LoadAssetAtPath<Text>(texfile);
        if ( File.Exists("Assets/" + file) == false ) return null;
        byte[] buf2 = File.ReadAllBytes("Assets/" + file);
        //byte[] buf3 = Reverse(buf2);
        DataView buf = new DataView(buf2);
        buf.path = file;
        dataViewExt dataViewExt = new dataViewExt(buf);
        dataViewExt.path = file;
        return dataViewExt;
        //return new DataView(open(file).buf);
    }

    private static byte[] Reverse(byte[] data)
    {
        byte[] datareverse = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            datareverse[data.Length - i - 1] = data[i];
        }
        return datareverse;
    }
}