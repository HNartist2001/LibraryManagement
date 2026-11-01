using LibraryDAL;
using LibraryDTO;

namespace LibraryTests;

/// <summary>
/// "Cơ sở dữ liệu" trong bộ nhớ dùng cho Unit Test: thay cho SQL Server để test
/// tầng nghiệp vụ mà không cần cài đặt gì, chạy nhanh và lặp lại được.
/// </summary>
public class FakeDb
{
    public List<Sach> Sachs { get; } = new();
    public List<DocGia> DocGias { get; } = new();
    public List<PhieuMuon> Phieus { get; } = new();
    public int NextPhieu { get; set; } = 1;

    public int DemDangMuon(string maSach) =>
        Phieus.Count(p => p.MaSach == maSach && p.NgayTra is null);

    public int DemDangMuonCuaDocGia(string maDocGia) =>
        Phieus.Count(p => p.MaDocGia == maDocGia && p.NgayTra is null);

    public SachHienThi HienThi(Sach s) => new()
    {
        MaSach = s.MaSach,
        TenSach = s.TenSach,
        TacGia = s.TacGia,
        TheLoai = s.TheLoai,
        NamXuatBan = s.NamXuatBan,
        SoLuong = s.SoLuong,
        DangMuon = DemDangMuon(s.MaSach)
    };

    /// <summary>Dữ liệu mẫu tối thiểu: 2 sách (1 sách đã hết), 2 độc giả, 1 phiếu đang mượn.</summary>
    public static FakeDb Mau()
    {
        var db = new FakeDb();

        db.Sachs.Add(new Sach { MaSach = "S001", TenSach = "Lập trình C# căn bản", TacGia = "Nguyễn Văn Hùng", TheLoai = "CNTT", NamXuatBan = 2023, SoLuong = 5 });
        db.Sachs.Add(new Sach { MaSach = "S002", TenSach = "Cơ sở dữ liệu", TacGia = "Trần Thị Mai", TheLoai = "CNTT", NamXuatBan = 2022, SoLuong = 1 });

        db.DocGias.Add(new DocGia { MaDocGia = "DG001", HoTen = "Nguyễn Văn An", Lop = "CNTT01", Email = "an@sv.edu.vn" });
        db.DocGias.Add(new DocGia { MaDocGia = "DG002", HoTen = "Trần Thị Bình", Lop = "CNTT01", Email = "binh@sv.edu.vn" });

        // S002 có 1 cuốn và đang cho DG001 mượn -> đã hết sách.
        db.Phieus.Add(new PhieuMuon
        {
            MaPhieu = db.NextPhieu++,
            MaSach = "S002",
            MaDocGia = "DG001",
            NgayMuon = DateTime.Today.AddDays(-2),
            HanTra = DateTime.Today.AddDays(12),
            NgayTra = null
        });

        return db;
    }
}

public class FakeSachRepository : ISachRepository
{
    private readonly FakeDb _db;
    public FakeSachRepository(FakeDb db) => _db = db;

    public IReadOnlyList<SachHienThi> LayTatCa() => _db.Sachs.Select(_db.HienThi).ToList();

    public SachHienThi? LayTheoMa(string maSach)
    {
        var s = _db.Sachs.FirstOrDefault(x => x.MaSach == maSach);
        return s is null ? null : _db.HienThi(s);
    }

    public IReadOnlyList<SachHienThi> TimKiem(string? tuKhoa, string? theLoai)
    {
        var kw = (tuKhoa ?? string.Empty).Trim();
        var tl = (theLoai ?? string.Empty).Trim();

        return _db.Sachs
            .Where(s => kw.Length == 0 ||
                        s.TenSach.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                        s.TacGia.Contains(kw, StringComparison.OrdinalIgnoreCase))
            .Where(s => tl.Length == 0 || s.TheLoai == tl)
            .Select(_db.HienThi)
            .ToList();
    }

    public IReadOnlyList<string> LayTheLoai() =>
        _db.Sachs.Select(s => s.TheLoai).Distinct().OrderBy(x => x).ToList();

    public bool TonTai(string maSach) => _db.Sachs.Any(s => s.MaSach == maSach);

    public void Them(Sach sach) => _db.Sachs.Add(sach);

    public void Sua(Sach sach)
    {
        var i = _db.Sachs.FindIndex(s => s.MaSach == sach.MaSach);
        if (i >= 0) _db.Sachs[i] = sach;
    }

    public void Xoa(string maSach) => _db.Sachs.RemoveAll(s => s.MaSach == maSach);
}

