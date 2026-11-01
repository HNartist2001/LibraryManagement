using LibraryCommon;
using LibraryDTO;
using Microsoft.Data.SqlClient;

namespace LibraryBLL;

/// <summary>
/// Bọc một thao tác nghiệp vụ: ghi log và đổi exception thành KetQua để UI không phải try/catch.
/// Lỗi kết nối CSDL được phân biệt với lỗi SQL thường bằng Class &gt;= 20 (Class là độ nghiêm trọng
/// do server trả về). Không dựa vào SqlException.Number: mất kết nối trả về Number = -1 hoặc 5,
/// không nằm trong danh sách mã nào người ta viết tay.
/// </summary>
internal static class Guard
{
    public static KetQua Chay(string thaoTac, Func<KetQua> viec)
    {
        try
        {
            return viec();
        }
        catch (SqlException ex) when (ex.Class >= 20)
        {
            Logger.Critical(
                $"{thaoTac}: mất kết nối SQL Server. Number={ex.Number}, Class={ex.Class}, Server={DbInfo()}",
                ex);
            return KetQua.Loi("Không kết nối được tới SQL Server. Kiểm tra lại chuỗi kết nối trong App.config.");
        }
        catch (SqlException ex)
        {
            Logger.Error($"{thaoTac}: lỗi SQL. Number={ex.Number}, Class={ex.Class} - {ex.Message}", ex);
            return KetQua.Loi($"Lỗi cơ sở dữ liệu: {ex.Message}");
        }
        catch (Exception ex)
        {
            Logger.Error($"{thaoTac}: lỗi không mong đợi.", ex);
            return KetQua.Loi($"Đã xảy ra lỗi: {ex.Message}");
        }
    }

    public static KetQua<T> Chay<T>(string thaoTac, Func<KetQua<T>> viec)
    {
        try
        {
            return viec();
        }
        catch (SqlException ex) when (ex.Class >= 20)
        {
            Logger.Critical($"{thaoTac}: mất kết nối SQL Server. Number={ex.Number}, Class={ex.Class}", ex);
            return KetQua<T>.Loi("Không kết nối được tới SQL Server. Kiểm tra lại chuỗi kết nối trong App.config.");
        }
        catch (SqlException ex)
        {
            Logger.Error($"{thaoTac}: lỗi SQL. Number={ex.Number}, Class={ex.Class} - {ex.Message}", ex);
            return KetQua<T>.Loi($"Lỗi cơ sở dữ liệu: {ex.Message}");
        }
        catch (Exception ex)
        {
            Logger.Error($"{thaoTac}: lỗi không mong đợi.", ex);
            return KetQua<T>.Loi($"Đã xảy ra lỗi: {ex.Message}");
        }
    }

    private static string DbInfo()
    {
        try
        {
            return LibraryDAL.Db.ConnectionString;
        }
        catch
        {
            return "(không đọc được chuỗi kết nối)";
        }
    }
}
