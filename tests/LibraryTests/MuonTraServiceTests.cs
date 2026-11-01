using LibraryBLL;

namespace LibraryTests;

[TestClass]
public class MuonTraServiceTests
{
    private FakeDb _db = null!;
    private MuonTraService _svc = null!;

    [TestInitialize]
    public void Setup()
    {
        _db = FakeDb.Mau();
        _svc = new MuonTraService(
            new FakePhieuMuonRepository(_db),
            new FakeSachRepository(_db),
            new FakeDocGiaRepository(_db));
    }

    // ---------------------------------------------------------------- quy tắc hết sách
    [TestMethod]
    public void KiemTraMuon_SachDaHet_TraVeLoi()
    {
        // S002 có 1 cuốn, đang cho DG001 mượn -> hết.
        var loi = _svc.KiemTraMuon("S002", "DG002", DateTime.Today, DateTime.Today.AddDays(7));

        Assert.IsNotNull(loi);
        StringAssert.Contains(loi, "hết");
    }

    [TestMethod]
    public void KiemTraMuon_ConSach_TraVeNull()
    {
        var loi = _svc.KiemTraMuon("S001", "DG002", DateTime.Today, DateTime.Today.AddDays(7));

        Assert.IsNull(loi);
    }

    // ---------------------------------------------------------------- các quy tắc ngày
    [TestMethod]
    public void KiemTraMuon_HanTraTruocNgayMuon_TraVeLoi()
    {
        var loi = _svc.KiemTraMuon("S001", "DG001", DateTime.Today, DateTime.Today.AddDays(-1));

        Assert.IsNotNull(loi);
        StringAssert.Contains(loi, "Hạn trả");
    }

    [TestMethod]
    public void KiemTraMuon_VuotQua60Ngay_TraVeLoi()
    {
        var loi = _svc.KiemTraMuon("S001", "DG001", DateTime.Today, DateTime.Today.AddDays(61));

        Assert.IsNotNull(loi);
        StringAssert.Contains(loi, "60 ngày");
    }

    [TestMethod]
    public void KiemTraMuon_NgayMuonOTuongLai_TraVeLoi()
    {
        var loi = _svc.KiemTraMuon("S001", "DG001", DateTime.Today.AddDays(1), DateTime.Today.AddDays(10));

        Assert.IsNotNull(loi);
        StringAssert.Contains(loi, "tương lai");
    }

    // ---------------------------------------------------------------- dữ liệu không tồn tại
    [TestMethod]
    public void KiemTraMuon_SachKhongTonTai_TraVeLoi()
    {
        var loi = _svc.KiemTraMuon("S999", "DG001", DateTime.Today, DateTime.Today.AddDays(7));

        Assert.IsNotNull(loi);
        StringAssert.Contains(loi, "Không tìm thấy sách");
    }

    [TestMethod]
    public void KiemTraMuon_DocGiaKhongTonTai_TraVeLoi()
    {
        var loi = _svc.KiemTraMuon("S001", "DG999", DateTime.Today, DateTime.Today.AddDays(7));

        Assert.IsNotNull(loi);
        StringAssert.Contains(loi, "Không tìm thấy độc giả");
    }

    // ---------------------------------------------------------------- tạo phiếu
    [TestMethod]
    public void TaoPhieuMuon_SachDaHet_KhongThemPhieu()
    {
        var soPhieuTruoc = _db.Phieus.Count;

        var kq = _svc.TaoPhieuMuon("S002", "DG002", DateTime.Today, DateTime.Today.AddDays(7));

        Assert.IsFalse(kq.ThanhCong);
        Assert.AreEqual(soPhieuTruoc, _db.Phieus.Count);
    }

    [TestMethod]
    public void TaoPhieuMuon_HopLe_ThemPhieuVaTraVeMaPhieu()
    {
        var kq = _svc.TaoPhieuMuon("S001", "DG002", DateTime.Today, DateTime.Today.AddDays(14));

        Assert.IsTrue(kq.ThanhCong, kq.ThongBao);
        Assert.IsTrue(kq.DuLieu > 0);
        Assert.AreEqual(1, _db.Phieus.Count(p => p.MaSach == "S001" && p.NgayTra is null));
    }

    // ---------------------------------------------------------------- trả sách
    [TestMethod]
    public void TraSach_PhieuDaTra_TraVeLoi()
    {
        _db.Phieus[0].NgayTra = DateTime.Today.AddDays(-1);

        var kq = _svc.TraSach(_db.Phieus[0].MaPhieu, DateTime.Today);

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "đã được trả");
    }

    [TestMethod]
    public void TraSach_NgayTraTruocNgayMuon_TraVeLoi()
    {
        var phieu = _db.Phieus[0];

        var kq = _svc.TraSach(phieu.MaPhieu, phieu.NgayMuon.AddDays(-2));

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "không được trước");
    }

    [TestMethod]
    public void TraSach_HopLe_CapNhatNgayTra()
    {
        var phieu = _db.Phieus[0];

        var kq = _svc.TraSach(phieu.MaPhieu, DateTime.Today);

        Assert.IsTrue(kq.ThanhCong, kq.ThongBao);
        Assert.AreEqual(DateTime.Today, _db.Phieus[0].NgayTra);
        Assert.AreEqual(0, _db.DemDangMuon("S002"));
    }

    [TestMethod]
    public void TraSach_MaPhieuKhongTonTai_TraVeLoi()
    {
        var kq = _svc.TraSach(9999, DateTime.Today);

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "Không tìm thấy phiếu");
    }
}
