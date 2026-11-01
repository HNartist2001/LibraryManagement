using System.Text;

namespace LibraryCommon;

/// <summary>
/// Ghi nhật ký ra file .txt theo ngày: Logs/Library_yyyyMMdd.txt
/// Nguyên tắc: KHÔNG BAO GIỜ ném exception. Nếu không ghi được file thì tự tắt im lặng,
/// vì lỗi ghi log không được phép làm chết ứng dụng.
/// </summary>
public static class Logger
{
    private static readonly object Lock = new();
    private static string? _folder;
    private static bool _disabled;

    /// <summary>Thư mục log. Mặc định: cạnh file chạy (AppContext.BaseDirectory)\Logs.</summary>
    public static string Folder
    {
        get
        {
            if (_folder is not null) return _folder;

            try
            {
                var dir = Path.Combine(AppContext.BaseDirectory, "Logs");
                Directory.CreateDirectory(dir);
                _folder = dir;
            }
            catch
            {
                // Không xác định được thư mục ghi được -> tắt file log, không làm chết app.
                _disabled = true;
                _folder = string.Empty;
            }

            return _folder;
        }
    }

    public static string CurrentFile => _disabled
        ? "(file logging disabled)"
        : Path.Combine(Folder, $"Library_{DateTime.Now:yyyyMMdd}.txt");

    public static void Info(string message) => Write("INFO ", message, null);

    public static void Warn(string message) => Write("WARN ", message, null);

    public static void Error(string message, Exception? ex = null) => Write("ERROR", message, ex);

    /// <summary>Dùng khi mất kết nối CSDL / tài nguyên bắt buộc: ứng dụng không thể tiếp tục bình thường.</summary>
    public static void Critical(string message, Exception? ex = null) => Write("CRIT ", message, ex);

    private static void Write(string level, string message, Exception? ex)
    {
        if (_disabled) return;

        try
        {
            var line = new StringBuilder()
                .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                .Append(" | ").Append(level)
                .Append(" | ").Append(message);

            if (ex is not null)
                line.AppendLine().Append(ex);

            lock (Lock)
            {
                // Folder có thể ném lần đầu -> bắt ở đây, không để lọt ra ngoài.
                var file = CurrentFile;
                if (_disabled) return;
                File.AppendAllText(file, line + Environment.NewLine, new UTF8Encoding(true));
            }
        }
        catch
        {
            _disabled = true; // ghi một lần thất bại là đủ để tắt, tránh treo app vì log
        }
    }
}
