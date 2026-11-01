using Microsoft.Data.SqlClient;
using LibraryDTO;

namespace LibraryDAL;

/// <summary>Truy cập bảng Sach bằng ADO.NET (SqlConnection / SqlCommand / SqlDataReader).</summary>
public class SachDAL : ISachRepository
{
    private const string SelectCoDangMuon = @"
SELECT  s.MaSach, s.TenSach, s.TacGia, s.TheLoai, s.NamXuatBan, s.SoLuong,
        ISNULL(m.DangMuon, 0) AS DangMuon
FROM    Sach s
OUTER APPLY (
        SELECT COUNT(*) AS DangMuon
        FROM   PhieuMuon p
        WHERE  p.MaSach = s.MaSach AND p.NgayTra IS NULL
) m ";

    public IReadOnlyList<SachHienThi> LayTatCa()
    {
        var list = new List<SachHienThi>();

        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(SelectCoDangMuon + " ORDER BY s.MaSach", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read()) list.Add(Doc(reader));
        return list;
    }

    public SachHienThi? LayTheoMa(string maSach)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(SelectCoDangMuon + " WHERE s.MaSach = @MaSach", conn);
        cmd.Parameters.AddWithValue("@MaSach", maSach);

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Doc(reader) : null;
    }

    public IReadOnlyList<SachHienThi> TimKiem(string? tuKhoa, string? theLoai)
    {
        var list = new List<SachHienThi>();

        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(SelectCoDangMuon + @"
WHERE  (@TuKhoa = N'' OR s.TenSach LIKE @Like OR s.TacGia LIKE @Like)
  AND  (@TheLoai = N'' OR s.TheLoai = @TheLoai)
ORDER BY s.MaSach", conn);

        var kw = (tuKhoa ?? string.Empty).Trim();
        cmd.Parameters.AddWithValue("@TuKhoa", kw);
        cmd.Parameters.AddWithValue("@Like", "%" + kw + "%");
        cmd.Parameters.AddWithValue("@TheLoai", (theLoai ?? string.Empty).Trim());

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) list.Add(Doc(reader));
        return list;
    }

    public IReadOnlyList<string> LayTheLoai()
    {
        var list = new List<string>();

        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand("SELECT DISTINCT TheLoai FROM Sach ORDER BY TheLoai", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read()) list.Add(reader.GetString(0));
        return list;
    }

    public bool TonTai(string maSach)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand("SELECT COUNT(*) FROM Sach WHERE MaSach = @MaSach", conn);
        cmd.Parameters.AddWithValue("@MaSach", maSach);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public void Them(Sach sach)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(@"
INSERT INTO Sach (MaSach, TenSach, TacGia, TheLoai, NamXuatBan, SoLuong)
VALUES (@MaSach, @TenSach, @TacGia, @TheLoai, @NamXuatBan, @SoLuong)", conn);

        GanThamSo(cmd, sach);
        cmd.ExecuteNonQuery();
    }

    public void Sua(Sach sach)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(@"
UPDATE Sach
SET    TenSach = @TenSach, TacGia = @TacGia, TheLoai = @TheLoai,
       NamXuatBan = @NamXuatBan, SoLuong = @SoLuong
WHERE  MaSach = @MaSach", conn);

        GanThamSo(cmd, sach);
        cmd.ExecuteNonQuery();
    }

    public void Xoa(string maSach)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand("DELETE FROM Sach WHERE MaSach = @MaSach", conn);
        cmd.Parameters.AddWithValue("@MaSach", maSach);
        cmd.ExecuteNonQuery();
    }

    private static void GanThamSo(SqlCommand cmd, Sach s)
    {
        cmd.Parameters.AddWithValue("@MaSach", s.MaSach);
        cmd.Parameters.AddWithValue("@TenSach", s.TenSach);
        cmd.Parameters.AddWithValue("@TacGia", s.TacGia);
        cmd.Parameters.AddWithValue("@TheLoai", s.TheLoai);
        cmd.Parameters.AddWithValue("@NamXuatBan", (object?)s.NamXuatBan ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SoLuong", s.SoLuong);
    }

    private static SachHienThi Doc(SqlDataReader r) => new()
    {
        MaSach = r.GetString(0),
        TenSach = r.GetString(1),
        TacGia = r.GetString(2),
        TheLoai = r.GetString(3),
        NamXuatBan = r.IsDBNull(4) ? null : r.GetInt32(4),
        SoLuong = r.GetInt32(5),
        DangMuon = r.GetInt32(6)
    };
}
