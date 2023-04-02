using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutomeasUII.AutomeasToolkit.Basic.TitledList;
using AutomeasUII.Core;
using AutomeasUII.DevConfig;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using Microsoft.Win32;
using PseudoassemblyLanguage.ScriptGenerator;
using SkiaSharp;
namespace AutomeasUII.ViewModel;

public class DashboardViewModel : ObservableObject{
    
    private readonly ObservableCollection<ObservableValue?> _trace1310 = new()
        { null, null, null, null, null, null, null, null, null, null, null, null };

    private readonly ObservableCollection<ObservableValue?> _trace1550 = new()
        { null, null, null, null, null, null, null, null, null, null, null, null };

    public CancellationTokenSource Source = new();
    public CancellationToken Token;

    public DashboardViewModel(){
        Task.Run(() => {
            while (true){
                Thread.Sleep(2500);
                var ports = SerialPort.GetPortNames();

                ((Combobox)ConfigBar.Collumns["TypRuchuRight"][1]).Content = new ObservableCollection<string>(ports);
                ((Combobox)ConfigBar.Collumns["TypRuchuRight"][2]).Content = new ObservableCollection<string>(ports);
            }
            
        });
        Series = new ObservableCollection<ISeries>{
            new LineSeries<ObservableValue?>{
                Values = _trace1310
            },
            new LineSeries<ObservableValue?>{
                Values = _trace1550
            }
        };
        Token = Source.Token;
        CommenceExperimentCommand = new RelayCommand(() => Task.Run(() => CommenceExperiment(), Token));
        HaltExperimentCommand = new RelayCommand(() => HaltExperiment());
        OnPropertyChanged(nameof(CommenceExperimentCommand));
    }

    public ObservableBool IsStartEnabled{ get; set; } = new(true);
    public ObservableType<int> ProgressBarMax{ get; set; } = new(1);
    public ObservableType<int> ProgressBarIndex{ get; set; } = new(0);
    public ObservableType<Visibility> ProgressBarVisible{ get; set; } = new(Visibility.Hidden);

    public RelayCommand CommenceExperimentCommand{ get; set; }
    public RelayCommand HaltExperimentCommand{ get; set; }

    public ObservableCollection<ISeries> Series{ get; set; }

    public Axis[] XAxes{ get; set; } ={
        new(){
            ForceStepToMin = true,
            MinStep = 1,
            TextSize = 14,
            SeparatorsPaint = new SolidColorPaint{
                Color = SKColors.Gray,
                StrokeThickness = 2
            }
        }
    };

