using System.Data;

namespace AppUsageMonitor.Models;

public interface IAppUsageDatabase {
    /// <summary>
    /// Insert a new app into the database.
    /// Query with the command:
    /// <code>INSERT INTO App (ID, Name) VALUES (@id, @name);</code>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    void AddApp(int id, string name);

    /// <summary>
    /// Delete an app from the database.
    /// Query with the command:
    /// <code>DELETE FROM App WHERE ID = @id</code>
    /// </summary>
    /// <param name="id"></param>
    void DeleteApp(int id);

    /// <summary>
    /// Query the app usage by day.
    /// Returns a DataTable with the following columns:
    /// minutes
    /// Query command:
    /// <code>SELECT minutes FROM Usage WHERE ID = @id AND YY = @year AND MM = @month AND DD = @day</code>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <returns>DataTable</returns>
    DataTable GetAppUsageByDay(int id, int year, int month, int day);

    /// <summary>
    /// Query the app usage by month.
    /// Returns a DataTable with the following columns:
    /// minutes
    /// Query command:
    /// <code>SELECT minutes FROM Usage WHERE ID = @id AND YY = @year AND MM = @month</code>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <returns>DataTable</returns>
    DataTable GetAppUsageByMonth(int id, int year, int month);

    /// <summary>
    /// Query the app usage by month.
    /// Returns a DataTable with the following columns:
    /// minutes
    /// Query command:
    /// <code>SELECT minutes FROM Usage WHERE ID = @id AND YY = @year AND MM = @month</code>
    /// </summary>
    /// <returns>DataTable</returns>
    DataTable GetAllApps();

    /// <summary>
    /// Query the app usage by month.
    /// Returns a DataTable with the following columns:
    /// minutes
    /// Query command:
    /// <code>SELECT minutes FROM Usage WHERE ID = @id AND YY = @year AND MM = @month</code>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <param name="minutes"></param>
    /// <returns>DataTable</returns>
    void UpdateAppUsage(int id, int year, int month, int day, int minutes);
}