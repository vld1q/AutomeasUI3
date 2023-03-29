using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public static int Interval = 0;
    public static int Repeat = 0;

    public static List<byte[]> Generate()
    {
        Macros parser = new();
        string command = $"mv {Step} 75l; mv {Step} 75r";
        List<byte[]> result = parser.parseLine(command);
        return result;
    }

    public static List<byte[]> Generate(string step, int interval, int repeat)
    {
        Macros parser = new();
        string cmd = $"mv {step} {interval}l";
        for (int i = 0; i < repeat-1; i++)
        {
            cmd += $";mv {step} {interval}l";
        }
        for (int i = 0; i < repeat; i++)
        {
            cmd += $";mv {step} {interval}r";
        }
        List<byte[]> result = parser.parseLine(cmd);
        return result;
    }

    public static class Preset
    {
        public static Tuple<List<byte[]>, int> Fastest(string step) =>
        new Tuple<List<byte[]>, int>(Cycle.Generate(step, 25, 1), 0);
        public static Tuple<List<byte[]>, int> FullStepMidSpeed(string step) =>
            new Tuple<List<byte[]>, int>(Cycle.Generate(step, 5, 5), 100);
        public static Tuple<List<byte[]>, int> FullStepSlowSpeed(string step) =>
            new Tuple<List<byte[]>, int>(Cycle.Generate(step, 1, 25), 100);
        public static Tuple<List<byte[]>, int> HalfStepFullSpeed(string step) =>
            new Tuple<List<byte[]>, int>(Cycle.Generate(step, 50, 1), 0);
        public static Tuple<List<byte[]>, int> HalfStepMidSpeed(string step) =>
            new Tuple<List<byte[]>, int>(Cycle.Generate(step, 10, 5), 100);
        public static Tuple<List<byte[]>, int> HalfStepSlowSpeed(string step) =>
            new Tuple<List<byte[]>, int>(Cycle.Generate(step, 1, 50), 100);
    }
}