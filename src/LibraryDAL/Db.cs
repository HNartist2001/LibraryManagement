using System.Configuration;
using Microsoft.Data.SqlClient;

namespace LibraryDAL;

/// <summary>
/// Nơi duy nhất biết chuỗi kết nối. Đọc từ App.config của ứng dụng
/// (connectionStrings -> name="LibraryDB"); nếu thiếu thì dùng giá trị mặc định.
/// Đổi máy chủ chỉ cần sửa App.config, không phải sửa code.
/// </summary>
public static class Db
{
    public const string DefaultConnectionString =
        @"Server=.\SQLEXPRESS05;Database=LibraryDB;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=10";

    private static string? _connectionString;

    public static string ConnectionString => _connectionString ??= Resolve();

    private static string Resolve()
    {
        try
        {
            var cs = ConfigurationManager.ConnectionStrings["LibraryDB"]?.ConnectionString;
            if (!string.IsNullOrWhiteSpace(cs)) return cs;
        }
        catch
        {
            // Không có file cấu hình -> dùng mặc định.
        }

        return DefaultConnectionString;
    }

    /// <summary>Cho phép test / công cụ đổi chuỗi kết nối trong bộ nhớ.</summary>
    public static void DatChuoiKetNoi(string connectionString) => _connectionString = connectionString;

    public static SqlConnection MoKetNoi()
    {
        var conn = new SqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}
