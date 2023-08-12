using System.Collections.Generic;

public class bgm
{
    public static vab parse_vab_header(dataViewExt v, int offset, int vagoff)
    {
        v.setLittleEndian();
        // pBAV
        if (v.Byte(offset) != 0x70 || v.Byte() != 0x42 ||
            v.Byte() != 0x41 || v.Byte() != 0x56) 
    {
            throw new Error("bad vab file");
        }

        vab vab = new vab();
        vab.version = v.Ulong();
        vab.bankid = v.Ulong();
        vab.filesize = v.Ulong();
        vab.reserved0 = v.Ushort();
        vab.num_progs = v.Ushort();
        vab.num_tones = v.Ushort();
        vab.num_vags = v.Ushort();
        vab.master_vol = v.Byte();
        vab.master_pan = v.Byte();
        vab.attr1 = v.Byte();
        vab.attr2 = v.Byte();
        vab.reserved1 = v.Ulong();
        vab.prog = new List<prog>(vab.num_progs); //new Array(vab.num_progs);
        vab.raw = new List<int>(vab.num_vags);//new Array(vab.num_vags);
        //debug(vab);

        for (int i = 0; i < 128; ++i)
        {
            prog prog = new prog();
            prog.num_tones = v.Byte();
            prog.vol = v.Byte();
            prog.priority = v.Byte();
            prog.mode = v.Byte();
            prog.pan = v.Byte();
            prog.reserved0 = v.Byte();
            prog.attr = v.Ushort();
            prog.reserved1 = v.Ulong();
            prog.reserved2 = v.Ulong();
            if (i < vab.num_progs)
            {
                prog.tone = new List<tone>(prog.num_tones);//new Array();
                vab.prog[i] = prog;
                //debug('Program', i, prog);
            }
        }

        for (int j = 0; j < vab.num_progs; ++j)
        {
            // debug('pos', v.getpos());
            for (int c = 0; c < 16; ++c)
            {
                // tone 是音区
                tone tone = new tone();
                tone.priority = v.Byte();
                // 音调模式 0 =正常; 4 =应用混响
                tone.mode = v.Byte();
                tone.vol = v.Byte();
                tone.pan = v.Byte();
                // 中心音符 0-127
                tone.center = v.Byte();
                // 音高修正（0~127，音分单位）
                tone.shift = v.Byte();
                // 发送的 note 在 (min-max) 音区中, 则 tone 出声
                tone.min = v.Byte();
                tone.max = v.Byte();
                // 颤音宽度（1/128率，0~127）
                tone.vibw = v.Byte();
                // 颤音的1个循环时间（tick 刻度单位）
                tone.vibt = v.Byte();
                // 滑音宽度（1/128率，0~127）
                tone.porw = v.Byte();
                // portamento持有时间（tick 刻度单位）
                tone.port = v.Byte();
                // 弯音（-0~127,127 = 1倍频程）
                tone.pitch_bend_min = v.Byte();
                // 弯音（+ 0~127,127 = 1倍频程）
                tone.pitch_bend_max = v.Byte();
                tone.reserved1 = v.Byte();
                tone.reserved2 = v.Byte();
                tone.adsr1 = v.Ushort();
                tone.adsr2 = v.Ushort();
                tone.prog = v.Short();
                tone.vag = v.Short();
                tone.reserved3 = v.Short();
                tone.reserved4 = v.Short();
                tone.reserved5 = v.Short();
                tone.reserved6 = v.Short();

                prog prog = vab.prog[tone.prog];
                if (prog != null && c < prog.num_tones)
                {
                    prog.tone[c] = tone;
                    //debug("Tone", j, c, tone, "\n");
                }
            }
        }

        // http://problemkaputt.de/psx-spx.htm#spuadpcmsamples
        int table_off = v.getpos();
        int data_off = vagoff;
        //debug('vag pos', table_off);

        for (int i = 0; i <= vab.num_vags; ++i)
        {
            int size = v.Ushort(table_off) * 8;
            table_off += 2;
            if (size <= 0) continue;

            int[] vag = new int[1]; //v.build(Uint8Array, data_off, size);
            //debug(' = wav', i, data_off, size);
            data_off += size;
            vab.raw = ADPCMtoPCM(vag);

            // let wav = new Sound.Wav(core); // 测试: 轮播场景音效
            // wav.rawBuffer(vab.raw[i], samplerate, 1, audio.RAW_TYPE_16BIT);
            // debug("WAV", i); wav.play(); 
            // thread.wait(1000);
        }
        return vab;
    }

    public static List<int> ADPCMtoPCM(int[] srcbuf)
    {
        int[] pos_adpcm_table = new int[5];
        pos_adpcm_table[0] = 0;
        pos_adpcm_table[1] = 60;
        pos_adpcm_table[2] = 114;
        pos_adpcm_table[3] = 98;
        pos_adpcm_table[4] = 122;
        int[] neg_adpcm_table = new int[5];
        neg_adpcm_table[0] = 0;
        neg_adpcm_table[1] = 0;
        neg_adpcm_table[2] = -52;
        neg_adpcm_table[3] = -55;
        neg_adpcm_table[4] = -60;
        int old = 0, older = 0;
        int srci = 0, tmp, filter, flags;
        List<int> outt = new List<int>();

        while (srci < srcbuf.Length)
        {
            filter = srcbuf[srci++];
            int shift = filter & 0xf;
            filter >>= 4;

            flags = srcbuf[srci++];
            if ((flags & 1)==0) break;
            // if ((flags & 4) > 0)

            for (int i = 0; i < 28 / 2; ++i)
            {
                tmp = srcbuf[srci++];
                DecodeNibble(true, shift, filter, tmp);
                DecodeNibble(false, shift, filter, tmp);
            }
        }
        // debug(srcbuf.length, out.length)
        return outt; //Uint8Array

        int Signed4bit(int number)
        {
            if ((number & 0x8) == 0x8)
            {
                return (number & 0x7) - 8;
            }
            else
            {
                return number;
            }
        }

        int MinMax(int number, int min, int max)
        {
            if (number < min) return min;
            if (number > max) return max;
            return number;
        }

        int SignExtend(int s)
        {
            //if ((s & 0x8000 ) == 0) s = (int)0xffff0000; // |=
            return s;
        }

        void DecodeNibble(bool firstNibble, int shift, int filter, int d)
        {
            int f0 = (int)(pos_adpcm_table[filter] / 64.0f);
            int f1 = (int)(neg_adpcm_table[filter] / 64.0f);
            int s = firstNibble ? (d & 0x0f) << 12 : (d & 0xf0) << 8;
            s = SignExtend(s);
            int sample = (s >> shift) + old * f0 + older * f1;
            older = old;
            old = sample;
            int x = (int)(sample + 0.5f); //parseInt
            pushback(x & 0xFF);
            pushback(x >> 8);
        }

        void pushback(int x)
        {
            outt.Add(x);
        }
    }
}
