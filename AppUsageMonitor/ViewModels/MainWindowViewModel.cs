using System.Reactive;
using AppUsageMonitor.Views;
using ReactiveUI;

namespace AppUsageMonitor.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public MainWindowViewModel() {
        ShowChartCommand = ReactiveCommand.Create<string>(OpenChartWindow);
    }

    public ReactiveCommand<string, Unit> ShowChartCommand { get; private set; }


    private void OpenChartWindow(string chartType) {
        var chartViewModel = new ChartWindowViewModel(chartType);
        var chartWindow = new ChartWindow {
            DataContext = chartViewModel
        };
        chartWindow.Show();
        chartWindow.Activate();
    }
}