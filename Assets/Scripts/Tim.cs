using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Tim // Texture import
{
    public int w;
    public int h;

    public static Texture2D parseStream(DataView buf)
    {
        // h.printHex(new Uint8Array(buf.buffer, buf.byteOffset, 100));
        if ( buf == null) return null;
        var head = buf.getUint32(0, true);
        if (head != 0x10)
        {
            throw new Error("bad TIM stream " + head);
        }
        
        int type = (int)buf.getUint32(4, true);
        int offset = (int)buf.getUint32(8, true);
        int pal_x = buf.getUint16(12, true);
        int pal_y = buf.getUint16(14, true);
        int palette_colors = buf.getUint16(16, true);
        int nb_palettes = buf.getUint16(18, true);
        int vi = 20;

        //console.debug('TIM palettes color', palette_colors, 'nb', nb_palettes, 
        //'pal-x', pal_x, 'pal-y', pal_y, 'offset', offset);

        // 调色板被纵向平均应用到图像上
        List<List<int>> palettes = new List<List<int>>(nb_palettes);
        for (int p = 0; p < nb_palettes; ++p) 
        {
            //palettes[p] = new Uint16Array(buf.buffer, buf.byteOffset + vi, palette_colors);
            palettes.Add(Uint16Array(buf, buf.byteOffset + vi, palette_colors*2 ).ToList());
            vi += palette_colors * 2;
            // console.debug("Palette", p);
            // h.printHex(palettes[p]);
        }

        int width = _width(buf.getUint16(vi + 8, true));
        int height = buf.getUint16(vi + 10, true);
        int wxh = width * height;
        int byteLength = _offset(width) * height + vi + 12;

        // if (buf.getUint16(vi, true) - 12 != width * height) 
        //   throw new Error("bad size");
        //console.debug("Tim pic Size:", wxh,
        // '[', width, 'x', height, '] byte:', byteLength);

        int buffer_index_offset = buf.byteOffset + offset + 20;
        int[] imgbuf = new int[wxh]; // new Float32Array(wxh *4);
        //int set_color = _set_short; // _set_float
        fill_image(0);

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        List<Color> colors = new List<Color>();

        //for (int i = wxh-1; i >= 0; --i)
        //{
        //    colors.Add(IntToColor(imgbuf[i]));
        //}
        for (int i = 0; i < wxh; ++i)
        {
            colors.Add(IntToColor(imgbuf[i]));
        }
        texture.SetPixels(0, 0, width, height, colors.ToArray());
        //for (int i = 0; i < height; i++)
        //{
        //    for (int j = 0; j < width / 2; j++)
        //    {
        //        int end = width - 1 - j;
        //        Color colortempFirst = texture.GetPixel(j, i);
        //        Color colortempEnd = texture.GetPixel(end, i);
        //        texture.SetPixel(j, i, colortempEnd);
        //        texture.SetPixel(end, i, colortempFirst);
        //    }
        //}
        texture.Apply();
        return texture;

        //return {
        //// 图像缓冲区
        //buf: imgbuf,
        //    // 调色板数量/图像分割数量
        //    nb_palettes,
        //    // 像素高度
        //    height,
        //    // 像素宽度
        //    width,
        //    // TIM 数据块的总长度
        //    byteLength,
        //    // 作为贴图绑定到模型
        //    bindTexTo,
        //  };

        void bindTexTo()
        {
            //draw.bindTexImage(imgbuf, width, height,
                //gl.GL_RGBA, gl.GL_UNSIGNED_SHORT_1_5_5_5_REV);
        }

        // 留作备用
        // A1B5G5R5 to Float{r,g,b,a}
        void _set_float(int pixel, int color /* A1B5G5R5 */)
        {
            int i = pixel * 4;
            /* R */
            imgbuf[i] = (0x1F & color) / 0x1F;
            /* G */
            imgbuf[i + 1] = ((0x03E0 & color) >> 5) / 0x1F;
            /* B */
            imgbuf[i + 2] = ((0x7C00 & color) >> 10) / 0x1F;
            /* A */
            imgbuf[i + 3] = (0x0F000 & color) >> 15;
        }

        Color IntToColor(int color /* A1B5G5R5 */)
        {
            Color colorr = new Color();
            /* R */
            colorr.r = (31 & color) / 31f; // 0x1F
            /* G */
            colorr.g = ((992 & color) >> 5) / 31f; // 0x03E0 0x1F
            /* B */
            colorr.b = ((31744 & color) >> 10) / 31f; // 0x7C00 0x1F
            /* A */
            colorr.a = (61440 & color) >> 15; // 0x0F000
            return colorr;
        }

        void _set_short(int pixel, int color)
        {
            imgbuf[pixel] = color;
        }

        int _width(int w)
        {
            switch (type)
            {
                case 0x02: return w;      // 16bit 
                case 0x09: return w << 1; //  8bit * 2
                case 0x08: return w << 2; //  4bit * 4
            }
            return 0;
        }

        int _offset(int w)
        {
            switch (type)
            {
                case 0x02: return w << 1; // 16bit * 2
                case 0x09: return w;      //  8bit 
                case 0x08: return w >> 1; //  4bit / 2
            }
            return 0;
        }

        //
        // 切换调色板, 使用调色板颜色重新填充 imbuf 缓冲区.
        //
        void fill_image(int i = 0)
        {
            switch (type)
            {
                case 0x02:
                    //console.debug("16 bit color");
                    bit16color();
                    break;

                case 0x08:
                    //console.debug("4 bit color");
                    bit4color();
                    break;

                case 0x09:
                    //console.debug("8 bit color");
                    bit8color();
                    break;

                default:
                    throw new Error("unsupport color type " + type);
            }
        }

        void bit16color()
        {
            //int[] index = new Uint16Array(buf.buffer, buffer_index_offset, wxh); // Uint16Array
            int[] index = Uint16Array(buf, buffer_index_offset, wxh);
            for (int i = 0; i < wxh; ++i)
            {
                _set_short(i, index[i]);
            }
        }

        void bit4color()
        {
            //int[] index = new Uint8Array(buf.buffer, buffer_index_offset, wxh / 2); //Uint8Array
            int[] index = Uint4Array(buf, buffer_index_offset, wxh);
            int pl = (int)(width / palettes.Count); //parseInt

            for (int i = 0; i < wxh; i += 2)
            {
                int indexParse = (int)(i % width / pl);
                int[] pal = palettes[indexParse].ToArray(); //parseInt
                int color = index[i / 2];
                int c1 = pal[(color & 0xF0) >> 4];
                _set_short(i, c1);
                int c2 = pal[color & 0x0F];
                _set_short(i + 1, c2);
            }
        }

        void bit8color()
        {
            //int[] index = new Uint8Array(buf.buffer, buffer_index_offset, wxh);
            int[] index = Uint8Array(buf, buffer_index_offset, wxh);
            int pl = (int)((float)width / (float)palettes.Count); //parseInt

            for (int i = 0; i < wxh; ++i)
            {
                int indexParse = (int)(i % width / (float)pl);
                if (indexParse >= nb_palettes) indexParse = nb_palettes - 1;
                int[] pal = palettes[indexParse].ToArray(); //parseInt
                if (pal.Length != 0)
                {
                    int _a = index[i];
                    _set_short(i, pal[_a]);
                }
                else
                {
                    //console.error(i, width, pl, i % width / pl, pal, palettes.length);
                }
            }
        }
    }

    public static int[] Uint16Array(DataView buf, int offset, int count)
    {
        UInt16[] array = buf.build_UInt16Array(buf, offset, count);
        int[] arrayInt = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            arrayInt[i] = array[i];
        }
        return arrayInt;
    }

    public static int[] Uint8Array(DataView buf, int offset, int count)
    {
        UInt16[] array = buf.build_UInt8Array(buf, offset, count);
        int[] arrayInt = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            arrayInt[i] = array[i];
        }
        return arrayInt;
    }

    public static int[] Uint4Array(DataView buf, int offset, int count)
    {
        UInt16[] array = buf.build_UInt4Array(buf, offset, count);
        int[] arrayInt = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            arrayInt[i] = array[i];
        }
        return arrayInt;
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
