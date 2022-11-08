using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Input;
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
        private ObservableCollection<ObservableValue?> _measuredValues = new(){new(1.14),null,null, null,null,null, null,null,null, null,null,null};
        private ObservableCollection<ObservableValue?> _measuredValues2 = new(){null,null,null, null,null,null, null,null,null, null,null,null};
        public RelayCommand CommenceExperimentCommand { get; set; }
        public void CommenceExperiment()
        {
            // 0. import relevant settings
            Dictionary<string, object> Settings = new()
            {
                {"step", ((Combobox)ConfigBar.Collumns["TypRuchuRight"][0]).GetValue()},
                {"repeats", ((Combobox)ConfigBar.Collumns["TypRuchuLeft"][1]).GetValue()}
                

            };
            List<byte[]> exe;
            {   // 1. Init connection with peripherials
                // Mcu mcu = new("COM10", 9600);
                //throw new NotImplementedException();
            }
            {   // 2.  Interpret & apply config
                PseudoassemblyLanguage.ScriptGenerator.Cycle.Step = (string)Settings["step"];
                exe = PseudoassemblyLanguage.ScriptGenerator.Cycle.Generate();
            }
            {   // 3. Execute
                for (int i = 0; i < Convert.ToUInt16((string)Settings["repeats"]); i++)
                {
                    /*
                     * mcu.Cycle(exe);
                     * gauge.SendRequest("3");
                     * _measuredValues.RemoveAt(0);
                     * _measuredValues.Add(gauge.GetMeasurement(MeasurementType.IlMeasDbm));
                     * gauge.SendRequest("5");
                     * _measuredValues.RemoveAt(0);
                     * _measuredValues1.Add(gauge.GetMeasurement(MeasurementType.IlMeasDbm));
                     */ 

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
            CommenceExperimentCommand = new RelayCommand((() => CommenceExperiment()));
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