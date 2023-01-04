using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
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
    public ObservableType<string> Title { get; set; }
        public ObservableType<string> Subtitle { get; set; }
        public ObservableType<string> EstimatedTime { get; set; }
        private ObservableCollection<ObservableValue?> _measuredValues = new(){null,null,null, null,null,null, null,null,null, null,null,null};
        private ObservableCollection<ObservableValue?> _measuredValues2 = new(){null,null,null, null,null,null, null,null,null, null,null,null};
        public RelayCommand CommenceExperimentCommand { get; set; }
        /// <summary>
        /// Execute instructions determined by the user via UI
        /// </summary>
        public void CommenceExperiment()
        {
            // 0. import relevant settings
            Dictionary<string, object> Settings = new()
            {
                {"step", ((Combobox)ConfigBar.Collumns["TypRuchuRight"][0]).GetValue()},
                {"repeats", ((Combobox)ConfigBar.Collumns["TypRuchuLeft"][1]).GetValue()}
                

            };
            List<byte[]> exe;
            Mcu mcu = new("COM5", 9600);
            mcu.Deactivate();
            Gauge gauge = new("COM6");
            Thread.Sleep(3000);
            {   // 1. Init connection with peripherials
                
                //throw new NotImplementedException();
            }
            {   // 2.  Interpret & apply config
                PseudoassemblyLanguage.ScriptGenerator.Cycle.Step = (string)Settings["step"];
                exe = PseudoassemblyLanguage.ScriptGenerator.Cycle.Generate();
            }
            {   // 3. Execute
                {
                    string line;
                    double? result;
                    for (int i = 0; i < Convert.ToUInt16((string)Settings["repeats"]); i++)
                    {
                        gauge.Port.DiscardInBuffer();
                        gauge.Port.DiscardOutBuffer();
                        mcu.Cycle(exe);
                        gauge.SendSafeRequest("3");
                        _measuredValues.RemoveAt(0);
                        result = Program.SingularMeasurement(gauge, Program.MeasurementType.IlMeasDbm);
                        
                        {
                            using ( FileStream fs = new FileStream(Path.Combine(@"C:\Users\Admin\Desktop", "measurements.txt"),FileMode.Append,FileAccess.Write,FileShare.None))
                            using (StreamWriter fw = new StreamWriter(fs))
                            {
                                line = $"{result.ToString()}\t 1300nm";
                               fw.WriteLine(line);
                               _measuredValues.Add(new(result));
                               gauge.SendSafeRequest("5");
                               _measuredValues2.RemoveAt(0);
                               result = Program.SingularMeasurement(gauge, Program.MeasurementType.IlMeasDbm);
                               line = $"{result.ToString()}\t 1500nm";
                               _measuredValues2.Add(new(result));
                               fw.WriteLine(line);
                            } 
                        }
                        

                    }    
                }
                
            }
            
        }
        
        public void HaltExperiment()
        {
            
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
            CommenceExperimentCommand = new RelayCommand((() => Task.Run(()=>CommenceExperiment()) ));
            OnPropertyChanged(nameof(CommenceExperimentCommand));
        }
        public Axis[] YAxes { get; set; } =
        {
        new()
        {
            MinLimit = 0,
            MaxLimit = 3,
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