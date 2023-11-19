using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AppUsageMonitor.Models;

public class Monitor : Middleware {
    private readonly Middleware _middleware;
    private readonly int _updatePeriod;
    private readonly Dictionary<string, int> _buffer;

    // ReSharper disable once NotAccessedField.Local
    private Timer _timer;

    public Monitor(string databasePath) : base(databasePath) {
        _updatePeriod = 1;
        _middleware = new Middleware(databasePath);
        _buffer = new Dictionary<string, int>();
        _timer = new Timer(UpdateUsage, null, 0, 60000);
        ReloadApps();
    }

    public new void AddApp(string appName) {
        if (!_buffer.ContainsKey(appName)) {
            _middleware.AddApp(appName);
            _buffer[appName] = 0;
        }
    }

    public new void DeleteApp(string appName) {
        if (_buffer.ContainsKey(appName)) {
            _middleware.DeleteApp(appName);
            _buffer.Remove(appName);
        }
    }

    private void ReloadApps() {
        var apps = _middleware.GetAllApps();
        _buffer.Clear();

        foreach (DataRow row in apps.Rows) {
            var appName = row["Name"].ToString();
            _buffer[appName!] = 0;
        }
    }

    private void UpdateUsage(object? state) {
        foreach (var appName in _buffer.Keys) {
            if (IsProcessRunning(appName)) _buffer[appName]++;

            if (_buffer[appName] >= _updatePeriod) {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                var day = DateTime.Now.Day;
                var mins = _middleware.GetAppUsageByDay(appName, year, month, day) + 1;

                _middleware.UpdateAppUsage(appName, year, month, day, mins);
                _buffer[appName] = 0;
            }
        }
    }

    private bool IsProcessRunning(string appName) {
        return Process.GetProcesses().Any(process => process.ProcessName == appName);
    }
}