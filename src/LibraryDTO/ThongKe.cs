namespace LibraryDTO;

/// <summary>Số liệu cho màn hình thống kê.</summary>
public class ThongKe
{
    public int TongDauSach { get; set; }
    public int TongCuon { get; set; }
    public int DangMuon { get; set; }
    public int PhieuQuaHan { get; set; }
    public int TongDocGia { get; set; }

    /// <summary>Số cuốn còn có thể cho mượn = tổng số lượng - số đang mượn.</summary>
    public int ConLai => TongCuon - DangMuon;
}

/// <summary>Số sách theo từng thể loại (dùng cho màn hình thống kê).</summary>
public class ThongKeTheLoai
{
    public string TheLoai { get; set; } = string.Empty;
    public int SoDauSach { get; set; }
    public int SoCuon { get; set; }

    public override string ToString() => $"{TheLoai}: {SoDauSach} đầu sách / {SoCuon} cuốn";
}