public class FakeDocGiaRepository : IDocGiaRepository
{
    private readonly FakeDb _db;
    public FakeDocGiaRepository(FakeDb db) => _db = db;

    public IReadOnlyList<DocGiaHienThi> LayTatCa() => _db.DocGias.Select(HienThi).ToList();

    public DocGiaHienThi? LayTheoMa(string maDocGia)
    {
        var d = _db.DocGias.FirstOrDefault(x => x.MaDocGia == maDocGia);
        return d is null ? null : HienThi(d);
    }

    public IReadOnlyList<DocGiaHienThi> TimKiem(string? tuKhoa)
    {
        var kw = (tuKhoa ?? string.Empty).Trim();
        return _db.DocGias
            .Where(d => kw.Length == 0 ||
                        d.HoTen.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                        d.MaDocGia.Contains(kw, StringComparison.OrdinalIgnoreCase))
            .Select(HienThi)
            .ToList();
    }

    public bool TonTai(string maDocGia) => _db.DocGias.Any(d => d.MaDocGia == maDocGia);

    public void Them(DocGia docGia) => _db.DocGias.Add(docGia);

    public void Sua(DocGia docGia)
    {
        var i = _db.DocGias.FindIndex(d => d.MaDocGia == docGia.MaDocGia);
        if (i >= 0) _db.DocGias[i] = docGia;
    }

    public void Xoa(string maDocGia) => _db.DocGias.RemoveAll(d => d.MaDocGia == maDocGia);

    private DocGiaHienThi HienThi(DocGia d) => new()
    {
        MaDocGia = d.MaDocGia,
        HoTen = d.HoTen,
        Lop = d.Lop,
        Email = d.Email,
        DangMuon = _db.DemDangMuonCuaDocGia(d.MaDocGia)
    };
}

public class FakePhieuMuonRepository : IPhieuMuonRepository
{
    private readonly FakeDb _db;
    public FakePhieuMuonRepository(FakeDb db) => _db = db;

    public IReadOnlyList<PhieuMuonHienThi> LayTatCa() => _db.Phieus.Select(HienThi).ToList();

    public IReadOnlyList<PhieuMuonHienThi> LayDangMuon() =>
        _db.Phieus.Where(p => p.NgayTra is null).Select(HienThi).ToList();

    public PhieuMuonHienThi? LayTheoMa(int maPhieu)
    {
        var p = _db.Phieus.FirstOrDefault(x => x.MaPhieu == maPhieu);
        return p is null ? null : HienThi(p);
    }

    public int DemDangMuonTheoMaSach(string maSach) => _db.DemDangMuon(maSach);

    public int Them(PhieuMuon phieu)
    {
        phieu.MaPhieu = _db.NextPhieu++;
        _db.Phieus.Add(phieu);
        return phieu.MaPhieu;
    }

    public void TraSach(int maPhieu, DateTime ngayTra)
    {
        var p = _db.Phieus.FirstOrDefault(x => x.MaPhieu == maPhieu);
        if (p is not null) p.NgayTra = ngayTra.Date;
    }

    private PhieuMuonHienThi HienThi(PhieuMuon p) => new()
    {
        MaPhieu = p.MaPhieu,
        MaSach = p.MaSach,
        MaDocGia = p.MaDocGia,
        NgayMuon = p.NgayMuon,
        HanTra = p.HanTra,
        NgayTra = p.NgayTra,
        TenSach = _db.Sachs.FirstOrDefault(s => s.MaSach == p.MaSach)?.TenSach ?? string.Empty,
        HoTenDocGia = _db.DocGias.FirstOrDefault(d => d.MaDocGia == p.MaDocGia)?.HoTen ?? string.Empty
    };
}

public class FakeThongKeRepository : IThongKeRepository
{
    private readonly FakeDb _db;
    public FakeThongKeRepository(FakeDb db) => _db = db;

    public ThongKe LayThongKe() => new()
    {
        TongDauSach = _db.Sachs.Count,
        TongCuon = _db.Sachs.Sum(s => s.SoLuong),
        DangMuon = _db.Phieus.Count(p => p.NgayTra is null),
        PhieuQuaHan = _db.Phieus.Count(p => p.NgayTra is null && p.HanTra.Date < DateTime.Today),
        TongDocGia = _db.DocGias.Count
    };

    public IReadOnlyList<ThongKeTheLoai> LayTheoTheLoai() => _db.Sachs
        .GroupBy(s => s.TheLoai)
        .Select(g => new ThongKeTheLoai { TheLoai = g.Key, SoDauSach = g.Count(), SoCuon = g.Sum(s => s.SoLuong) })
        .ToList();
}
