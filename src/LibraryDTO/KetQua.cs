namespace LibraryDTO;

/// <summary>
/// Kết quả một thao tác nghiệp vụ. Tầng BLL trả về kiểu này thay vì để exception
/// lọt lên UI, nhờ vậy UI chỉ cần hiển thị ThongBao.
/// </summary>
public class KetQua
{
    public bool ThanhCong { get; init; }
    public string ThongBao { get; init; } = string.Empty;

    public static KetQua Ok(string thongBao = "") => new() { ThanhCong = true, ThongBao = thongBao };

    public static KetQua Loi(string thongBao) => new() { ThanhCong = false, ThongBao = thongBao };
}

/// <summary>Kết quả kèm dữ liệu trả về.</summary>
public class KetQua<T> : KetQua
{
    public T? DuLieu { get; init; }

    public static KetQua<T> Ok(T duLieu, string thongBao = "") =>
        new() { ThanhCong = true, ThongBao = thongBao, DuLieu = duLieu };

    public static new KetQua<T> Loi(string thongBao) =>
        new() { ThanhCong = false, ThongBao = thongBao };
}