    public Axis[] YAxes{ get; set; } ={
        new(){
            MinLimit = 0,
            MaxLimit = 1,
            ForceStepToMin = true,
            MinStep = 1,
            TextSize = 14,
            SeparatorsPaint = new SolidColorPaint{
                Color = SKColors.Gray,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[]{ 3, 3 })
            }
        }
    };

    public DrawMarginFrame Frame{ get; set; } =
        new(){
            Fill = new SolidColorPaint{
                Color = new SKColor(0, 0, 0, 30)
            },
            Stroke = new SolidColorPaint{
                Color = new SKColor(80, 80, 80),
                StrokeThickness = 2
            }
        };
    

    /// <summary>
    ///     Execute instructions determined by the user via UI
    /// </summary>
    public void CommenceExperiment(){
        Mcu mcu;
        Gauge gauge;
        IsStartEnabled.Value = false;
        ProgressBarIndex.Value = 0;
        ProgressBarVisible.Value = Visibility.Visible;
        #region auxillary functions
        bool TaskCancelled(){
            try{
                Token.ThrowIfCancellationRequested();
            }
            catch (Exception){
                Source = new CancellationTokenSource();
                Token = Source.Token;
                CommenceExperimentCommand = new RelayCommand(() => Task.Run(() => CommenceExperiment(), Token));
                IsStartEnabled.Value = true;
                return true;
            }

            return false;
        }
        bool VerifyDisplayErrorIfFails(Action operation, string title, string msg = ""){
            try{
                operation();
            }
            catch (Exception){
                Task.Run(() => ExceptionWindow.DisplayErrorBox(title, $"{msg} e.Message"));
                IsStartEnabled.Value = true;
                return true;
            }

            return false;
        }

        void AutoScaleGraph(){
            double min = 0;
            double max = 0;
            //first cycle
            for (var i = _trace1310.Count - 1; i < _trace1310.Count; i++){
                var a = _trace1310[i];
                var b = _trace1550[i];
                if (a == null || b == null) return;
                if (a.Value == null || b.Value == null) return;
                var aa = (double)a.Value;
                var bb = (double)b.Value;
                min = aa < bb ? aa : bb;
                max = aa > bb ? aa : bb;
            }

            for (var i = _trace1310.Count - 2; i >= 0; i--){
                var a = _trace1310[i];
                var b = _trace1550[i];
                if (a == null || b == null) continue;
                if (a.Value == null || b.Value == null) continue;
                var aa = (double)a.Value;
                var bb = (double)b.Value;
                var localMin = aa < bb ? aa : bb;
                var localMax = aa > bb ? aa : bb;
                min = localMin < min ? localMin : min;
                max = localMax > max ? localMax : max;
            }

            YAxes[0].MinLimit = min - 2;
            YAxes[0].MaxLimit = max + 2;
        }

        string FailsafeMeasurementAlgorithm(Action gaugeSetup, Action mcuCycle, Program.MeasurementType mMode,
            int delayMs = 0){
            gaugeSetup();
            mcuCycle();
            if (delayMs > 0) Thread.Sleep(delayMs);
            return gauge.GetMeasurement(mMode, 300);
        }
        #endregion
        // ------------------------------------------------------------------------------
        #region get all settings from ui
        Dictionary<string, object> settings = new(){
            { "step", ((Combobox)ConfigBar.Collumns["TypRuchuRight"][0]).GetValue() },
            { "repeats", ((Combobox)ConfigBar.Collumns["TypRuchuLeft"][2]).GetValue() }
        };
        var measSettings = new MeasSequence(){
            Lambda1310 = new(((CheckboxCollection)ConfigBar.Collumns["TypRuchuLeft"][1]).Content[0].Checked),
            Lambda1550 = new(((CheckboxCollection)ConfigBar.Collumns["TypRuchuLeft"][1]).Content[1].Checked),
            MeasIL = new(((CheckboxCollection)ConfigBar.Collumns["TypRuchuLeft"][0]).Content[0].Checked),
            MeasBR = new(((CheckboxCollection)ConfigBar.Collumns["TypRuchuLeft"][0]).Content[1].Checked),
            MeasPower = new(((CheckboxCollection)ConfigBar.Collumns["TypRuchuLeft"][0]).Content[2].Checked),
        };
        measSettings.GenerateLists();
        ProgressBarMax.Value = Convert.ToUInt16(settings["repeats"]);
        #endregion
        #region check for errors
        var errors = false;
        errors |= VerifyDisplayErrorIfFails(() => measSettings.CheckLambdas(), "No lambda has been selected");
        errors |= VerifyDisplayErrorIfFails(() => measSettings.CheckMeasModes(), "No meas mode has been selected");
        if (errors) return;
        errors |= VerifyDisplayErrorIfFails(() => {
            var value = ((Combobox)ConfigBar.Collumns["TypRuchuRight"][1]).GetValue();
            settings.Add("mcuCOM", value);
        }, "Serial ports for Mcu not selected");
        if (errors) return;
        errors |= VerifyDisplayErrorIfFails(() => {
            var value = ((Combobox)ConfigBar.Collumns["TypRuchuRight"][2]).GetValue();
            settings.Add("gaugeCOM", value);
        }, "Serial port for Gauge not selected");
        if (errors) return;
        try{
            mcu = new Mcu((string)settings["mcuCOM"], 9600);
            gauge = new Gauge((string)settings["gaugeCOM"]);
        }
        catch (Exception e){
            ExceptionWindow.DisplayErrorBox("Could not connect to Mcu/Gauge serial", e.Message);
            IsStartEnabled.Value = true;
            return;
        }
        #endregion
        {
            string fileName;
            #region get fileName
            { // 1. get target directory
                if (TaskCancelled()){
                    mcu.Port.Close();
                    gauge.Port.Close();
                    return;
                } // exit thread

                {
                    // 3.1 open file explorer, get file path.
                    var dialog = new SaveFileDialog();
                    dialog.Title = "Miejsce zapisu pomiarów";
                    dialog.DefaultExt = ".csv";
                    dialog.AddExtension = true;
                    dialog.ShowDialog();
                    fileName = dialog.FileName;
                    if (fileName is ""){
                        IsStartEnabled.Value = true;
                        mcu.Port.Close();
                        gauge.Port.Close();
                        return; // exit thread
                    }

                    if (TaskCancelled()){
                        mcu.Port.Close();
                        gauge.Port.Close();
                        return;
                    } // exit thread
                }
            }
            #endregion
            int repeats = Convert.ToUInt16((string)settings["repeats"]);
            using (var fs =
                   new FileStream(fileName,
                       FileMode.Append, FileAccess.Write, FileShare.None))
            using (var fw = new StreamWriter(fs)){
                foreach (var mode in measSettings.Modes){
                    gauge.SetMode(mode);
                    for (var i = 0; i < _trace1310.Count; i++){ // clear trace
                        _trace1310[i] = null;
                    }

                    Thread.Sleep(3000);
                    if (measSettings.Lambda1310.Value == true){
                        fw.WriteLine("");
                        fw.WriteLine($"Lambda 1310nm;{nameof(mode)}");
                        fw.WriteLine($"[No];[Value]");
                        gauge.SetMode(Program.MeasurementType.Nm1310);
                        Thread.Sleep(3000);
                        for (int i = 0; i < repeats; i++){
                            // meas
                            #region meas algorithm
                            var valueNm1310 = FailsafeMeasurementAlgorithm(() => {
                                    return;
                                },
                            () => {
                                var step = (string)settings["step"];
                                var interval = step is "full" ? 30 : 60;
                                var left = Cycle.GenerateLeft(step, interval, 0);
                                var right = Cycle.GenerateRight(step, interval, 0);
                                var exe = Cycle.Preset.FullStepMidSpeed(step);
                                var fail = false;
                                switch (step){
                                    case "half":
                                    case "full":
                                        fail = VerifyDisplayErrorIfFails(() => mcu.Cycle(left, right, 500),
                                            "Mcu USART fail", "Mcu did not respond \'y\' to a command");
                                        break;
                                    case "half_b":
                                        fail = VerifyDisplayErrorIfFails(() => mcu.Cycle(exe.Item1, exe.Item2),
                                            "Mcu USART fail", "Mcu did not respond \'y\' to a command");
                                        break;
                                    default:
                                        fail = true;
                                        break;
                                }

                                if (fail) return;
                            }, mode);
                            var result1310 = Convert.ToDouble(valueNm1310, CultureInfo.InvariantCulture);
                            _trace1310.RemoveAt(0);
                            _trace1310.Add(new ObservableValue(result1310));
                            fw.WriteLine($"{i};{result1310}");
                            AutoScaleGraph();
                            if (TaskCancelled()){
                                mcu.Port.Close();
                                gauge.Port.Close();
                                return;
                            } // exit thread
                            #endregion

                        }
                    }

                    if (measSettings.Lambda1550.Value == true){
                        gauge.SetMode(Program.MeasurementType.Nm1550);
                        Thread.Sleep(3000);
                        for (int i = 0; i < repeats; i++){
                            // meas
                            #region meas algorithm
                            var valueNm1310 = FailsafeMeasurementAlgorithm(() => {
                                    return;
                                },
                            () => {
                                var step = (string)settings["step"];
                                var interval = step is "full" ? 30 : 60;
                                var left = Cycle.GenerateLeft(step, interval, 0);
                                var right = Cycle.GenerateRight(step, interval, 0);
                                var exe = Cycle.Preset.FullStepMidSpeed(step);
                                var fail = false;
                                switch (step){
                                    case "half":
                                    case "full":
                                        fail = VerifyDisplayErrorIfFails(() => mcu.Cycle(left, right, 500),
                                            "Mcu USART fail", "Mcu did not respond \'y\' to a command");
                                        break;
                                    case "half_b":
                                        fail = VerifyDisplayErrorIfFails(() => mcu.Cycle(exe.Item1, exe.Item2),
                                            "Mcu USART fail", "Mcu did not respond \'y\' to a command");
                                        break;
                                    default:
                                        fail = true;
                                        break;
                                }

                                if (fail) return;
                            }, mode);
                            var result1310 = Convert.ToDouble(valueNm1310, CultureInfo.InvariantCulture);
                            _trace1310.RemoveAt(0);
                            _trace1310.Add(new ObservableValue(result1310));
                            fw.WriteLine($"{i};{result1310}");
                            AutoScaleGraph();
                            if (TaskCancelled()){
                                mcu.Port.Close();
                                gauge.Port.Close();
                                return;
                            } // exit thread
                            #endregion
                        }
                    }
                }
            }
        }
        // end
        IsStartEnabled.Value = true;
        ProgressBarVisible.Value = Visibility.Hidden;
        mcu.Port.Close();
        gauge.Port.Close();
    }

    public void HaltExperiment(){
        Source.Cancel();
    }
}

