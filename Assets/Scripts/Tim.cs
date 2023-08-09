using UnityEngine;

public class Tim
{
    public int w;
    public int h;

    public static Texture parseStream(DataView buf)
    {
        // h.printHex(new Uint8Array(buf.buffer, buf.byteOffset, 100));
        var head = buf.getUint32(0, true);
        if (head != 0x10)
        {
            throw new Error("bad TIM stream " + head);
        }
        return default(Texture);
    }

    public static int _width(int type, int w)
    {
        switch (type)
        {
            case 0x02: return w;      // 16bit 
            case 0x09: return w << 1; //  8bit * 2
            case 0x08: return w << 2; //  4bit * 4
        }
        return 0;
    }
}
