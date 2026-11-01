using LibraryBLL;
using LibraryDTO;

namespace LibraryTests;

[TestClass]
public class SachServiceTests
{
    private FakeDb _db = null!;
    private SachService _svc = null!;

    [TestInitialize]
    public void Setup()
    {
        _db = FakeDb.Mau();
        _svc = new SachService(new FakeSachRepository(_db), new FakePhieuMuonRepository(_db));
    }

    private static Sach SachMoi(string ma = "S100", string ten = "Sách mới") => new()
    {
        MaSach = ma,
        TenSach = ten,
        TacGia = "Tác giả",
        TheLoai = "CNTT",
        NamXuatBan = 2024,
        SoLuong = 2
    };

    [TestMethod]
    public void Them_MaRong_TraVeLoi()
    {
        var kq = _svc.Them(SachMoi(ma: "   "));

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "Mã sách");
    }

    [TestMethod]
    public void Them_TrungMa_TraVeLoi()
    {
        var kq = _svc.Them(SachMoi(ma: "S001"));

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "đã tồn tại");
    }

    [TestMethod]
    public void Them_TenSachRong_TraVeLoi()
    {
        var kq = _svc.Them(SachMoi(ten: ""));

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "Tên sách");
    }

    [TestMethod]
    public void Them_SoLuongAm_TraVeLoi()
    {
        var sach = SachMoi();
        sach.SoLuong = -1;

        var kq = _svc.Them(sach);

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "Số lượng");
    }

    [TestMethod]
    public void Them_NamXuatBanNgoaiKhoang_TraVeLoi()
    {
        var sach = SachMoi();
        sach.NamXuatBan = 1800;

        var kq = _svc.Them(sach);

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "Năm xuất bản");
    }

    [TestMethod]
    public void Them_HopLe_ThemVaoKho()
    {
        var soLuongTruoc = _db.Sachs.Count;

        var kq = _svc.Them(SachMoi());

        Assert.IsTrue(kq.ThanhCong, kq.ThongBao);
        Assert.AreEqual(soLuongTruoc + 1, _db.Sachs.Count);
        Assert.IsTrue(_db.Sachs.Any(s => s.MaSach == "S100"));
    }

    [TestMethod]
    public void Sua_SoLuongNhoHonSoDangMuon_TraVeLoi()
    {
        // S002 có 1 cuốn và đang cho mượn 1 -> không được giảm số lượng xuống 0.
        var sach = _db.Sachs.First(s => s.MaSach == "S002");
        sach.SoLuong = 0;

        var kq = _svc.Sua(sach);

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "đang mượn");
    }

    [TestMethod]
    public void Sua_ThongTinHopLe_CapNhatDuLieu()
    {
        var sach = _db.Sachs.First(s => s.MaSach == "S001");
        sach.TenSach = "Tên đã sửa";

        var kq = _svc.Sua(sach);

        Assert.IsTrue(kq.ThanhCong, kq.ThongBao);
        Assert.AreEqual("Tên đã sửa", _db.Sachs.First(s => s.MaSach == "S001").TenSach);
    }

    [TestMethod]
    public void Xoa_SachDangCoPhieuMuon_TraVeLoi()
    {
        var kq = _svc.Xoa("S002");

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "Không thể xóa");
        Assert.IsTrue(_db.Sachs.Any(s => s.MaSach == "S002"));
    }

    [TestMethod]
    public void Xoa_SachKhongCoPhieuMuon_XoaKhoiKho()
    {
        var kq = _svc.Xoa("S001");

        Assert.IsTrue(kq.ThanhCong, kq.ThongBao);
        Assert.IsFalse(_db.Sachs.Any(s => s.MaSach == "S001"));
    }

    [TestMethod]
    public void Xoa_MaKhongTonTai_TraVeLoi()
    {
        var kq = _svc.Xoa("S999");

        Assert.IsFalse(kq.ThanhCong);
        StringAssert.Contains(kq.ThongBao, "Không tìm thấy");
    }

    [TestMethod]
    public void TimKiem_TheoTheLoai_TraVeDungSach()
    {
        var kq = _svc.TimKiem(null, "CNTT");

        Assert.IsTrue(kq.ThanhCong);
        Assert.AreEqual(2, kq.DuLieu!.Count);
    }

    [TestMethod]
    public void TimKiem_TheoTenKhongKhop_TraVeRong()
    {
        var kq = _svc.TimKiem("không có sách này", null);

        Assert.IsTrue(kq.ThanhCong);
        Assert.AreEqual(0, kq.DuLieu!.Count);
    }
}
