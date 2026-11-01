using Microsoft.Data.SqlClient;
using LibraryDTO;

namespace LibraryDAL;

/// <summary>Truy cập bảng PhieuMuon (mượn / trả sách).</summary>
public class PhieuMuonDAL : IPhieuMuonRepository
{
    private const string SelectCoTen = @"
SELECT  p.MaPhieu, p.MaSach, p.MaDocGia, p.NgayMuon, p.HanTra, p.NgayTra,
        s.TenSach, d.HoTen
FROM    PhieuMuon p
JOIN    Sach   s ON s.MaSach   = p.MaSach
JOIN    DocGia d ON d.MaDocGia = p.MaDocGia ";

    public IReadOnlyList<PhieuMuonHienThi> LayTatCa()
    {
        var list = new List<PhieuMuonHienThi>();

        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(SelectCoTen + " ORDER BY p.NgayMuon DESC, p.MaPhieu DESC", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read()) list.Add(Doc(reader));
        return list;
    }

    public IReadOnlyList<PhieuMuonHienThi> LayDangMuon()
    {
        var list = new List<PhieuMuonHienThi>();

        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(SelectCoTen + @"
WHERE p.NgayTra IS NULL
ORDER BY p.HanTra", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read()) list.Add(Doc(reader));
        return list;
    }

    public PhieuMuonHienThi? LayTheoMa(int maPhieu)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(SelectCoTen + " WHERE p.MaPhieu = @Ma", conn);
        cmd.Parameters.AddWithValue("@Ma", maPhieu);

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Doc(reader) : null;
    }

    public int DemDangMuonTheoMaSach(string maSach)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(
            "SELECT COUNT(*) FROM PhieuMuon WHERE MaSach = @Ma AND NgayTra IS NULL", conn);
        cmd.Parameters.AddWithValue("@Ma", maSach);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public int Them(PhieuMuon p)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(@"
INSERT INTO PhieuMuon (MaSach, MaDocGia, NgayMuon, HanTra, NgayTra)
VALUES (@MaSach, @MaDocGia, @NgayMuon, @HanTra, NULL);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);

        cmd.Parameters.AddWithValue("@MaSach", p.MaSach);
        cmd.Parameters.AddWithValue("@MaDocGia", p.MaDocGia);
        cmd.Parameters.AddWithValue("@NgayMuon", p.NgayMuon.Date);
        cmd.Parameters.AddWithValue("@HanTra", p.HanTra.Date);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void TraSach(int maPhieu, DateTime ngayTra)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(
            "UPDATE PhieuMuon SET NgayTra = @NgayTra WHERE MaPhieu = @Ma AND NgayTra IS NULL", conn);

        cmd.Parameters.AddWithValue("@Ma", maPhieu);
        cmd.Parameters.AddWithValue("@NgayTra", ngayTra.Date);
        cmd.ExecuteNonQuery();
    }

    private static PhieuMuonHienThi Doc(SqlDataReader r) => new()
    {
        MaPhieu = r.GetInt32(0),
        MaSach = r.GetString(1),
        MaDocGia = r.GetString(2),
        NgayMuon = r.GetDateTime(3),
        HanTra = r.GetDateTime(4),
        NgayTra = r.IsDBNull(5) ? null : r.GetDateTime(5),
        TenSach = r.GetString(6),
        HoTenDocGia = r.GetString(7)
    };
}
