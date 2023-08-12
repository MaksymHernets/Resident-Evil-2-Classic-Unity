using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class DataView
{
    public byte[] buffer;
    public int byteLength = 0;
    public int byteOffset = 0;
    public string path;

    public DataView(byte[] data)
    {
        this.buffer = data;
        byteOffset = 0;
    }

    public DataView(byte[] data, int offset)
    {
        this.buffer = data;
        byteOffset = offset;
    }

    public DataView(byte[] data, int offset, int len)
    {
        this.buffer = data;
        byteOffset = offset;
        byteLength = len;
    }

    public UInt16 getUint4(int offset, bool littleEndian = true)
    {
        byte[] bytes = new byte[2];
        bytes[0] = buffer[offset];
        bytes[1] = 0;
        if (littleEndian == false) bytes = Reverse(bytes);
        return BitConverter.ToUInt16(bytes, 0);
    }

    public UInt16 getUint8(int offset, bool littleEndian = true)
    {
        byte[] bytes = new byte[2];
        bytes[0] = buffer[offset];
        bytes[1] = 0;
        if (littleEndian == false) bytes = Reverse(bytes);
        return BitConverter.ToUInt16(bytes, 0);
    }

    public UInt16 getUint16(int offset, bool littleEndian = true)
    {
        byte[] bytes = new byte[2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = buffer[offset + i];
        }
        if (littleEndian == false) bytes = Reverse(bytes);
        return BitConverter.ToUInt16(bytes, 0);
    }

    public UInt16 getUint24(int offset, bool littleEndian = true)
    {
        byte[] bytes = new byte[3];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = buffer[offset + i];
        }
        if (littleEndian == false) bytes = Reverse(bytes);
        return BitConverter.ToUInt16(bytes, 0);
    }

    public UInt32 getUint32(int offset, bool littleEndian = true)
    {
        byte[] bytes = new byte[4];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = buffer[offset + i];
        }
        if (littleEndian == false) bytes = Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }

    public Int16 getInt8(int offset, bool littleEndian = true)
    {
        byte[] bytes = new byte[1];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = buffer[offset + i];
        }
        if (littleEndian == false) bytes = Reverse(bytes);
        return BitConverter.ToInt16(bytes, 0);
    }

    public Int16 getInt16(int offset, bool littleEndian = true)
    {
        byte[] bytes = new byte[2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = buffer[offset + i];
        }
        if (littleEndian == false) bytes = Reverse(bytes);
        return BitConverter.ToInt16(bytes, 0);
    }

    public Int32 getInt32(int offset, bool littleEndian = true)
    {
        byte[] bytes = new byte[4];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = buffer[offset + i];
        }
        if ( littleEndian == false) bytes = Reverse( bytes );
        return BitConverter.ToInt32(bytes, 0);
    }

    public float getFloat16(int offset, bool littleEndian = true)
    {
        //byte[] bytes = new byte[4];
        //for (int i = 0; i < 2; i++)
        //{
        //    bytes[i] = data[offset + i];
        //}
        //bytes[2] = 0;
        //bytes[3] = 0;
        //if (littleEndian == false) bytes = Reverse(bytes);
        return Parse16BitFloat(buffer[offset], buffer[offset + 1]);
    }

    public float Parse16BitFloat(byte HI, byte LO)
    {
        // Program assumes ints are at least 16 bits
        int fullFloat = ((HI << 8) | LO);
        int exponent = (HI & 0b01111110) >> 1; // minor optimisation can be placed here
        int mant = fullFloat & 0x01FF;

        // Special values
        if (exponent == 0b00111111) // If using constants, shift right by 1
        {
            // Check for non or inf
            return mant != 0 ? float.NaN :
                ((HI & 0x80) == 0 ? float.PositiveInfinity : float.NegativeInfinity);
        }
        else // normal/denormal values: pad numbers
        {
            exponent = exponent - 31 + 127;
            mant = mant << 14;
            Int32 finalFloat = (HI & 0x80) << 24 | (exponent << 23) | mant;
            return BitConverter.ToSingle(BitConverter.GetBytes(finalFloat), 0);
        }
    }

    public float getFloat32(int offset, bool littleEndian = true)
    {
        return 0;
    }

    public float getFloat64(int offset, bool littleEndian = true)
    {
        return 0;
    }

    private byte[] Reverse(byte[] data)
    {
        byte[] datareverse = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            datareverse[data.Length-i-1] = data[i];
        }
        return datareverse;
    }

    //public int Ulong()
    //{
    //    int offset = _offset;
    //    _offset += 4;
    //    return (int)getUint32(offset, true);
    //}

    //public int Ulong(int offset)
    //{
    //    return (int)getUint32(offset, true);
    //}

    public Int16[] build_Int16Array(DataView v, int begin, int length)
    {
        Int16[] array = new Int16[length/2];
        int j = 0;
        for (int i = begin; i < begin + length - 1; i += 2)
        {
            array[j] = getInt16(i);
            ++j;
        }
        return array;
    }

    public float[] build_Float16Array(DataView v, int begin, int length)
    {
        float[] array = new float[length / 2];
        int j = 0;
        for (int i = begin; i < begin + length - 1; i += 2)
        {
            array[j] = getFloat16(i);
            ++j;
        }
        return array;
    }

    public UInt16[] build_UInt16Array(DataView v, int begin, int length)
    {
        UInt16[] array = new UInt16[length/2];
        int j = 0;
        for (int i = begin; i < begin + length - 1; i += 2)
        {
            array[j] = getUint16(i);
            ++j;
        }
        return array;
    }

    public UInt16[] build_UInt8Array(DataView v, int begin, int length)
    {
        UInt16[] array = new UInt16[length];
        int j = 0;
        for (int i = begin; i < begin + length; i++)
        {
            array[j] = getUint8(i);
            ++j;
        }
        return array;
    }

    public UInt16[] build_UInt4Array(DataView v, int begin, int length)
    {
        UInt16[] array = new UInt16[length];
        int j = 0;
        for (int i = begin; i < begin + length; i++)
        {
            array[j] = getUint4(i);
            ++j;
        }
        return array;
    }
}


