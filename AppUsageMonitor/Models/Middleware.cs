using System;
using System.Data;
using System.Data.SQLite;

namespace AppUsageMonitor.Models;

public class Middleware : IMiddleware {
    private Database _database;


    public Middleware(string databasePath) {
        _database = new Database(databasePath);
    }

    public int GetAppUsageByDay(string appName, int year, int month, int day) {
        var id = GetAppId(appName);
        var dataTable = _database.GetAppUsageByDay(id, year, month, day);
        return dataTable.Rows.Count > 0 ? Convert.ToInt32(dataTable.Rows[0][0]) : 0;
    }

    public int GetAppUsageByMonth(string appName, int year, int month) {
        var id = GetAppId(appName);
        var dataTable = _database.GetAppUsageByMonth(id, year, month);
        return dataTable.Rows.Count > 0 ? Convert.ToInt32(dataTable.Rows[0][0]) : 0;
    }

    public int[] GetAppUsageByDaysInMonth(string appName, int year, int month) {
        var usage = new int[DateTime.DaysInMonth(year, month)];
        for (var day = 1; day <= usage.Length; day++) usage[day - 1] = GetAppUsageByDay(appName, year, month, day);

        return usage;
    }

    public int[] GetAppUsageByMonthsInYear(string appName, int year) {
        var usage = new int[12];
        for (var month = 1; month <= usage.Length; month++) usage[month - 1] = GetAppUsageByMonth(appName, year, month);

        return usage;
    }

    public void AddApp(string appName) {
        var id = GetNextAppId();
        _database.AddApp(id, appName);
    }

    public void DeleteApp(string appName) {
        var id = GetAppId(appName);
        _database.DeleteApp(id);
    }

    private int GetAppId(string appName) {
        var query = "SELECT ID FROM App WHERE Name = @name;";

        using var command = new SQLiteCommand(query, _database.Connection);
        command.Parameters.AddWithValue("@name", appName);

        using var reader = command.ExecuteReader();
        return reader.Read() ? reader.GetInt32(0) : -1;
    }

    private int GetNextAppId() {
        var query = "SELECT MAX(ID) FROM App;";

        using var command = new SQLiteCommand(query, _database.Connection);
        using var reader = command.ExecuteReader();
        return reader.Read() && !reader.IsDBNull(0) ? reader.GetInt32(0) + 1 : 1;
    }

    public void UpdateAppUsage(string appName, int year, int month, int day, int minutes) {
        _database.UpdateAppUsage(GetAppId(appName), year, month, day, minutes);
    }

    public DataTable GetAllApps() {
        return _database.GetAllApps();
    }
}