using System;
using System.Collections.Generic;
using System.Linq;

namespace PseudoassemblyLanguage.Parser;

public class Macros
{
    internal readonly Dictionary<string, int> Keyword = new()
    {
        { "full", 0x2 },
        { "half", 0x4 },
        { "half_b", 0x8 },
        { "1/4", 0x6 },
        { "1/8", 0xA },
        { "1/16", 0xC },
        { "1/32", 0xE },
        { "pause", 0x0 }
    };

    internal readonly Dictionary<string, int> PseoudoAsmMnemonic = new()
    {
        { "mv", 0x0 },
        { "mvs", 0x1 },
        { "hlt", 0xF }
    };

    private int GetFirstOpcodeWord(int commandId, int movementMode) => (commandId << 4) | movementMode;

    public byte[] parseChunk(string chunk)
    {
        int result;
        {
            // 2. convert words into hex
            string mnemonic, step, value;
            {
                // 1. split  line into words
                List<string> split = new(chunk.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
                mnemonic = split[0];
                step = split[1];
                value = split[2];
            }
            int a, b, c;
            a = PseoudoAsmMnemonic[mnemonic];
            b = Keyword[step];
            if (value.Last() == 'r')
            {
                b++;
            }

            value = value.Substring(0, value.Length - 1);
            c = Convert.ToUInt16(value);
            result = GetFirstOpcodeWord(a, b);
            result = (result << 8) | c;
            var bb = new byte[2];
            bb = BitConverter.GetBytes(Convert.ToUInt16(result));
            {
                // swap values to get LittleEndian
                (bb[0], bb[1]) = (bb[1], bb[0]);
            }
            return bb;
        }
    }

    public List<byte[]> parseLine(string line)
    {
        List<byte[]> result = new();
        {
            // split into chunks
            List<string> chunks = new(line.Split(';'));
            chunks.Remove("");
            foreach (var chunk in chunks)
            {
                result.Add(parseChunk(chunk.Trim())); // append trimmed parsed chunk
            }
        }
        return result;
    }
}