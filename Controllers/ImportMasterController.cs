//using ExcelDataReader;
//using FinAxisLeaseBudgeting.Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Npgsql;
//using System.Data;
//using System.Globalization;
//using System.Text;

//namespace FinAxisLeaseBudgeting.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ImportMasterController : Controller
//    {
//        private readonly FinAxisDbContext _context;

//        public ImportMasterController(FinAxisDbContext context)
//        {
//            _context = context;
//        }

//        [NonAction]
//        private string GetTableNameFromSheetName(string sheetName)
//        {
//            return sheetName.Trim().ToLower();
//        }

//        [NonAction]
//        public string NormalizeValue(object cell, string columnName = "")
//        {
//            if (cell == null)
//                return GetDefaultValueForColumn(columnName);

//            if (cell is DateTime dt)
//                return dt.ToString("yyyy-MM-dd HH:mm:ss");

//            var text = cell.ToString()?.Trim();
//            if (string.IsNullOrEmpty(text))
//                return GetDefaultValueForColumn(columnName);

//            if (DateTime.TryParseExact(
//                    text,
//                    new[] { "dd-MM-yyyy", "dd-MM-yyyy HH:mm:ss", "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss" },
//                    CultureInfo.InvariantCulture,
//                    DateTimeStyles.None,
//                    out var parsed))
//            {
//                return parsed.ToString("yyyy-MM-dd HH:mm:ss");
//            }

//            return text;
//        }

//        [NonAction]
//        private string GetDefaultValueForColumn(string columnName)
//        {
//            return columnName.ToLower() switch
//            {
//                "is_active" => "true",
//                "created_at" => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
//                _ => ""
//            };
//        }

//        [NonAction]
//        private string EscapeCsv(string value)
//        {
//            if (string.IsNullOrEmpty(value)) return string.Empty;
//            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
//            {
//                return $"\"{value.Replace("\"", "\"\"")}\"";
//            }
//            return value;
//        }

//        [HttpPost("yardi-import-v1")]
//        [RequestSizeLimit(500_000_000)]
//        [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
//        public async Task<IActionResult> TestImportV3(IFormFile file, string Username, string Type)
//        {
//            if (file == null || file.Length == 0)
//                return BadRequest("File missing");

//            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

//            var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{file.FileName}");
//            List<string> importFailed = new List<string>();

//            try
//            {
//                using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, 64 * 1024, true))
//                {
//                    await file.CopyToAsync(fs);
//                }

//                var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
//                if (conn.State != ConnectionState.Open)
//                    await conn.OpenAsync();

//                using var excelStream = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, false);
//                using var reader = ExcelReaderFactory.CreateReader(excelStream);

//                do
//                {
//                    string targetTable = string.Empty;
//                    try
//                    {
//                        targetTable = GetTableNameFromSheetName(reader.Name);

//                        var tableColumns = new List<string>();
//                        const string columnsSql = @"
//                    SELECT column_name 
//                    FROM information_schema.columns 
//                    WHERE table_schema = 'public' 
//                      AND table_name = @table 
//                    ORDER BY ordinal_position;";

//                        using (var cmd = new NpgsqlCommand(columnsSql, conn))
//                        {
//                            cmd.Parameters.AddWithValue("table", targetTable);
//                            using var colReader = cmd.ExecuteReader();
//                            while (colReader.Read())
//                                tableColumns.Add(colReader.GetString(0));
//                        }

//                        // FIX: Use AS SELECT WHERE 1=0 to avoid inheriting NOT NULL constraints during bulk copy
//                        using (var cmd = new NpgsqlCommand($"CREATE TEMP TABLE {targetTable}_temp AS SELECT * FROM {targetTable} WHERE 1=0;", conn))
//                        {
//                            cmd.ExecuteNonQuery();
//                        }

//                        reader.Read(); // header row

//                        using (var writer = conn.BeginTextImport($"COPY {targetTable}_temp ({string.Join(",", tableColumns)}) FROM STDIN WITH (FORMAT csv)"))
//                        {
//                            while (reader.Read())
//                            {
//                                var line = new StringBuilder();
//                                for (int i = 0; i < tableColumns.Count; i++)
//                                {
//                                    try
//                                    {
//                                        if (i > 0) line.Append(',');

//                                        var cell = reader.GetValue(i);
//                                        var columnName = tableColumns[i];
//                                        line.Append(EscapeCsv(NormalizeValue(cell, columnName)));
//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        Console.WriteLine($"Error reading cell at index {i}: {ex.Message}");
//                                        line.Append("");
//                                    }
//                                }
//                                writer.WriteLine(line.ToString());
//                            }
//                        }

