using LibraryBLL;
using LibraryDAL;
using LibraryDTO;
using Microsoft.Data.SqlClient;

namespace LibraryUI;

/// <summary>
/// Khởi tạo các repository (DAL) và service (BLL) một lần cho cả ứng dụng.
/// UI chỉ lấy service ở đây, không tự new DAL - giữ đúng hướng phụ thuộc UI -> BLL -> DAL.
/// </summary>
public static class AppServices
{
    public static ISachRepository SachRepository { get; } = new SachDAL();
    public static IDocGiaRepository DocGiaRepository { get; } = new DocGiaDAL();
    public static IPhieuMuonRepository PhieuMuonRepository { get; } = new PhieuMuonDAL();
    public static IThongKeRepository ThongKeRepository { get; } = new ThongKeDAL();

    public static SachService Sach { get; } = new(SachRepository, PhieuMuonRepository);
    public static DocGiaService DocGia { get; } = new(DocGiaRepository, PhieuMuonRepository);
    public static MuonTraService MuonTra { get; } = new(PhieuMuonRepository, SachRepository, DocGiaRepository);
    public static ThongKeService ThongKe { get; } = new(ThongKeRepository);

    /// <summary>Mô tả ngắn "CSDL: LibraryDB @ .\SQLEXPRESS05" để hiện ở thanh trạng thái.</summary>
    public static string MoTaKetNoi()
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(Db.ConnectionString);
            return $"CSDL: {builder.InitialCatalog} @ {builder.DataSource}";
        }
        catch
        {
            return "CSDL: (không đọc được cấu hình)";
        }
    }
}

/// <summary>Màn hình có dữ liệu cần nạp lại khi được mở.</summary>
public interface IRefreshableView
{
    void RefreshData();
}
