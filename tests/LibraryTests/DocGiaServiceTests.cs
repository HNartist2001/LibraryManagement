using LibraryBLL;
using LibraryDTO;

namespace LibraryTests;

[TestClass]
public class DocGiaServiceTests
{
    private FakeDb _db = null!;
    private DocGiaService _svc = null!;

    [TestInitialize]
    public void Setup()
    {
        _db = FakeDb.Mau();
        _svc = new DocGiaService(new FakeDocGiaRepository(_db), new FakePhieuMuonRepository(_db));
    }

    [TestMethod]
    public void Them_HoTenRong_TraVeLoi()
    {
        var kq = _svc.Them(new DocGia { MaDocGia = "DG100", HoTen = "  " });

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "Họ tên");
    }

    [TestMethod]
    public void Them_EmailSaiDinhDang_TraVeLoi()
    {
        var kq = _svc.Them(new DocGia { MaDocGia = "DG100", HoTen = "Nguyễn Văn A", Email = "abc" });

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "Email");
    }

    [TestMethod]
    public void Them_HopLe_ThemVaoDanhSach()
    {
        var kq = _svc.Them(new DocGia { MaDocGia = "DG100", HoTen = "Nguyễn Văn A", Lop = "CNTT09", Email = "a@sv.edu.vn" });

        Assert.IsTrue(kq.ThanhCong, kq.ThongBao);
        Assert.IsTrue(_db.DocGias.Any(d => d.MaDocGia == "DG100"));
    }

    [TestMethod]
    public void Them_TrungMaDocGia_TraVeLoi()
    {
        var kq = _svc.Them(new DocGia { MaDocGia = "DG001", HoTen = "Người khác" });

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "đã tồn tại");
    }

    [TestMethod]
    public void Xoa_DocGiaConPhieuMuon_TraVeLoi()
    {
        var kq = _svc.Xoa("DG001");

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "Không thể xóa");
        Assert.IsTrue(_db.DocGias.Any(d => d.MaDocGia == "DG001"));
    }

    [TestMethod]
    public void Xoa_DocGiaKhongConPhieuMuon_XoaKhoiDanhSach()
    {
        var kq = _svc.Xoa("DG002");

        Assert.IsTrue(kq.ThanhCong, kq.ThongBao);
        Assert.IsFalse(_db.DocGias.Any(d => d.MaDocGia == "DG002"));
    }

    [DataTestMethod]
    [DataRow("an@sv.edu.vn", true)]
    [DataRow("nguyen van an@sv.edu.vn", true)]
    [DataRow("an@sv", false)]
    [DataRow("an@@sv.edu.vn", false)]
    [DataRow("@sv.edu.vn", false)]
    [DataRow("an@.vn", false)]
    [DataRow("", false)]
    public void EmailHopLe_KiemTraDinhDang(string email, bool mongDoi)
    {
        Assert.AreEqual(mongDoi, DocGiaService.EmailHopLe(email));
    }
}
