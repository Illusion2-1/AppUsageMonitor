using System;
using System.Data;
using System.Data.SQLite;

namespace AppUsageMonitor.Models;

public class Middleware : IMiddleware {
    private readonly Database _database;
    
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
    
    public double[] GetAppUsageByDaysInWeek(string appName, int year, int month, int day) {
        var date = new DateTime(year, month, day);
        const DayOfWeek firstDayOfWeek = DayOfWeek.Monday;

        // Calculate the start date of the week
        var daysToSubtract = (int)date.DayOfWeek - (int)firstDayOfWeek;
        daysToSubtract = daysToSubtract < 0 ? daysToSubtract + 7 : daysToSubtract; // Special handling for Sunday
        var startDateOfWeek = date.AddDays(-daysToSubtract);

        // Calculate the number of days from the start of the week to the specified date (inclusive)
        var numberOfDays = (date - startDateOfWeek).Days + 1;
    
        var usage = new double[numberOfDays + 1]; // Array starts from index 1
    
        for (var i = 1; i <= numberOfDays; i++) {
            // Calculate the date for the current iteration
            var currentDate = startDateOfWeek.AddDays(i - 1);
            // Get the usage in minutes and convert it to hours with one decimal place
            usage[i] = Math.Round(GetAppUsageByDay(appName, currentDate.Year, currentDate.Month, currentDate.Day) / 60.0, 1);
        }

        return usage;
    }

    public double[] GetAppUsageByDaysInMonth(string appName, int year, int month) {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var usage = new double[daysInMonth + 1]; // Array starts from index 1
    
        for (var day = 1; day <= daysInMonth; day++) {
            // Get the usage in minutes and convert it to hours with one decimal place
            usage[day] = Math.Round(GetAppUsageByDay(appName, year, month, day) / 60.0, 1);
        }

        return usage;
    }

    public double[] GetAppUsageByMonthsInYear(string appName, int year) {
        var usage = new double[13]; // Array starts from index 1
    
        for (var month = 1; month <= 12; month++) {
            // Get the usage in minutes and convert it to hours with one decimal place
            usage[month] = Math.Round(GetAppUsageByMonth(appName, year, month) / 60.0, 1);
        }

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