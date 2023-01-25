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
using SkiaSharp;

namespace AutomeasUI.ViewModel;

public partial class DashboardViewModel : ObservableObject
{
    //private readonly Mcu _mcu = new Mcu("COM10",9600);
    //private readonly Gauge _gauge = new Gauge("COM5");
    public ObservableType<bool> isStartEnabled { get; set; }= new(true);
    public ObservableType<int> progressBarMax { get; set; } = new(1);
    public ObservableType<int> progressBarIndex { get; set; } = new(0);
    public ObservableType<Visibility> progressBarVisible { get; set; } = new(Visibility.Hidden);
    public ObservableType<string> Title { get; set; }
    public ObservableType<string> Subtitle { get; set; }
    public ObservableType<string> EstimatedTime { get; set; }

    private ObservableCollection<ObservableValue?> _measuredValues = new()
        { null, null, null, null, null, null, null, null, null, null, null, null };

    private ObservableCollection<ObservableValue?> _measuredValues2 = new()
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
        progressBarVisible.Value = Visibility.Visible;
        // [DOES NOT WORK] TODO verify that cancelling token really kills process
        // TODO autoscale graph
        // 0. import relevant settings
        Dictionary<string, object> Settings = new()
        {
            { "step", ((Combobox)ConfigBar.Collumns["TypRuchuRight"][0]).GetValue() },
            { "repeats", ((Combobox)ConfigBar.Collumns["TypRuchuLeft"][1]).GetValue() }
        };
        progressBarMax.Value = Convert.ToUInt16(Settings["repeats"]);
        List<byte[]> exe;
        Mcu mcu = new("COM5", 9600);
        mcu.Deactivate();
        Gauge gauge = new("COM6");
        Thread.Sleep(3000);
        {
            // 1. Init connection with peripherials

            //throw new NotImplementedException();
        }
        {
            // 2.  Interpret & apply config
            PseudoassemblyLanguage.ScriptGenerator.Cycle.Step = (string)Settings["step"];
            exe = PseudoassemblyLanguage.ScriptGenerator.Cycle.Generate();
        }
        {
            // 3. Execute
            {
                string line;
                string fileName = GenerateResultFileName(@"C:\Users\Kczyz\Desktop\AutomeasResults");
                double? result;
                
                for (int i = 0; i < Convert.ToUInt16((string)Settings["repeats"]); i++)
                {
                    gauge.Port.DiscardInBuffer();
                    gauge.Port.DiscardOutBuffer();
                    mcu.Cycle(exe);


                    {
                        
                        using (FileStream fs =
                               new FileStream(fileName,
                                   FileMode.Append, FileAccess.Write, FileShare.None))
                        using (StreamWriter fw = new StreamWriter(fs))
                        {
                            gauge.SendSafeRequest("3");
                            result = Convert.ToDouble(gauge.GetMeasurement(Program.MeasurementType.PowerMeasDbm),
                                CultureInfo.InvariantCulture);
                            _measuredValues.RemoveAt(0);
                            _measuredValues.Add(new(result));
                            line = $"{result.ToString()}\t 1300nm";
                            fw.WriteLine(line);
                            gauge.SendSafeRequest("5");
                            result = Convert.ToDouble(gauge.GetMeasurement(Program.MeasurementType.BrMeasDb),
                                CultureInfo.InvariantCulture);
                            AutoscaleGraph((double)result, 5.0);
                            _measuredValues2.RemoveAt(0);
                            _measuredValues2.Add(new(result));
                            line = $"{result.ToString()}\t 1500nm";
                            fw.WriteLine(line);
                        }

                        progressBarIndex.Value++;
                    }
                }
            }
        }
        isStartEnabled.Value = true;
        progressBarVisible.Value = Visibility.Hidden;
    }

    public void HaltExperiment()
    {
        this.tokenSource.Cancel();
        this.tokenSource.Dispose();
        this.isStartEnabled.Value = true;
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
        Title = new("Profil B");
        Subtitle = new("Pomiar nr. 7");
        EstimatedTime = new("1h 30m 15s");
        Series = new()
        {
            new LineSeries<ObservableValue?>()
            {
                Values = _measuredValues
            },
            new LineSeries<ObservableValue?>()
            {
                Values = _measuredValues2
            }
        };
        this.tokenSource = new CancellationTokenSource();
        CommenceExperimentCommand = new RelayCommand((() => Task.Run(() => CommenceExperiment(),this.tokenSource.Token)));
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