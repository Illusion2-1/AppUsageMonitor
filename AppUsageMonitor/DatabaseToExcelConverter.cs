using System.Data;
using System.Data.SQLite;
using OfficeOpenXml;

namespace AppUsageMonitor;

public class DatabaseToExcelConverter {
    public void Convert(string databasePath, string excelPath) {
        var connectionString = $"Data Source={databasePath};Version=3;";
        var query = @"
            SELECT a.""Name"", u.""YY"", u.""MM"", u.""DD"", SUM(u.""minutes"") as ""总时长""
            FROM ""Usage"" u
            JOIN ""App"" a ON u.""ID"" = a.""ID""
            GROUP BY a.""Name"", u.""YY"", u.""MM"", u.""DD""
            ORDER BY a.""Name"", u.""YY"", u.""MM"", u.""DD"";";

        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        using var command = new SQLiteCommand(query, connection);
        using var reader = command.ExecuteReader();
        var dataTable = new DataTable();
        dataTable.Load(reader);

        using var excelPackage = new ExcelPackage();
        var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

        string[] columnNames = { "程序名", "年", "月", "日", "总时长" };

        for (var i = 0; i < dataTable.Columns.Count; i++) worksheet.Cells[1, i + 1].Value = columnNames[i];

        for (var i = 0; i < dataTable.Rows.Count; i++)
        for (var j = 0; j < dataTable.Columns.Count; j++)
            worksheet.Cells[i + 2, j + 1].Value = dataTable.Rows[i][j];

        excelPackage.SaveAs(new System.IO.FileInfo(excelPath));
    }
}