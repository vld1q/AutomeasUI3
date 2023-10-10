using System;
using System.Collections.Generic;
using AutomeasUII.PseudoassemblyLanguage.Parser;

/* (co)-written by:
 * Kordian Czyżewski
 */
namespace PseudoassemblyLanguage.ScriptGenerator;

/// <summary>
///     Generates regular cycle (same step all the way)
/// </summary>
public static class Cycle{
    public static string Step = "full";
    public static int Interval = 0;
    public static int Repeat = 0;

    public static List<byte[]> Generate(){
        Macros parser = new();
        var command = $"mv {Step} 75l; mv {Step} 75r";
        var result = parser.ParseLine(command);
        return result;
    }

    public static List<byte[]> Generate(string step, int interval, int repeat){
        Macros parser = new();
        var cmd = $"mv {step} {interval}l";
        for (var i = 0; i < repeat - 1; i++) cmd += $";mv {step} {interval}l";
        for (var i = 0; i < repeat - 1; i++) cmd += $";mv {step} {interval}r";

        cmd += ";mv full 10r";
        cmd += ";mv pause 0r";
        var result = parser.ParseLine(cmd);
        return result;
    }

    public static List<byte[]> GenerateLeft(string step, int interval, int repeat){
        Macros parser = new();
        var cmd = $"mv {step} {interval}l";
        for (var i = 0; i < repeat - 1; i++) cmd += $";mv {step} {interval}l";
        cmd += ";mv pause 0r";
        var result = parser.ParseLine(cmd);
        return result;
    }

    public static List<byte[]> GenerateRight(string step, int interval, int repeat){
        Macros parser = new();
        var cmd = $"mv {step} {interval}r";
        for (var i = 0; i < repeat - 1; i++) cmd += $";mv {step} {interval}r";
        cmd += ";mv pause 0r";
        var result = parser.ParseLine(cmd);
        return result;
    }

    public static class Preset{
        public static Tuple<List<byte[]>, int> Fastest(string step){
            return new(Generate(step, 15, 2), 0);
        }

        public static Tuple<List<byte[]>, int> FullStepMidSpeed(string step){
            return new(Generate(step, 5, 5), 0);
        }

        public static Tuple<List<byte[]>, int> FullStepSlowSpeed(string step){
            return new(Generate(step, 1, 25), 0);
        }

        public static Tuple<List<byte[]>, int> HalfStepFullSpeed(string step){
            return new Tuple<List<byte[]>, int>(Generate(step, 50, 1), 0);
        }

        public static Tuple<List<byte[]>, int> HalfStepMidSpeed(string step){
            return new(Generate(step, 10, 5), 0);
        }

        public static Tuple<List<byte[]>, int> HalfStepSlowSpeed(string step){
            return new(Generate(step, 1, 50), 0);
        }
    }
}