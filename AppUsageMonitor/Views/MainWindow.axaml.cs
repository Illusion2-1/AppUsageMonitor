using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AppUsageMonitor.Models;
using AppUsageMonitor.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AppUsageMonitor.Views;

public partial class MainWindow : Window {
    public static readonly Monitor Monitor = new("AppUsage.db");
    public static string? SelectedProcess = new("");
    public MainWindow() {
        InitializeComponent();
        Features = this.FindControl<ListBox>("Features");
        ListAddProcess = this.FindControl<ListBox>("ListAddProcess");
        PanelAddProgram = this.FindControl<StackPanel>("PanelAddProgram");
        PanelViewThisMonth = this.FindControl<StackPanel>("PanelViewThisMonth");
        PanelSelectProgram = this.FindControl<StackPanel>("PanelSelectProgram");
        ListSelectProgram = this.FindControl<ListBox>("ListSelectProgram");
        ButtonAddProcess = this.FindControl<Button>("ButtonAddProcess");
        TodayUsageTextBlock = this.FindControl<TextBlock>("TodayUsageTextBlock");
        MonthUsageTextBlock = this.FindControl<TextBlock>("MonthUsageTextBlock");
        CurrentProgramTextBlock = this.FindControl<TextBlock>("CurrentProgramTextBlock");
        CreateProcessList();
        var viewModel = new MainWindowViewModel();
        DataContext = viewModel;


        Features!.SelectionChanged += ListBoxFeatures_SelectionChanged!;
        ListSelectProgram!.SelectionChanged += ListBox_SelectionChanged!;
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (e.AddedItems.Count > 0) {
            SelectedProcess = ListSelectProgram.SelectedItem?.ToString();
            ListSelectProgram.SelectedIndex = -1;
            Features.SelectedIndex = 1;
        }
    }

    private void ListBoxFeatures_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is ListBoxItem listBoxItem) ViewModel_SelectionChanged(listBoxItem.Name!);
    }

    private void ViewModel_SelectionChanged(string selectedItemName) {
        switch (selectedItemName) {
            case "SelectProgram":
                Console.WriteLine("选择程序 触发");
                PanelVisibilityHandler(0);
                break;
            case "ViewThisMonth":
                Console.WriteLine("查看当月使用情况 触发");
                PanelVisibilityHandler(1);
                break;
            case "AddProgram":
                Console.WriteLine("添加程序 触发");
                PanelVisibilityHandler(2);
                break;
        }
    }

    public void AddProcessButtonClicked(object source, RoutedEventArgs args) {
        Debug.Assert(ListAddProcess.SelectedItem != null, "AddProcessList.SelectedItem != null");
        Console.WriteLine(ListAddProcess.SelectedItem.ToString());
        if (IsAdductionSuccess(ListAddProcess.SelectedItem.ToString()))
            MessageBox.Show(this, "添加成功", "成功", MessageBox.MessageBoxButtons.Ok);
        else
            MessageBox.Show(this, "已经存在相同名程序", "失败", MessageBox.MessageBoxButtons.Ok);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
        Closing += MainWindow_OnClose;
    }

    private void PanelVisibilityHandler(int id) {
        HideAllPanel();
        switch (id) {
            case 0:
                PanelSelectProgram.IsVisible = true;
                ShowListSelectProgram();
                break;
            case 1:
                PanelViewThisMonth.IsVisible = true;
                CurrentProgramTextBlock.Text = SelectedProcess;
                TodayUsageTextBlock.Text = GetToday();
                MonthUsageTextBlock.Text = GetThisMonth();
                break;
            case 2:
                PanelAddProgram.IsVisible = true;
                CreateProcessList();
                break;
        }
    }

    private void HideAllPanel() {
        PanelSelectProgram.IsVisible = false;
        PanelViewThisMonth.IsVisible = false;
        PanelAddProgram.IsVisible = false;
    }

    private void CreateProcessList() {
        var processNameList = new ProcessNameList();
        ListAddProcess.ItemsSource = processNameList.ToStringArray();
    }

    private void ShowListSelectProgram() {
        ListSelectProgram.ItemsSource = GetAllAppsAsStringArray();
    }

    private bool IsAdductionSuccess(string? processName) {
        var rows = Monitor.GetAllApps().Select($"Name='{processName}'").ToArray();
        if (rows.Length > 0) return false;

        Monitor.AddApp(processName!);
        return true;
    }

    private string?[] GetAllAppsAsStringArray() {
        var dataTable = Monitor.GetAllApps();
        var resultArray = dataTable.AsEnumerable()
            .Select(row => row.Field<string>("Name"))
            .ToArray();
        return resultArray;
    }

    private string GetToday() {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var day = DateTime.Now.Day;
        return TimeParser.ConvertMinutesToTime(Monitor.GetAppUsageByDay(SelectedProcess!, year, month, day));
    }

    private string GetThisMonth() {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        return TimeParser.ConvertMinutesToTime(Monitor.GetAppUsageByMonth(SelectedProcess!, year, month));
    }

    private void MainWindow_OnClose(object? sender, CancelEventArgs e) {
        e.Cancel = true;
        Hide();
    }

    public void ShowWindow() {
        Show();
        Activate();
    }

    // ReSharper disable UnusedParameter.Local
    private void ButtonExport_OnClick(object? sender, RoutedEventArgs e) {
        var databaseToExcel = new DatabaseToExcelConverter();
        if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Data"))) Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Data")) ;
        databaseToExcel.Convert("AppUsage.db", Path.Combine(Environment.CurrentDirectory, "Data","AppUsage.xlsx"));
    }

    private void ButtonDelete_OnClick(object? sender, RoutedEventArgs e) {
        Monitor.DeleteApp(SelectedProcess!);
    }
}