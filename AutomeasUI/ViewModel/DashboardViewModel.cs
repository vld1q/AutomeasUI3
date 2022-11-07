using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading.Tasks;
using AutomeasUI.Core;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;

namespace AutomeasUI.ViewModel;

public partial class DashboardViewModel
{
    //private readonly Mcu _mcu = new Mcu("COM10",9600);
    //private readonly Gauge _gauge = new Gauge("COM5");
    public ObservableType<string> Title { get; set; }
        public ObservableType<string> Subtitle { get; set; }
        public ObservableType<string> EstimatedTime { get; set; }
        private ObservableCollection<ObservableValue?> _measuredValues = new(){null,null,null, null,null,null, null,null,null, null,null,null};
        private ObservableCollection<ObservableValue?> _measuredValues2 = new(){null,null,null, null,null,null, null,null,null, null,null,null};

        [RelayCommand]
        public void CommenceExperiment()
        {
            
        }

        [RelayCommand]
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