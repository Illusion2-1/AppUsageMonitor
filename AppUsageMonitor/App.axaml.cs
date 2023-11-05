using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AppUsageMonitor.ViewModels;
using AppUsageMonitor.Views;

namespace AppUsageMonitor;

public partial class App : Application {
    private MainWindow? _mainWindow;

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            _mainWindow = new MainWindow {
                DataContext = new MainWindowViewModel()
            };
            desktop.MainWindow = _mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void OnExitClick(object? sender, EventArgs eventArgs) {
        // 关闭所有窗口并退出应用程序
        Environment.Exit(0);
    }

    private void OnShowClick(object? sender, EventArgs e) {
        _mainWindow!.Show();
        _mainWindow!.Activate();
    }
}