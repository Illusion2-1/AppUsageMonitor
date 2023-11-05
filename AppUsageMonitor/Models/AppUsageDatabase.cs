using System.Data;
using System.Data.SQLite;

namespace AppUsageMonitor.Models;

public class Database : IAppUsageDatabase {
    public SQLiteConnection Connection;

    public Database(string databasePath) {
        Connection = new SQLiteConnection($"Data Source={databasePath};Version=3;");
        Connection.Open();

        var createAppTableQuery = "CREATE TABLE IF NOT EXISTS App (ID INTEGER PRIMARY KEY, Name TEXT NOT NULL);";
        var createUsageTableQuery =
            "CREATE TABLE IF NOT EXISTS Usage (ID INTEGER, YY INTEGER, MM INTEGER, DD INTEGER, minutes INTEGER, FOREIGN KEY(ID) REFERENCES App(ID));";

        using (var command = new SQLiteCommand(createAppTableQuery, Connection)) {
            command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(createUsageTableQuery, Connection)) {
            command.ExecuteNonQuery();
        }
    }

    public void AddApp(int id, string name) {
        var query = "INSERT INTO App (ID, Name) VALUES (@id, @name);";

        using (var command = new SQLiteCommand(query, Connection)) {
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", name);

            command.ExecuteNonQuery();
        }
    }

    public void DeleteApp(int id) {
        var query = "DELETE FROM App WHERE ID = @id;";

        using (var command = new SQLiteCommand(query, Connection)) {
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }
    }

    public DataTable GetAppUsageByDay(int id, int year, int month, int day) {
        var query = "SELECT minutes FROM Usage WHERE ID = @id AND YY = @year AND MM = @month AND DD = @day;";

        using var command = new SQLiteCommand(query, Connection);
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
        var query = "SELECT SUM(minutes) AS TotalMinutes FROM Usage WHERE ID = @id AND YY = @year AND MM = @month GROUP BY ID;";

        using (var command = new SQLiteCommand(query, Connection)) {
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@year", year);
            command.Parameters.AddWithValue("@month", month);

            using (var adapter = new SQLiteDataAdapter(command)) {
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
    }

    public void UpdateAppUsage(int id, int year, int month, int day, int minutes) {
        var query = "UPDATE Usage SET minutes = @minutes WHERE ID = @id AND YY = @year AND MM = @month AND DD = @day;";

        using (var command = new SQLiteCommand(query, Connection)) {
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@year", year);
            command.Parameters.AddWithValue("@month", month);
            command.Parameters.AddWithValue("@day", day);
            command.Parameters.AddWithValue("@minutes", minutes);

            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0) InsertAppUsage(id, year, month, day, minutes);
        }
    }

    private void InsertAppUsage(int id, int year, int month, int day, int minutes) {
        var query = "INSERT INTO Usage (ID, YY, MM, DD, minutes) VALUES (@id, @year, @month, @day, @minutes);";

        using (var command = new SQLiteCommand(query, Connection)) {
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@year", year);
            command.Parameters.AddWithValue("@month", month);
            command.Parameters.AddWithValue("@day", day);
            command.Parameters.AddWithValue("@minutes", minutes);

            command.ExecuteNonQuery();
        }
    }

    public DataTable GetAllApps() {
        var query = "SELECT DISTINCT Name FROM App;";

        using (var command = new SQLiteCommand(query, Connection)) {
            using (var adapter = new SQLiteDataAdapter(command)) {
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
    }
}