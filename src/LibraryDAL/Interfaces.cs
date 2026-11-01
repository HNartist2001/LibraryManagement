using LibraryDTO;

namespace LibraryDAL;

/// <summary>
/// Hợp đồng truy cập dữ liệu sách. Tầng BLL chỉ biết interface này, nhờ vậy
/// Unit Test có thể thay bằng lớp giả (fake) mà không cần SQL Server.
/// </summary>
public interface ISachRepository
{
    IReadOnlyList<SachHienThi> LayTatCa();
    SachHienThi? LayTheoMa(string maSach);
    IReadOnlyList<SachHienThi> TimKiem(string? tuKhoa, string? theLoai);
    IReadOnlyList<string> LayTheLoai();
    bool TonTai(string maSach);
    void Them(Sach sach);
    void Sua(Sach sach);
    void Xoa(string maSach);
}

public interface IDocGiaRepository
{
    IReadOnlyList<DocGiaHienThi> LayTatCa();
    DocGiaHienThi? LayTheoMa(string maDocGia);
    IReadOnlyList<DocGiaHienThi> TimKiem(string? tuKhoa);
    bool TonTai(string maDocGia);
    void Them(DocGia docGia);
    void Sua(DocGia docGia);
    void Xoa(string maDocGia);
}

public interface IPhieuMuonRepository
{
    IReadOnlyList<PhieuMuonHienThi> LayTatCa();
    IReadOnlyList<PhieuMuonHienThi> LayDangMuon();
    PhieuMuonHienThi? LayTheoMa(int maPhieu);
    int DemDangMuonTheoMaSach(string maSach);
    int Them(PhieuMuon phieu);
    void TraSach(int maPhieu, DateTime ngayTra);
}

public interface IThongKeRepository
{
    ThongKe LayThongKe();
    IReadOnlyList<ThongKeTheLoai> LayTheoTheLoai();
}
