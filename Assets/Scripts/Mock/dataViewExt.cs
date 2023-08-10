using System;
using UnityEngine.UIElements;

public class dataViewExt : DataView
{
    bool littleEndian = true;
    int pos;
    //public DataView v;
    public int h_dir_count;
    public int h_dir_size;
    public int h_dir_offset;

    public dataViewExt(DataView v) : base(v.buffer)
    {
        //this.v = v;
        pos = 0;
        //pos = v._offset;
    }

    public int getpos()
    {
        return pos;
    }

    // 设置内置指针
    public void setpos(int i)
    {
        if (i >= 0) pos = i;
        else throw new Error("bad pos:" + i);
    }

    // 相对移动当前指针位置移动指针
    public void movepos(int i)
    {
        if (Single.IsNaN(i)) throw new Error("bad offset " + i);
        pos += i;
    }

    //return Object.assign(v, {
    //    // 返回内置指针的值, 内置指针总是指向即将读取的数值的位置
    //}

    // 设置小端字节序, 默认为 LittleEndian
    public void setLittleEndian()
    {
        littleEndian = true;
    }

    // 设置为大端字节序
    public void setBigEndian()
    {
        littleEndian = false;
    }

    public bool isLittleEndian()
    {
        return littleEndian;
    }

    // 返回一个字节的无符号整型
    public byte Byte(int offset = 0)
    {
        return (byte)getUint8(P(offset, 1), littleEndian);
    }

    // 返回一个字节的有符号整型
    public char Char(char i)
    {
        return (char)getInt8(P(i, 1), littleEndian);
    }

    // 2个字节有符号整型
    public short Short(int i = 0)
    {
        return getInt16(P(i, 2), littleEndian);
    }

    // 2个字节无符号整型
    public ushort Ushort(int i = 0)
    {
        return getUint16(P(i, 2), littleEndian);
    }

    //public ushort Ushort()
    //{
    //    int offset = pos;
    //    pos += 2;
    //    return (ushort)getUint16(P(offset, 2), littleEndian);
    //}

    // 4字节有符号整型
    public int Long(int offset = 0)
    {
        return getInt32(P(offset, 4), littleEndian);
    }

    // 4字节无符号整型
    public int Ulong(int offset = 0)
    {
        return (int)getUint32(P(offset, 4), littleEndian);
    }

    //public int Ulong()
    //{
    //    int offset = pos;
    //    //_offset += 4;
    //    pos += 4;
    //    return (int)getUint32(P((int)offset, 4), littleEndian);
    //}

    // 4字节浮点数
    public float Float(float i)
    {
        return getFloat32(P((int)i, 4), littleEndian);
    }

    // 8字节浮点数
    public float Float64(float i)
    {
        return getFloat64(P((int)i, 8), littleEndian);
    }

    // 16进制格式打印缓冲区内容, begin 未设置使用内置指针
    // length 默认为 0x100 字节, 该方法不改变内置指针的位置
    public void print(int begin, int length)
    {
        //var buf = this.build(Uint8Array, begin || pos, length || 0x100);
        //H.printHex(buf);
    }

    public int _offset(int typeIdx)
    {
        //buf._offset += typeIdx;
        if (typeIdx >= h_dir_count)
        {
            throw new Error("Dir Exceeded the maximum", h_dir_count.ToString());
        }
        var r = Ulong(h_dir_offset + (typeIdx << 2));
        //debug("] OFFSET", typeIdx, h4(r), 'AT', h4(h_dir_offset + (typeIdx << 2)));
        return (int)r;
    }

    private int P(int i, int o)
    {
        if ( i == 0)
        {
            i = pos;
            pos += o;
        }
        else
        {
            pos = i + o;
        }
        return i;
    }

    // 用视图中的缓冲区构建 T 类型的 TypedArray
    // 该方法不会引起内存复制, 任何修改都会反映在原始视图和创建的视图上.
    public void build(int begin, int length)
    {
        try
        {
            // console.log("build buf:", v.byteOffset, v.byteOffset + begin, length);
            //return new T(v.buffer, v.byteOffset + begin, length);
        }
        catch (Exception e)
        {
            throw new Error(e.Message + " Offset:" + begin
                + " Length:" + length + " Total:" + byteLength);
        }
    }
}