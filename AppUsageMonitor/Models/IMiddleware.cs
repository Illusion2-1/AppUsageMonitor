using System.Data;

namespace AppUsageMonitor.Models;

public interface IMiddleware {
    /// <summary>
    /// Returns the number of minutes used by the app on the specified day.
    /// </summary>
    /// <param name="appName"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <returns></returns>
    int GetAppUsageByDay(string appName, int year, int month, int day);

    /// <summary>
    /// Returns the total number of minutes used by the app on the specified month.
    /// </summary>
    /// <param name="appName"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <returns></returns>
    int GetAppUsageByMonth(string appName, int year, int month);

    /// <summary>
    /// Returns the total number of minutes used by the app on each day of the specified month.
    /// </summary>
    /// <param name="appName"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <returns></returns>
    double[] GetAppUsageByDaysInMonth(string appName, int year, int month);

    /// <summary>
    /// Returns the total number of minutes used by the app on each month of the specified year.
    /// </summary>
    /// <param name="appName"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    double[] GetAppUsageByMonthsInYear(string appName, int year);

    /// <summary>
    /// Adds the specified app to the database.
    /// </summary>
    /// <param name="appName"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <param name="minutes"></param>
    void UpdateAppUsage(string appName, int year, int month, int day, int minutes);

    /// <summary>
    /// delegate access entry for GetAllApps in AppUsageDatabase.cs
    /// </summary>
    /// <returns></returns>
    DataTable GetAllApps();

    /// <summary>
    /// Adds the specified app to the database.
    /// delegate access entry for AddApp in AppUsageDatabase.cs
    /// </summary>
    /// <param name="appName"></param>
    void AddApp(string appName);

    /// <summary>
    /// Deletes the specified app from the database.
    /// delegate access entry for DeleteApp in AppUsageDatabase.cs
    /// </summary>
    /// <param name="appName"></param>
    void DeleteApp(string appName);
}