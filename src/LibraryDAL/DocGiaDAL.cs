using Microsoft.Data.SqlClient;
using LibraryDTO;

namespace LibraryDAL;

/// <summary>Truy cập bảng DocGia.</summary>
public class DocGiaDAL : IDocGiaRepository
{
    private const string SelectCoDangMuon = @"
SELECT  d.MaDocGia, d.HoTen, ISNULL(d.Lop, N'') AS Lop, ISNULL(d.Email, N'') AS Email,
        ISNULL(m.DangMuon, 0) AS DangMuon
FROM    DocGia d
OUTER APPLY (
        SELECT COUNT(*) AS DangMuon
        FROM   PhieuMuon p
        WHERE  p.MaDocGia = d.MaDocGia AND p.NgayTra IS NULL
) m ";

    public IReadOnlyList<DocGiaHienThi> LayTatCa()
    {
        var list = new List<DocGiaHienThi>();

        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(SelectCoDangMuon + " ORDER BY d.MaDocGia", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read()) list.Add(Doc(reader));
        return list;
    }

    public DocGiaHienThi? LayTheoMa(string maDocGia)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(SelectCoDangMuon + " WHERE d.MaDocGia = @Ma", conn);
        cmd.Parameters.AddWithValue("@Ma", maDocGia);

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Doc(reader) : null;
    }

    public IReadOnlyList<DocGiaHienThi> TimKiem(string? tuKhoa)
    {
        var list = new List<DocGiaHienThi>();

        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(SelectCoDangMuon + @"
WHERE (@TuKhoa = N'' OR d.HoTen LIKE @Like OR d.MaDocGia LIKE @Like OR ISNULL(d.Lop, N'') LIKE @Like)
ORDER BY d.MaDocGia", conn);

        var kw = (tuKhoa ?? string.Empty).Trim();
        cmd.Parameters.AddWithValue("@TuKhoa", kw);
        cmd.Parameters.AddWithValue("@Like", "%" + kw + "%");

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) list.Add(Doc(reader));
        return list;
    }

    public bool TonTai(string maDocGia)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand("SELECT COUNT(*) FROM DocGia WHERE MaDocGia = @Ma", conn);
        cmd.Parameters.AddWithValue("@Ma", maDocGia);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public void Them(DocGia d)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(@"
INSERT INTO DocGia (MaDocGia, HoTen, Lop, Email)
VALUES (@Ma, @HoTen, @Lop, @Email)", conn);

        GanThamSo(cmd, d);
        cmd.ExecuteNonQuery();
    }

    public void Sua(DocGia d)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(@"
UPDATE DocGia SET HoTen = @HoTen, Lop = @Lop, Email = @Email WHERE MaDocGia = @Ma", conn);

        GanThamSo(cmd, d);
        cmd.ExecuteNonQuery();
    }

    public void Xoa(string maDocGia)
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand("DELETE FROM DocGia WHERE MaDocGia = @Ma", conn);
        cmd.Parameters.AddWithValue("@Ma", maDocGia);
        cmd.ExecuteNonQuery();
    }

    private static void GanThamSo(SqlCommand cmd, DocGia d)
    {
        cmd.Parameters.AddWithValue("@Ma", d.MaDocGia);
        cmd.Parameters.AddWithValue("@HoTen", d.HoTen);
        cmd.Parameters.AddWithValue("@Lop", (object?)d.Lop ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Email", (object?)d.Email ?? DBNull.Value);
    }

    private static DocGiaHienThi Doc(SqlDataReader r) => new()
    {
        MaDocGia = r.GetString(0),
        HoTen = r.GetString(1),
        Lop = r.GetString(2),
        Email = r.GetString(3),
        DangMuon = r.GetInt32(4)
    };
}
