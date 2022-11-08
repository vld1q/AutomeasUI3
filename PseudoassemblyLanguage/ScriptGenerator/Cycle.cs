using System.Collections.Generic;
using PseudoassemblyLanguage.Parser;

namespace PseudoassemblyLanguage.ScriptGenerator;

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