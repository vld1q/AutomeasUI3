using System.Collections.Generic;
using PseudoassemblyLanguage.Parser;
/* (co)-written by:
 * Kordian Czyżewski
 */
namespace PseudoassemblyLanguage.ScriptGenerator;
/// <summary>
/// Generates regular cycle (same step all the way)
/// </summary>
public static class Cycle
{
    public static string Step = "full";

    public static List<byte[]> Generate()
    {
        Macros parser = new();
        string command = $"mv {Step} 75l; mv {Step} 75r";
        List<byte[]> result = parser.parseLine(command);
        return result;
    }
}