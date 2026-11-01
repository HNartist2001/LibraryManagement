namespace LibraryDTO;

/// <summary>Độc giả của thư viện (bảng DocGia).</summary>
public class DocGia
{
    public string MaDocGia { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Lop { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public override string ToString() => $"{MaDocGia} - {HoTen}";
}

/// <summary>Độc giả kèm số phiếu đang mượn.</summary>
public class DocGiaHienThi : DocGia
{
    public int DangMuon { get; set; }

    public string TinhTrang => DangMuon == 0 ? "Không mượn" : $"Đang mượn {DangMuon} cuốn";
}
