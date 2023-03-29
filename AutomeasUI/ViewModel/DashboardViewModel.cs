using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutomeasAsyncCommunication;
using AutomeasToolkit.Basic.TitledList;
using AutomeasUI.Core;
using AutomeasUI.DevConfig;
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

namespace AutomeasUI.ViewModel;

public partial class DashboardViewModel : ObservableObject
{
    //private readonly Mcu _mcu = new Mcu("COM10",9600);
    //private readonly Gauge _gauge = new Gauge("COM5");
    public CancellationTokenSource Source = new CancellationTokenSource();
    public CancellationToken Token;
    private Thread _experiment;
    public ObservableBool isStartEnabled { get; set; }= new(true);
    public ObservableType<int> progressBarMax { get; set; } = new(1);
    public ObservableType<int> progressBarIndex { get; set; } = new(0);
    public ObservableType<Visibility> progressBarVisible { get; set; } = new(Visibility.Hidden);
    public ObservableType<string> Title { get; set; }
    public ObservableType<string> Subtitle { get; set; }
    public ObservableType<string> EstimatedTime { get; set; }

    private ObservableCollection<ObservableValue?> _trace1310 = new()
        { null, null, null, null, null, null, null, null, null, null, null, null };
    private ObservableCollection<ObservableValue?> _trace1550 = new()
        { null, null, null, null, null, null, null, null, null, null, null, null };

    public RelayCommand CommenceExperimentCommand { get; set; }
    public RelayCommand HaltExperimentCommand { get; set; }
    public CancellationTokenSource tokenSource;

    private void AutoscaleGraph(double measuredValue, double margin){
        if (measuredValue > YAxes[0].MaxLimit)
        {
            YAxes[0].MaxLimit = measuredValue + margin;
        }
        else if (measuredValue < YAxes[0].MinLimit)
        {
            YAxes[0].MinLimit = measuredValue - margin;
        }
    }

    private string LoadMeasPath(string path)
    {
        string today = DateTime.Today.ToString();
        { // parse current date
            today = today.Split(' ')[0];
            today = today.Replace('.', '-');
        }
        {
            // check if dir exists, if not init dir
            if (path.Last() != '\\')
            {
                path += "\\";
            }

            string dir = $"{path}{today}";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }
    }

    private string GenerateResultFileName(string path)
    {
        string ctime;
        int fileCount;
        { // get filename
            ctime = DateTime.Now.ToString("hh-mm");
            path = LoadMeasPath(path);
            fileCount = (from file in Directory.EnumerateFiles(path, "*.txt", SearchOption.TopDirectoryOnly)
                select file).Count(); 
        }
        string fileName = Path.Combine(path, $"results{fileCount}_time_{ctime}.txt");
        return fileName;
    }
    /// <summary>
    /// Execute instructions determined by the user via UI
    /// </summary>
    public void CommenceExperiment()
    {
        isStartEnabled.Value = false;
        bool TaskCancelled()
        {
            try
            {
                Token.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                Source = new CancellationTokenSource();
                Token = Source.Token;
                CommenceExperimentCommand = new RelayCommand((() => Task.Run(() => CommenceExperiment(),Token)));
                isStartEnabled.Value = true;
                return true;
            }

            return false;
        }
        Mcu mcu;
        Gauge gauge;
        bool VerifyDisplayErrorIfFails(Action operation, string title, string msg = "")
        {
            try
            {
                operation();
            }
            catch (Exception e)
            {
                Task.Run(() => ExceptionWindow.DisplayErrorBox(title, $"{msg} e.Message") );
                isStartEnabled.Value = true;
                return true;
            }

            return false;
        }

        string FailsafeMeasurementAlgorithm(Action gaugeSetup, Action mcuCycle, Program.MeasurementType mMode, int delayMs = 0)
        {
            gaugeSetup();
            mcuCycle();
            if (delayMs > 0) Thread.Sleep(delayMs);
            return gauge.GetMeasurement(mMode, 300);
        }
        // ------------------------------------------------------------------------------
        progressBarVisible.Value = Visibility.Visible;
        // 0. import relevant settings
        Dictionary<string, object> Settings = new()
        {
            { "step", ((Combobox)ConfigBar.Collumns["TypRuchuRight"][0]).GetValue() },
            { "repeats", ((Combobox)ConfigBar.Collumns["TypRuchuLeft"][1]).GetValue() },
            {"type", ((Combobox)ConfigBar.Collumns["PomiaryLeft"][0]).GetValue()}, 
        };
        progressBarMax.Value = Convert.ToUInt16(Settings["repeats"]);
        List<byte[]> exe;
        bool errors = false;
        errors |= VerifyDisplayErrorIfFails(() =>
        {
            string value = ((Combobox)ConfigBar.Collumns["TypRuchuRight"][1]).GetValue();
            Settings.Add("mcuCOM", value);
        }, "Serial ports for Mcu not selected");
        if(errors) return;
        errors|= VerifyDisplayErrorIfFails(() =>
        {
            string value = ((Combobox)ConfigBar.Collumns["TypRuchuRight"][2]).GetValue();
            Settings.Add("gaugeCOM", value);  
        },"Serial port for Gauge not selected");
        if(errors) return;
        try
        {
            mcu = new((string)Settings["mcuCOM"], 9600);
            gauge = new((string)Settings["gaugeCOM"]);
        }
        catch (Exception e)
        {
            ExceptionWindow.DisplayErrorBox("Could not connect to Mcu/Gauge serial",e.Message);
            isStartEnabled.Value = true;
            return;
        }
        Thread.Sleep(3000);
        Program.MeasurementType measMode;
        {
            var tp = (string)Settings["type"];
            measMode = (tp is "IL") ? Program.MeasurementType.IlMeasDbm :
                (tp is "BR") ? Program.MeasurementType.BrMeasDb :
                Program.MeasurementType.PowerMeasDbm;
        }
        gauge.SetMode(measMode);
        {
            // 2.  Interpret & apply config
            PseudoassemblyLanguage.ScriptGenerator.Cycle.Step = (string)Settings["step"];
            exe = PseudoassemblyLanguage.ScriptGenerator.Cycle.Generate();
        }
        {
            if (TaskCancelled()) return; // exit thread
            // 3. Execute
            {
                // 3.1 open file explorer, get file path.
                var dialog = new SaveFileDialog();
                dialog.Title = "Miejsce zapisu pomiarów";
                dialog.DefaultExt = ".csv";
                dialog.AddExtension = true;
                dialog.ShowDialog();
                string fileName = dialog.FileName;
                if (fileName is "") return; // exit thread
                if (TaskCancelled()) return; // exit thread
                // 3.2 start doing measurments
                int repeats = Convert.ToUInt16((string)Settings["repeats"]);
                
                using (FileStream fs =
                       new FileStream(fileName,
                           FileMode.Append, FileAccess.Write, FileShare.None))
                using (StreamWriter fw = new StreamWriter(fs))
                {
                    for (int i = 0; i < repeats; i++)
                    {
                        // meas1
                        var valueNm1310 = FailsafeMeasurementAlgorithm(() =>
                            {
                                //gauge.SetMode(Program.MeasurementType.BrMeasDb);
                                Thread.Sleep(500);
                                gauge.SetMode(Program.MeasurementType.Nm1310);
                            },
                            () =>
                            {
                                var exe = Cycle.Preset.FullStepSlowSpeed("full");
                                var fail = VerifyDisplayErrorIfFails(() => mcu.Cycle(exe.Item1, exe.Item2),
                                    "Mcu USART fail", "Mcu did not respond \'y\' to a command");
                                if (fail) return;
                            }, measMode, 0);
                        var result1310 = Convert.ToDouble(valueNm1310, CultureInfo.InvariantCulture);
                        if (TaskCancelled()) return; // exit thread
                        // meas2
                        var valueNm1550 = FailsafeMeasurementAlgorithm(() =>
                            {
                                //gauge.SetMode(Program.MeasurementType.IlMeasDbm);
                                Thread.Sleep(500);
                                gauge.SetMode(Program.MeasurementType.Nm1550);
                            },
                            () =>
                            {
                                var exe = Cycle.Preset.FullStepSlowSpeed("full");
                                var fail = VerifyDisplayErrorIfFails(() => mcu.Cycle(exe.Item1, exe.Item2),
                                    "Mcu USART fail", "Mcu did not respond \'y\' to a command");
                                if(fail) return;
                            }, measMode, 0);
                        var result1550 = Convert.ToDouble(valueNm1550, CultureInfo.InvariantCulture);
                        if (TaskCancelled()) return; // exit thread
                        // sync both trends at the same time
                        _trace1310.RemoveAt(0); 
                        _trace1310.Add(new(result1310));
                        _trace1310.RemoveAt(0);
                        _trace1310.Add(new(result1550));
                        progressBarIndex.Value++;
                        if (TaskCancelled()) return; // exit thread
                    }
                }
            }
        }
        isStartEnabled.Value = true;
        progressBarVisible.Value = Visibility.Hidden;
    }

