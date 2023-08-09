using UnityEngine.Windows;

public class File2
{
    bool LITTLE_ENDIAN = true;
    bool BIG_ENDIAN  = false;
    bool defaultEndian = true;

    //public void open(file, isAbsolute)
    //{
    //    if (!isAbsolute) file = pwd + file;
    //    try
    //    {
    //        const size = fs.fileSize(file);
    //        const buf = new ArrayBuffer(size);
    //        let fd = fs.open(file, 'rb');
    //        fs.read(fd, buf, 0, size, 0);
    //        fs.close(fd);

    //        return {
    //            buf, 
    //  size, 
    //};
    //    }
    //    catch (e)
    //    {
    //        throw new Error("open file " + file + "," + e.message);
    //    }
    //}

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