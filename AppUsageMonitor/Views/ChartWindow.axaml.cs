using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AppUsageMonitor.Views; 

public partial class ChartWindow : Window {
    public ChartWindow() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}