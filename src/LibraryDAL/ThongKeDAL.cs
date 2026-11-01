using Microsoft.Data.SqlClient;
using LibraryDTO;

namespace LibraryDAL;

/// <summary>Số liệu thống kê cho màn hình Tổng quan / Thống kê.</summary>
public class ThongKeDAL : IThongKeRepository
{
    public ThongKe LayThongKe()
    {
        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(@"
SELECT (SELECT COUNT(*) FROM Sach)                                                AS TongDauSach,
       (SELECT ISNULL(SUM(SoLuong), 0) FROM Sach)                                 AS TongCuon,
       (SELECT COUNT(*) FROM PhieuMuon WHERE NgayTra IS NULL)                      AS DangMuon,
       (SELECT COUNT(*) FROM PhieuMuon
         WHERE NgayTra IS NULL AND HanTra < CAST(GETDATE() AS DATE))               AS PhieuQuaHan,
       (SELECT COUNT(*) FROM DocGia)                                              AS TongDocGia", conn);

        using var reader = cmd.ExecuteReader();
        reader.Read();

        return new ThongKe
        {
            TongDauSach = reader.GetInt32(0),
            TongCuon = reader.GetInt32(1),
            DangMuon = reader.GetInt32(2),
            PhieuQuaHan = reader.GetInt32(3),
            TongDocGia = reader.GetInt32(4)
        };
    }

    public IReadOnlyList<ThongKeTheLoai> LayTheoTheLoai()
    {
        var list = new List<ThongKeTheLoai>();

        using var conn = Db.MoKetNoi();
        using var cmd = new SqlCommand(@"
SELECT   TheLoai, COUNT(*) AS SoDauSach, SUM(SoLuong) AS SoCuon
FROM     Sach
GROUP BY TheLoai
ORDER BY SUM(SoLuong) DESC", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ThongKeTheLoai
            {
                TheLoai = reader.GetString(0),
                SoDauSach = reader.GetInt32(1),
                SoCuon = reader.GetInt32(2)
            });
        }

        return list;
    }
}
