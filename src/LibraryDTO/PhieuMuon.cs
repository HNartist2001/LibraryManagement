namespace LibraryDTO;

/// <summary>Phiếu mượn sách (bảng PhieuMuon). NgayTra = null nghĩa là chưa trả.</summary>
public class PhieuMuon
{
    public int MaPhieu { get; set; }
    public string MaSach { get; set; } = string.Empty;
    public string MaDocGia { get; set; } = string.Empty;
    public DateTime NgayMuon { get; set; }
    public DateTime HanTra { get; set; }
    public DateTime? NgayTra { get; set; }

    public bool DaTra => NgayTra.HasValue;

    public bool QuaHan => !DaTra && HanTra.Date < DateTime.Today;
}

/// <summary>Phiếu mượn kèm tên sách / tên độc giả để hiển thị.</summary>
public class PhieuMuonHienThi : PhieuMuon
{
    public string TenSach { get; set; } = string.Empty;
    public string HoTenDocGia { get; set; } = string.Empty;

    public string TrangThai
    {
        get
        {
            if (DaTra) return "Đã trả";
            if (QuaHan) return "Quá hạn";
            return "Đang mượn";
        }
    }

    public int SoNgayConLai => (HanTra.Date - DateTime.Today).Days;
}