//                        using (var cmd = new NpgsqlCommand($@"
//                    INSERT INTO {targetTable} ({string.Join(",", tableColumns)})
//                    SELECT {string.Join(",", tableColumns)}
//                    FROM {targetTable}_temp
//                    ON CONFLICT DO NOTHING;", conn))
//                        {
//                            cmd.ExecuteNonQuery();
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"Error processing sheet '{reader.Name}': {ex.Message}");
//                        importFailed.Add($"Error processing sheet '{reader.Name}': {ex.Message}");
//                    }
//                    finally
//                    {
//                        if (!string.IsNullOrEmpty(targetTable))
//                        {
//                            try
//                            {
//                                using var dropCmd = new NpgsqlCommand($"DROP TABLE IF EXISTS {targetTable}_temp;", conn);
//                                dropCmd.ExecuteNonQuery();
//                            }
//                            catch { /* Suppress cleanup errors */ }
//                        }
//                    }

//                } while (reader.NextResult());

//                if (importFailed.Any())
//                {
//                    return BadRequest(string.Join("\n", importFailed.ToArray()));
//                }
//                return Ok("Import completed successfully");
//            }
//            finally
//            {
//                if (System.IO.File.Exists(tempFile))
//                    System.IO.File.Delete(tempFile);
//            }
//        }
//    }
//}

using ExcelDataReader;
using FinAxisLeaseBudgeting.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;
using System.Globalization;
using System.Text;

