using LibraryCommon;

namespace LibraryUI;

internal static class Program
{
    /// <summary>Điểm khởi động ứng dụng Quản lý thư viện.</summary>
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        Logger.Info("=== Khởi động ứng dụng Quản lý thư viện ===");
        Logger.Info("Chuỗi kết nối: " + LibraryDAL.Db.ConnectionString);

        try
        {
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            Logger.Critical("Ứng dụng dừng do lỗi không xử lý được.", ex);
            MessageBox.Show(
                "Ứng dụng gặp lỗi không mong muốn:\n\n" + ex.Message +
                "\n\nChi tiết đã được ghi vào file log:\n" + Logger.CurrentFile,
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Logger.Info("=== Đóng ứng dụng ===");
        }
    }
}