    public void HaltExperiment()
    {
        Source.Cancel();
    }

    public ObservableCollection<ISeries> Series { get; set; }

    public Axis[] XAxes { get; set; } =
    {
        new()
        {
            ForceStepToMin = true,
            MinStep = 1,
            TextSize = 14,
            SeparatorsPaint = new SolidColorPaint
            {
                Color = SKColors.Gray,
                StrokeThickness = 2
            }
        }
    };

    private readonly object _lock = new object();
    private readonly object _lock2 = new object();
    public DashboardViewModel()
    {
        Task.Run(() =>
        {
            while (true)
            {
                Thread.Sleep(2500);
                var ports = SerialPort.GetPortNames();

                ((Combobox)ConfigBar.Collumns["TypRuchuRight"][1]).Content = new ObservableCollection<string>(ports);
                ((Combobox)ConfigBar.Collumns["TypRuchuRight"][2]).Content = new ObservableCollection<string>(ports);
                
            }

        });
        Title = new("Profil B");
        Subtitle = new("Pomiar nr. 7");
        EstimatedTime = new("1h 30m 15s");
        Series = new()
        {
            new LineSeries<ObservableValue?>()
            {
                Values = _trace1310
            },
            new LineSeries<ObservableValue?>()
            {
                Values = _trace1550
            }
        };
        this.tokenSource = new CancellationTokenSource();
        Token = Source.Token;
        CommenceExperimentCommand = new RelayCommand((() => Task.Run(() => CommenceExperiment(),Token)));
        HaltExperimentCommand = new RelayCommand(() => HaltExperiment());
        OnPropertyChanged(nameof(CommenceExperimentCommand));
    }

    public Axis[] YAxes { get; set; } =
    {
        new()
        {
            MinLimit = 0,
            MaxLimit = 60,
            ForceStepToMin = true,
            MinStep = 1,
            TextSize = 14,
            SeparatorsPaint = new SolidColorPaint
            {
                Color = SKColors.Gray,
                StrokeThickness = 2,
                PathEffect = new DashEffect(new float[] { 3, 3 })
            }
        }
    };

    public DrawMarginFrame Frame { get; set; } =
        new()
        {
            Fill = new SolidColorPaint
            {
                Color = new(0, 0, 0, 30)
            },
            Stroke = new SolidColorPaint
            {
                Color = new(80, 80, 80),
                StrokeThickness = 2
            }
        };
}
