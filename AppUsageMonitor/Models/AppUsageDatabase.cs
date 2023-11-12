using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace AppUsageMonitor.Models;

public class Database : IAppUsageDatabase {
    public readonly SQLiteConnection Connection;
    private readonly string _sqlQueryCommandPath=Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QueryCommands.sql");
    public Database(string databasePath) {
        Connection = new SQLiteConnection($"Data Source={databasePath};Version=3;");
        Connection.Open();
        using (var command = new SQLiteCommand(Query("CreateAppTableQuery"), Connection)) {
            command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(Query("CreateUsageTableQuery"), Connection)) {
            command.ExecuteNonQuery();
        }
    }

    public void AddApp(int id, string name) {

        using var command = new SQLiteCommand(Query("AddApp"), Connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", name);

        command.ExecuteNonQuery();
    }

    public void DeleteApp(int id) {

        using var command = new SQLiteCommand(Query("DeleteApp"), Connection);
        command.Parameters.AddWithValue("@id", id);

        command.ExecuteNonQuery();
    }

    public DataTable GetAppUsageByDay(int id, int year, int month, int day) {

        using var command = new SQLiteCommand(Query("GetAppUsageByDay"), Connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@year", year);
        command.Parameters.AddWithValue("@month", month);
        command.Parameters.AddWithValue("@day", day);

        using var adapter = new SQLiteDataAdapter(command);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);
        return dataTable;
    }

    public DataTable GetAppUsageByMonth(int id, int year, int month) {
        
        using var command = new SQLiteCommand(Query("GetAppUsageByMonth"), Connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@year", year);
        command.Parameters.AddWithValue("@month", month);

        using var adapter = new SQLiteDataAdapter(command);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);
        return dataTable;
    }

    public void UpdateAppUsage(int id, int year, int month, int day, int minutes) {

        using var command = new SQLiteCommand(Query("UpdateAppUsage"), Connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@year", year);
        command.Parameters.AddWithValue("@month", month);
        command.Parameters.AddWithValue("@day", day);
        command.Parameters.AddWithValue("@minutes", minutes);

        var rowsAffected = command.ExecuteNonQuery();
        if (rowsAffected == 0) InsertAppUsage(id, year, month, day, minutes);
    }

    private void InsertAppUsage(int id, int year, int month, int day, int minutes) {

        using var command = new SQLiteCommand(Query("InsertAppUsage"), Connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@year", year);
        command.Parameters.AddWithValue("@month", month);
        command.Parameters.AddWithValue("@day", day);
        command.Parameters.AddWithValue("@minutes", minutes);

        command.ExecuteNonQuery();
    }
    public DataTable GetAllApps() {

        using var command = new SQLiteCommand(Query("GetAllApps"), Connection);
        using var adapter = new SQLiteDataAdapter(command);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);
        return dataTable;
    }

    private string Query(string queryName) {
        var fileContent = File.ReadAllText(_sqlQueryCommandPath);
        var queryStartTag = $"-- {queryName}";
        var queryEndTag = "-- END";
        var startIndex = fileContent.IndexOf(queryStartTag, StringComparison.Ordinal) +queryStartTag.Length;
        var endIndex = fileContent.IndexOf(queryEndTag, startIndex, StringComparison.Ordinal);
        return fileContent.Substring(startIndex, endIndex - startIndex).Trim();
    }
}