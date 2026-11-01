using LibraryBLL;
using LibraryDTO;

namespace LibraryTests;

[TestClass]
public class ThongKeServiceTests
{
    private FakeDb _db = null!;
    private ThongKeService _svc = null!;

    [TestInitialize]
    public void Setup()
    {
        _db = FakeDb.Mau();
        _svc = new ThongKeService(new FakeThongKeRepository(_db));
    }

    [TestMethod]
    public void LayThongKe_TinhDungCacChiSo()
    {
        var kq = _svc.LayThongKe();

        Assert.IsTrue(kq.ThanhCong);
        var tk = kq.DuLieu!;
        Assert.AreEqual(2, tk.TongDauSach);
        Assert.AreEqual(6, tk.TongCuon);       // 5 + 1
        Assert.AreEqual(1, tk.DangMuon);
        Assert.AreEqual(2, tk.TongDocGia);
    }

    [TestMethod]
    public void ConLai_BangTongCuonTruDangMuon()
    {
        var tk = new ThongKe { TongCuon = 79, DangMuon = 6 };

        Assert.AreEqual(73, tk.ConLai);
    }

    [TestMethod]
    public void LayTheoTheLoai_GomNhomDung()
    {
        var kq = _svc.LayTheoTheLoai();

        Assert.IsTrue(kq.ThanhCong);
        var cntt = kq.DuLieu!.First(x => x.TheLoai == "CNTT");
        Assert.AreEqual(2, cntt.SoDauSach);
        Assert.AreEqual(6, cntt.SoCuon);
    }
}
