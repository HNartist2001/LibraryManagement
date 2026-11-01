using LibraryCommon;
using LibraryDAL;
using LibraryDTO;

namespace LibraryBLL;

/// <summary>Nghiệp vụ quản lý độc giả.</summary>
public class DocGiaService
{
    private readonly IDocGiaRepository _docGia;
    private readonly IPhieuMuonRepository _phieu;

    public DocGiaService(IDocGiaRepository docGia, IPhieuMuonRepository phieu)
    {
        _docGia = docGia;
        _phieu = phieu;
    }

    public KetQua<IReadOnlyList<DocGiaHienThi>> LayTatCa() =>
        Guard.Chay("Lấy danh sách độc giả", () => KetQua<IReadOnlyList<DocGiaHienThi>>.Ok(_docGia.LayTatCa()));

    public KetQua<IReadOnlyList<DocGiaHienThi>> TimKiem(string? tuKhoa) =>
        Guard.Chay("Tìm kiếm độc giả", () =>
        {
            var ketQua = _docGia.TimKiem(tuKhoa);
            Logger.Info($"Tìm độc giả: tuKhoa='{tuKhoa}' -> {ketQua.Count} kết quả.");
            return KetQua<IReadOnlyList<DocGiaHienThi>>.Ok(ketQua);
        });

    public KetQua Them(DocGia d) =>
        Guard.Chay("Thêm độc giả", () =>
        {
            var loi = KiemTra(d, laThemMoi: true);
            if (loi is not null)
            {
                Logger.Warn($"Thêm độc giả bị từ chối: {loi} (MaDocGia='{d.MaDocGia}')");
                return KetQua.Loi(loi);
            }

            _docGia.Them(d);
            Logger.Info($"Đã thêm độc giả {d.MaDocGia} - {d.HoTen}.");
            return KetQua.Ok($"Đã thêm độc giả {d.MaDocGia}.");
        });

    public KetQua Sua(DocGia d) =>
        Guard.Chay("Cập nhật độc giả", () =>
        {
            var loi = KiemTra(d, laThemMoi: false);
            if (loi is not null)
            {
                Logger.Warn($"Cập nhật độc giả bị từ chối: {loi} (MaDocGia='{d.MaDocGia}')");
                return KetQua.Loi(loi);
            }

            if (!_docGia.TonTai(d.MaDocGia)) return KetQua.Loi($"Không tìm thấy độc giả {d.MaDocGia}.");

            _docGia.Sua(d);
            Logger.Info($"Đã cập nhật độc giả {d.MaDocGia} - {d.HoTen}.");
            return KetQua.Ok($"Đã cập nhật độc giả {d.MaDocGia}.");
        });

    public KetQua Xoa(string maDocGia) =>
        Guard.Chay("Xóa độc giả", () =>
        {
            if (string.IsNullOrWhiteSpace(maDocGia)) return KetQua.Loi("Chưa chọn độc giả để xóa.");
            if (!_docGia.TonTai(maDocGia)) return KetQua.Loi($"Không tìm thấy độc giả {maDocGia}.");

            var phieu = _phieu.LayDangMuon().Where(p => p.MaDocGia == maDocGia).ToList();
            if (phieu.Count > 0)
            {
                var thongBao = $"Không thể xóa: độc giả {maDocGia} còn {phieu.Count} phiếu mượn chưa trả.";
                Logger.Warn($"Xóa {maDocGia} bị từ chối: {thongBao}");
                return KetQua.Loi(thongBao);
            }

            _docGia.Xoa(maDocGia);
            Logger.Info($"Đã xóa độc giả {maDocGia}.");
            return KetQua.Ok($"Đã xóa độc giả {maDocGia}.");
        });

    private string? KiemTra(DocGia d, bool laThemMoi)
    {
        if (string.IsNullOrWhiteSpace(d.MaDocGia)) return "Mã độc giả không được để trống.";
        if (d.MaDocGia.Trim().Length > 10) return "Mã độc giả tối đa 10 ký tự.";
        if (laThemMoi && _docGia.TonTai(d.MaDocGia.Trim())) return $"Mã độc giả {d.MaDocGia} đã tồn tại.";
        if (string.IsNullOrWhiteSpace(d.HoTen)) return "Họ tên không được để trống.";
        if (!string.IsNullOrWhiteSpace(d.Email) && !EmailHopLe(d.Email))
            return "Email không hợp lệ (phải có dạng ten@tenmien).";

        return null;
    }

    /// <summary>Kiểm tra email ở mức tối thiểu: có đúng một @, phần trước và sau đều không rỗng, có dấu chấm ở tên miền.</summary>
    public static bool EmailHopLe(string email)
    {
        var e = (email ?? string.Empty).Trim();
        var at = e.IndexOf('@');
        if (at <= 0 || at != e.LastIndexOf('@') || at == e.Length - 1) return false;

        var domain = e[(at + 1)..];
        return domain.Contains('.') && !domain.StartsWith('.') && !domain.EndsWith('.');
    }
}
