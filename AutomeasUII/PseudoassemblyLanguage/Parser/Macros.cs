using System;
using System.Collections.Generic;
using System.Linq;

/* (co)-written by:
 * Kordian Czyżewski
 */
namespace AutomeasUII.PseudoassemblyLanguage.Parser;

/// <summary>
///     Pseudoassembly to hex translator
/// </summary>
public class Macros{
    /// <summary>
    ///     Translates step names into corresponding pin configurations
    ///     Provided command type is some kind of move, those 4 bits are directly mapped onto MCU outputs
    ///     M3,M2,M1 (three oldest bits) configure step type
    ///     Youngest bit determines direction. By default all moves are counter-clockwise.
    ///     In order to change direction value obtained from dictionary has to be incremented by one
    ///     <example>
    ///         for step 1/4:
    ///         0x6 = move left
    ///         0x7 = move right
    ///     </example>
    /// </summary>
    internal readonly Dictionary<string, int> Keyword = new(){
        { "full", 0x2 },
        { "half", 0x4 },
        { "half_b", 0x8 },
        { "1/4", 0x6 },
        { "1/8", 0xA },
        { "1/16", 0xC },
        { "1/32", 0xE },
        { "pause", 0x0 }
    };

    /// <summary>
    ///     determines instruction type.
    ///     overall syntax is [type: 4bit][arg1: 4bit][arg2: 8bit]
    ///     <example>
    ///         mv half 75r
    ///         hlt
    ///         mvs 1/32 25l
    ///     </example>
    /// </summary>
    internal readonly Dictionary<string, int> PseoudoAsmMnemonic = new(){
        { "mv", 0x0 },
        { "mvs", 0x1 },
        { "hlt", 0xF }
    };

    /// <summary>
    ///     Function merges two half-bytes into one word (8bit)
    ///     Applicable only for movement type command
    ///     Target is 8bit AVR MCU. (ATmega328P in particular) so the word is one byte (8 bits)
    ///     Opcode consists of two words, first word is separated into two half-bytes
    /// </summary>
    /// <param name="commandId"> Determines command type, determined by <c>PseudoAsmMnemonic</c> </param>
    /// <param name="movementMode"> Determines movement mode, determined by <c>Keyword</c></param>
    /// <returns></returns>
    private int GetFirstOpcodeWord(int commandId, int movementMode){
        return (commandId << 4) | movementMode;
    }

    /// <summary>
    ///     Translate one command (string terminated with ";") into 2byte opcode
    /// </summary>
    /// <param name="chunk"> Command</param>
    /// <returns>byte[2]</returns>
    public byte[] ParseChunk(string chunk){
        int result;
        {
            // 2. convert words into hex
            string mnemonic, step, value;
            {
                // 1. split  line into words
                List<string> split = new(chunk.Split(new[]{ ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
                mnemonic = split[0];
                step = split[1];
                value = split[2];
            }
            int a, b, c;
            a = PseoudoAsmMnemonic[mnemonic];
            b = Keyword[step];
            if (value.Last() == 'r') b++;

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

    /// <summary>
    ///     Translates sets of commands (ie. program) into list of corresponding opcodes
    /// </summary>
    /// <param name="line"> program or set of commands to be translated</param>
    /// <returns> list of byte[2]</returns>
    public List<byte[]> ParseLine(string line){
        List<byte[]> result = new();
        {
            // split into chunks
            List<string> chunks = new(line.Split(';'));
            chunks.Remove("");
            foreach (var chunk in chunks) result.Add(ParseChunk(chunk.Trim())); // append trimmed parsed chunk
        }
        return result;
    }
}