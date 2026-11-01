namespace LibraryDTO;

/// <summary>Sách trong thư viện (dữ liệu lưu trong bảng Sach).</summary>
public class Sach
{
    public string MaSach { get; set; } = string.Empty;
    public string TenSach { get; set; } = string.Empty;
    public string TacGia { get; set; } = string.Empty;
    public string TheLoai { get; set; } = string.Empty;
    public int? NamXuatBan { get; set; }
    public int SoLuong { get; set; }

    public override string ToString() => $"{MaSach} - {TenSach}";
}

/// <summary>Sách kèm số cuốn đang được mượn, dùng để hiển thị lên lưới.</summary>
public class SachHienThi : Sach
{
    public int DangMuon { get; set; }

    public int ConLai => SoLuong - DangMuon;

    public string TinhTrang => ConLai > 0 ? $"Còn {ConLai} cuốn" : "Hết sách";

    public bool ConChoMuon => ConLai > 0;
}
