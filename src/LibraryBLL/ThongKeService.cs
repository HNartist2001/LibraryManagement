using LibraryDAL;
using LibraryDTO;

namespace LibraryBLL;

/// <summary>Nghiệp vụ thống kê: tổng đầu sách, số cuốn còn có thể mượn, số phiếu đang mượn.</summary>
public class ThongKeService
{
    private readonly IThongKeRepository _thongKe;

    public ThongKeService(IThongKeRepository thongKe) => _thongKe = thongKe;

    public KetQua<ThongKe> LayThongKe() =>
        Guard.Chay("Lấy số liệu thống kê", () => KetQua<ThongKe>.Ok(_thongKe.LayThongKe()));

    public KetQua<IReadOnlyList<ThongKeTheLoai>> LayTheoTheLoai() =>
        Guard.Chay("Lấy thống kê theo thể loại",
            () => KetQua<IReadOnlyList<ThongKeTheLoai>>.Ok(_thongKe.LayTheoTheLoai()));
}