namespace FinAxisLeaseBudgeting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportMasterController : Controller
    {
        private readonly FinAxisDbContext _context;
        private readonly ILogger<ImportMasterController> _logger;

        public ImportMasterController(FinAxisDbContext context, ILogger<ImportMasterController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [NonAction]
        private string GetTableNameFromSheetName(string sheetName)
        {
            return sheetName.Trim().ToLower().Replace(" ", "_").Replace("-", "_");
        }

        [NonAction]
        public string NormalizeValue(object cell, string columnName = "")
        {
            if (cell == null)
                return GetDefaultValueForColumn(columnName);

            if (cell is DateTime dt)
                return dt.ToString("yyyy-MM-dd HH:mm:ss");

            var text = cell.ToString()?.Trim();
            if (string.IsNullOrEmpty(text))
                return GetDefaultValueForColumn(columnName);

            if (DateTime.TryParseExact(
                    text,
                    new[] { "dd-MM-yyyy", "dd-MM-yyyy HH:mm:ss", "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss" },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed))
            {
                return parsed.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return text;
        }

        [NonAction]
        private string GetDefaultValueForColumn(string columnName)
        {
            return columnName.ToLower() switch
            {
                "is_active" => "true",
                "created_at" => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                _ => ""
            };
        }

        [NonAction]
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }

        [NonAction]
        private string FormatDatabaseException(string sheetName, Exception ex)
        {
            if (ex is PostgresException pgEx)
            {
                // Translate common PostgreSQL error codes into user-friendly UI messages
                switch (pgEx.SqlState)
                {
                    case "23505": // Unique violation
                        return $"Duplicate record found in '{sheetName}'. A record with the same unique key already exists.";
                    case "23503": // Foreign key violation
                        return $"Reference error in '{sheetName}': {pgEx.Detail ?? pgEx.MessageText}";
                    case "23502": // Not null violation
                        return $"Missing required data in '{sheetName}': Column '{pgEx.ColumnName}' cannot be null.";
                    case "22P02": // Invalid text representation (type mismatch like string into int/date)
                        return $"Data format error in '{sheetName}': Invalid value provided for column '{pgEx.ColumnName}'.";
                    default:
                        return $"Database error in '{sheetName}': {pgEx.MessageText}";
                }
            }

            return $"Error processing sheet/file '{sheetName}': {ex.Message}";
        }

        [HttpPost("yardi-import-v1")]
        [RequestSizeLimit(500_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
        public async Task<IActionResult> TestImportV3(IFormFile file, string Username, string Type)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Import failed: Uploaded file is missing or empty. User: {Username}, Type: {Type}", Username, Type);
                return BadRequest(new { message = "Uploaded file is missing or empty.", errors = new[] { "Please select a valid file to upload." } });
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{file.FileName}");
            List<string> importFailed = new List<string>();

            try
            {
                using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, 64 * 1024, true))
                {
                    await file.CopyToAsync(fs);
                }

                var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var excelStream = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, false);

                string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                using var reader = extension == ".csv"
                    ? ExcelReaderFactory.CreateCsvReader(excelStream)
                    : ExcelReaderFactory.CreateReader(excelStream);

                do
                {
                    string targetTable = string.Empty;
                    string rawSheetName = string.Empty;
                    try
                    {
                        rawSheetName = string.IsNullOrEmpty(reader.Name) ? Path.GetFileNameWithoutExtension(file.FileName) : reader.Name;
                        targetTable = GetTableNameFromSheetName(rawSheetName);

                        var tableColumns = new List<string>();
                        const string columnsSql = @"
                            SELECT column_name 
                            FROM information_schema.columns 
                            WHERE table_schema = 'public' 
                              AND table_name = @table 
                            ORDER BY ordinal_position;";

                        using (var cmd = new NpgsqlCommand(columnsSql, conn))
                        {
                            cmd.Parameters.AddWithValue("table", targetTable);
                            using var colReader = cmd.ExecuteReader();
                            while (colReader.Read())
                                tableColumns.Add(colReader.GetString(0));
                        }

                        if (tableColumns.Count == 0)
                        {
                            string errMessage = $"Target table '{targetTable}' matching sheet '{rawSheetName}' was not found in the database schema.";
                            _logger.LogWarning(errMessage);
                            importFailed.Add(errMessage);
                            continue;
                        }

                        using (var cmd = new NpgsqlCommand($"CREATE TEMP TABLE {targetTable}_temp AS SELECT * FROM {targetTable} WHERE 1=0;", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        if (!reader.Read())
                        {
                            _logger.LogWarning("Sheet or file '{SheetName}' is empty or missing headers.", rawSheetName);
                            continue;
                        }

                        using (var writer = conn.BeginTextImport($"COPY {targetTable}_temp ({string.Join(",", tableColumns)}) FROM STDIN WITH (FORMAT csv)"))
                        {
                            while (reader.Read())
                            {
                                var line = new StringBuilder();
                                for (int i = 0; i < tableColumns.Count; i++)
                                {
                                    try
                                    {
                                        if (i > 0) line.Append(',');

                                        var cell = reader.GetValue(i);
                                        var columnName = tableColumns[i];
                                        line.Append(EscapeCsv(NormalizeValue(cell, columnName)));
                                    }
                                    catch (Exception cellEx)
                                    {
                                        _logger.LogDebug(cellEx, "Error parsing cell at index {Index} for column {Column}", i, tableColumns[i]);
                                        line.Append("");
                                    }
                                }
                                writer.WriteLine(line.ToString());
                            }
                        }

                        using (var cmd = new NpgsqlCommand($@"
                            INSERT INTO {targetTable} ({string.Join(",", tableColumns)})
                            SELECT {string.Join(",", tableColumns)}
                            FROM {targetTable}_temp
                            ON CONFLICT DO NOTHING;", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception sheetEx)
                    {
                        string friendlyError = FormatDatabaseException(string.IsNullOrEmpty(rawSheetName) ? "unknown" : rawSheetName, sheetEx);
                        _logger.LogError(sheetEx, "Failed processing container '{SheetName}' for user {Username}", rawSheetName, Username);
                        importFailed.Add(friendlyError);
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(targetTable))
                        {
                            try
                            {
                                using var dropCmd = new NpgsqlCommand($"DROP TABLE IF EXISTS {targetTable}_temp;", conn);
                                dropCmd.ExecuteNonQuery();
                            }
                            catch (Exception dropEx)
                            {
                                _logger.LogWarning(dropEx, "Failed to drop temp table {Table}_temp", targetTable);
                            }
                        }
                    }

                } while (reader.NextResult());

                if (importFailed.Any())
                {
                    _logger.LogError("Import completed with errors for user {Username}. Errors count: {Count}", Username, importFailed.Count);
                    return BadRequest(new
                    {
                        message = "Import failed for one or more sheets/files. Please check error details.",
                        errors = importFailed
                    });
                }

                _logger.LogInformation("Import completed successfully for user {Username}, file {FileName}", Username, file.FileName);
                return Ok(new { message = "Import completed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error during import execution for user {Username}", Username);
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred during import processing.",
                    errors = new[] { ex.Message }
                });
            }
            finally
            {
                if (System.IO.File.Exists(tempFile))
                {
                    try { System.IO.File.Delete(tempFile); } catch { }
                }
            }
        }
    }
}