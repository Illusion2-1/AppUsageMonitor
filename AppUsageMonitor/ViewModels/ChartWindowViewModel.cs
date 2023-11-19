using System;
using AppUsageMonitor.Models;
using AppUsageMonitor.Views;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace AppUsageMonitor.ViewModels; 

public class ChartWindowViewModel {
    private readonly Monitor _getValues = MainWindow.Monitor;
    private readonly string _getProcess = MainWindow.SelectedProcess!;
    public ISeries[]? Series { get; private set; }

    public ChartWindowViewModel(string chartType) {
        LoadData(chartType);
    }
    public Axis[] XAxes { get; set; } = {
        new Axis {
            CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed, 1),
            CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
            Labeler = value => value.ToString("0")
        }
    };
    public Axis[] YAxes { get; set; } = {
        new Axis {
            CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed, 1),
            CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
            CrosshairSnapEnabled = true
        }
    };
    private void LoadData(string chartType) {
        // 根据chartType加载数据
        double[] values = GetDataBasedOnChartType(chartType);

        // 使用加载的数据初始化Series
        Series = new ISeries[] {
            new LineSeries<double> {
                Values = values,
                Fill = null
            }
        };
    }
    private double[] GetDataBasedOnChartType(string chartType) {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var day = DateTime.Now.Day;

        return chartType switch {
            "Daily" => InitializeArray(year, month),
            "Weekly" => InitializeArray(year, month, day),
            "Monthly" => InitializeArray(year),
            _ => throw new NotImplementedException(),
        };
    }

    public ChartWindowViewModel() {
    }
    private double[] InitializeArray(int year, int month) {
        return _getValues.GetAppUsageByDaysInMonth(_getProcess, year, month);
    }

    private double[] InitializeArray(int year, int month, int day) {
        return _getValues.GetAppUsageByDaysInWeek(_getProcess, year, month, day);
    }
    private double[] InitializeArray(int year) {
        return _getValues.GetAppUsageByMonthsInYear(_getProcess, year);
    }
}

