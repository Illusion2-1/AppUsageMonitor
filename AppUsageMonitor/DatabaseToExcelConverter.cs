using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using OfficeOpenXml;

namespace AppUsageMonitor;

public class DatabaseToExcelConverter {
    public void Convert(string databasePath, string excelPath) {
        // 连接字符串
        var connectionString = $"Data Source={databasePath};Version=3;";
        // SQL 查询
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

        // 创建Excel包
        using var excelPackage = new ExcelPackage();
        // 字典来存储程序名和对应的工作表
        var sheetsDictionary = new Dictionary<string, ExcelWorksheet>();

        // 列名
        string[] columnNames = { "程序名", "年", "月", "日", "总时长" };

        // 读取数据并添加到对应的工作表
        while (reader.Read()) {
            // 获取程序名
            var programName = reader.GetString(0);
            // 检查是否已经为该程序名创建了工作表
            if (!sheetsDictionary.TryGetValue(programName, out var worksheet)) {
                // 如果没有，则创建一个新的工作表
                worksheet = excelPackage.Workbook.Worksheets.Add(programName);
                sheetsDictionary[programName] = worksheet;
                // 添加列名到新的工作表
                for (var i = 0; i < columnNames.Length; i++)
                    worksheet.Cells[1, i + 1].Value = columnNames[i];
            }

            // 将数据行添加到工作表
            var rowIndex = worksheet.Dimension?.Rows + 1 ?? 2;
            for (var i = 0; i < reader.FieldCount; i++)
                worksheet.Cells[rowIndex, i + 1].Value = reader.GetValue(i);
        }

        // 保存Excel文件
        var fileInfo = new FileInfo(excelPath);
        excelPackage.SaveAs(fileInfo);
    }
}