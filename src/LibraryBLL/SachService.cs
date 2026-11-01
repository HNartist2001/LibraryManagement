using LibraryCommon;
using LibraryDAL;
using LibraryDTO;

namespace LibraryBLL;

/// <summary>Nghiệp vụ quản lý sách: kiểm tra dữ liệu trước khi gọi tầng dữ liệu.</summary>
public class SachService
{
    private readonly ISachRepository _sach;
    private readonly IPhieuMuonRepository _phieu;

    public SachService(ISachRepository sach, IPhieuMuonRepository phieu)
    {
        _sach = sach;
        _phieu = phieu;
    }

    public KetQua<IReadOnlyList<SachHienThi>> LayTatCa() =>
        Guard.Chay("Lấy danh sách sách", () => KetQua<IReadOnlyList<SachHienThi>>.Ok(_sach.LayTatCa()));

    public KetQua<IReadOnlyList<SachHienThi>> TimKiem(string? tuKhoa, string? theLoai) =>
        Guard.Chay("Tìm kiếm sách", () =>
        {
            var ketQua = _sach.TimKiem(tuKhoa, theLoai);
            Logger.Info($"Tìm sách: tuKhoa='{tuKhoa}', theLoai='{theLoai}' -> {ketQua.Count} kết quả.");
            return KetQua<IReadOnlyList<SachHienThi>>.Ok(ketQua);
        });

    public KetQua<IReadOnlyList<string>> LayTheLoai() =>
        Guard.Chay("Lấy danh sách thể loại", () => KetQua<IReadOnlyList<string>>.Ok(_sach.LayTheLoai()));

    public KetQua Them(Sach sach) =>
        Guard.Chay("Thêm sách", () =>
        {
            var loi = KiemTra(sach, laThemMoi: true);
            if (loi is not null)
            {
                Logger.Warn($"Thêm sách bị từ chối: {loi} (MaSach='{sach.MaSach}')");
                return KetQua.Loi(loi);
            }

            _sach.Them(sach);
            Logger.Info($"Đã thêm sách {sach.MaSach} - {sach.TenSach} (số lượng {sach.SoLuong}).");
            return KetQua.Ok($"Đã thêm sách {sach.MaSach}.");
        });

    public KetQua Sua(Sach sach) =>
        Guard.Chay("Cập nhật sách", () =>
        {
            var loi = KiemTra(sach, laThemMoi: false);
            if (loi is not null)
            {
                Logger.Warn($"Cập nhật sách bị từ chối: {loi} (MaSach='{sach.MaSach}')");
                return KetQua.Loi(loi);
            }

            if (!_sach.TonTai(sach.MaSach)) return KetQua.Loi($"Không tìm thấy sách {sach.MaSach}.");

            // Không được giảm số lượng xuống thấp hơn số cuốn đang cho mượn.
            var dangMuon = _phieu.DemDangMuonTheoMaSach(sach.MaSach);
            if (sach.SoLuong < dangMuon)
            {
                var thongBao = $"Số lượng không được nhỏ hơn số đang mượn ({dangMuon} cuốn).";
                Logger.Warn($"Cập nhật {sach.MaSach} bị từ chối: {thongBao}");
                return KetQua.Loi(thongBao);
            }

            _sach.Sua(sach);
            Logger.Info($"Đã cập nhật sách {sach.MaSach} - {sach.TenSach}.");
            return KetQua.Ok($"Đã cập nhật sách {sach.MaSach}.");
        });

    public KetQua Xoa(string maSach) =>
        Guard.Chay("Xóa sách", () =>
        {
            if (string.IsNullOrWhiteSpace(maSach)) return KetQua.Loi("Chưa chọn sách để xóa.");
            if (!_sach.TonTai(maSach)) return KetQua.Loi($"Không tìm thấy sách {maSach}.");

            var dangMuon = _phieu.DemDangMuonTheoMaSach(maSach);
            if (dangMuon > 0)
            {
                var thongBao = $"Không thể xóa: sách {maSach} đang có {dangMuon} phiếu mượn chưa trả.";
                Logger.Warn($"Xóa {maSach} bị từ chối: {thongBao}");
                return KetQua.Loi(thongBao);
            }

            _sach.Xoa(maSach);
            Logger.Info($"Đã xóa sách {maSach}.");
            return KetQua.Ok($"Đã xóa sách {maSach}.");
        });

    /// <summary>Trả về câu thông báo lỗi, hoặc null nếu dữ liệu hợp lệ.</summary>
    private string? KiemTra(Sach s, bool laThemMoi)
    {
        if (string.IsNullOrWhiteSpace(s.MaSach)) return "Mã sách không được để trống.";
        if (s.MaSach.Trim().Length > 10) return "Mã sách tối đa 10 ký tự.";
        if (laThemMoi && _sach.TonTai(s.MaSach.Trim())) return $"Mã sách {s.MaSach} đã tồn tại.";

        if (string.IsNullOrWhiteSpace(s.TenSach)) return "Tên sách không được để trống.";
        if (string.IsNullOrWhiteSpace(s.TacGia)) return "Tác giả không được để trống.";
        if (string.IsNullOrWhiteSpace(s.TheLoai)) return "Thể loại không được để trống.";
        if (s.SoLuong < 0) return "Số lượng phải lớn hơn hoặc bằng 0.";

        var namToiDa = DateTime.Today.Year + 1;
        if (s.NamXuatBan is int nam && (nam < 1900 || nam > namToiDa))
            return $"Năm xuất bản phải trong khoảng 1900..{namToiDa}.";

        return null;
    }
}
