using LibraryCommon;
using LibraryDAL;
using LibraryDTO;

namespace LibraryBLL;

/// <summary>Nghiệp vụ mượn – trả sách, gồm quy tắc "hết sách thì không cho mượn".</summary>
public class MuonTraService
{
    /// <summary>Hạn trả tối đa cho một phiếu (ngày).</summary>
    public const int SoNgayMuonToiDa = 60;

    private readonly IPhieuMuonRepository _phieu;
    private readonly ISachRepository _sach;
    private readonly IDocGiaRepository _docGia;

    public MuonTraService(IPhieuMuonRepository phieu, ISachRepository sach, IDocGiaRepository docGia)
    {
        _phieu = phieu;
        _sach = sach;
        _docGia = docGia;
    }

    public KetQua<IReadOnlyList<PhieuMuonHienThi>> LayTatCa() =>
        Guard.Chay("Lấy danh sách phiếu mượn", () => KetQua<IReadOnlyList<PhieuMuonHienThi>>.Ok(_phieu.LayTatCa()));

    public KetQua<IReadOnlyList<PhieuMuonHienThi>> LayDangMuon() =>
        Guard.Chay("Lấy danh sách đang mượn", () => KetQua<IReadOnlyList<PhieuMuonHienThi>>.Ok(_phieu.LayDangMuon()));

    public KetQua<int> TaoPhieuMuon(string maSach, string maDocGia, DateTime ngayMuon, DateTime hanTra) =>
        Guard.Chay("Tạo phiếu mượn", () =>
        {
            var loi = KiemTraMuon(maSach, maDocGia, ngayMuon, hanTra);
            if (loi is not null)
            {
                Logger.Warn($"Mượn sách bị từ chối: {loi} (MaSach='{maSach}', MaDocGia='{maDocGia}')");
                return KetQua<int>.Loi(loi);
            }

            var maPhieu = _phieu.Them(new PhieuMuon
            {
                MaSach = maSach,
                MaDocGia = maDocGia,
                NgayMuon = ngayMuon.Date,
                HanTra = hanTra.Date
            });

            Logger.Info($"Tạo phiếu mượn #{maPhieu}: {maDocGia} mượn {maSach}, hạn trả {hanTra:dd/MM/yyyy}.");
            return KetQua<int>.Ok(maPhieu, $"Đã tạo phiếu mượn #{maPhieu}.");
        });

    public KetQua TraSach(int maPhieu, DateTime ngayTra) =>
        Guard.Chay("Trả sách", () =>
        {
            var phieu = _phieu.LayTheoMa(maPhieu);
            if (phieu is null) return KetQua.Loi($"Không tìm thấy phiếu mượn #{maPhieu}.");
            if (phieu.DaTra) return KetQua.Loi($"Phiếu #{maPhieu} đã được trả trước đó ({phieu.NgayTra:dd/MM/yyyy}).");

            if (ngayTra.Date < phieu.NgayMuon.Date)
            {
                var thongBao = $"Ngày trả ({ngayTra:dd/MM/yyyy}) không được trước ngày mượn ({phieu.NgayMuon:dd/MM/yyyy}).";
                Logger.Warn($"Trả sách bị từ chối: {thongBao}");
                return KetQua.Loi(thongBao);
            }

            _phieu.TraSach(maPhieu, ngayTra);

            var tre = (ngayTra.Date - phieu.HanTra.Date).Days;
            var ghiChu = tre > 0 ? $" (trả trễ {tre} ngày)" : string.Empty;
            Logger.Info($"Đã trả phiếu #{maPhieu}: {phieu.MaSach} - {phieu.HoTenDocGia}{ghiChu}.");
            return KetQua.Ok($"Đã ghi nhận trả sách cho phiếu #{maPhieu}{ghiChu}.");
        });

    /// <summary>
    /// Kiểm tra toàn bộ điều kiện mượn. Tách riêng để Unit Test gọi trực tiếp mà không cần CSDL.
    /// Trả về null nếu hợp lệ.
    /// </summary>
    public string? KiemTraMuon(string maSach, string maDocGia, DateTime ngayMuon, DateTime hanTra)
    {
        if (string.IsNullOrWhiteSpace(maSach)) return "Chưa chọn sách.";
        if (string.IsNullOrWhiteSpace(maDocGia)) return "Chưa chọn độc giả.";

        if (ngayMuon.Date > DateTime.Today) return "Ngày mượn không được ở tương lai.";
        if (hanTra.Date < ngayMuon.Date) return "Hạn trả phải bằng hoặc sau ngày mượn.";
        if ((hanTra.Date - ngayMuon.Date).TotalDays > SoNgayMuonToiDa)
            return $"Hạn trả không được quá {SoNgayMuonToiDa} ngày kể từ ngày mượn.";

        var sach = _sach.LayTheoMa(maSach);
        if (sach is null) return $"Không tìm thấy sách {maSach}.";

        if (_docGia.LayTheoMa(maDocGia) is null) return $"Không tìm thấy độc giả {maDocGia}.";

        // Quy tắc bắt buộc của đề bài: hết sách thì không cho mượn.
        if (!sach.ConChoMuon)
            return $"Sách '{sach.TenSach}' đã hết ({sach.SoLuong}/{sach.SoLuong} cuốn đang được mượn).";

        return null;
    }
}