public class MeasSequence{
    public ObservableBool Lambda1310{ get; set; } = new(false);
    public ObservableBool Lambda1550{ get; set; } = new(false);
    public ObservableBool MeasPower{ get; set; } = new(false);
    public ObservableBool MeasBR{ get; set; } = new(false);
    public ObservableBool MeasIL{ get; set; } = new(false);
    public readonly List<Program.MeasurementType> Lambdas = new();
    public readonly List<Program.MeasurementType> Modes = new();
    public void GenerateLists(){
        // lambda
        if (Lambda1550.Value == true) Lambdas.Add(Program.MeasurementType.Nm1550);
        if (Lambda1310.Value == true) Lambdas.Add(Program.MeasurementType.Nm1310);
        // meas mode
        if (MeasIL.Value == true) Modes.Add(Program.MeasurementType.IlMeasDbm);
        if (MeasBR.Value == true) Modes.Add(Program.MeasurementType.BrMeasDb);
        if (MeasPower.Value == true) Modes.Add(Program.MeasurementType.PowerMeasDbm);
    }

    public void CheckLambdas(){
        bool anyLambdaSet = (Lambda1310.Value ?? false | Lambda1550.Value ?? false);
        if (anyLambdaSet == false){
            throw new Exception("No value selected");
        }
    }

    public void CheckMeasModes(){
        bool anyModeSet = (MeasIL.Value ?? false | MeasBR.Value ?? false | MeasPower.Value ?? false);
        if (anyModeSet == false){
            throw new Exception("No value selected");
        }
    }
    public void ClearAll(){
        Lambda1550.Value = false;
        Lambda1550.Value = false;
        MeasPower.Value = false;
        MeasBR.Value = false;
        MeasIL.Value = false;
    }
}