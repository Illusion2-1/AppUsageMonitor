using System;
using System.Reactive;
using System.Runtime.InteropServices.ComTypes;
using AppUsageMonitor.Views;
using AppUsageMonitor.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using ReactiveUI;

namespace AppUsageMonitor.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public ReactiveCommand<string, Unit> ShowChartCommand { get; private set; }

    public MainWindowViewModel() {
        ShowChartCommand = ReactiveCommand.Create<string>(OpenChartWindow);
    }

    
    private void OpenChartWindow(string chartType) {
        var chartViewModel = new ChartWindowViewModel(chartType);
        var chartWindow = new ChartWindow {
            DataContext = chartViewModel
        };
        chartWindow.Show();
        chartWindow.Activate();
    }
}