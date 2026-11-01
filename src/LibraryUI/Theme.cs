using System.Drawing.Drawing2D;

namespace LibraryUI;

/// <summary>Bảng màu, font và các hàm tạo control dùng chung cho toàn bộ giao diện.</summary>
public static class Theme
{
    public static readonly Color Bg = Color.FromArgb(241, 244, 251);
    public static readonly Color Surface = Color.White;
    public static readonly Color Primary = Color.FromArgb(47, 91, 234);
    public static readonly Color PrimaryDark = Color.FromArgb(32, 68, 190);
    public static readonly Color PrimarySoft = Color.FromArgb(232, 238, 255);
    public static readonly Color Accent = Color.FromArgb(18, 184, 134);
    public static readonly Color AccentSoft = Color.FromArgb(224, 247, 240);
    public static readonly Color Danger = Color.FromArgb(224, 49, 49);
    public static readonly Color DangerSoft = Color.FromArgb(255, 235, 235);
    public static readonly Color Warning = Color.FromArgb(240, 140, 0);
    public static readonly Color WarningSoft = Color.FromArgb(255, 244, 229);
    public static readonly Color Text = Color.FromArgb(27, 36, 48);
    public static readonly Color Muted = Color.FromArgb(107, 122, 144);
    public static readonly Color Border = Color.FromArgb(226, 232, 242);
    public static readonly Color SidebarBg = Color.FromArgb(22, 34, 58);
    public static readonly Color SidebarText = Color.FromArgb(198, 209, 226);
    public static readonly Color SidebarActive = Color.FromArgb(37, 54, 88);

    public static readonly Font Base = new("Segoe UI", 9.5f);
    public static readonly Font Small = new("Segoe UI", 8.5f);
    public static readonly Font Bold = new("Segoe UI Semibold", 9.5f);
    public static readonly Font H1 = new("Segoe UI Semibold", 17f);
    public static readonly Font H2 = new("Segoe UI Semibold", 12f);
    public static readonly Font H3 = new("Segoe UI Semibold", 10.5f);
    public static readonly Font Big = new("Segoe UI Semibold", 22f);

    /// <summary>Icon Segoe MDL2 Assets (có sẵn trên Windows 10/11) - không cần file ảnh.</summary>
    public static readonly Font Icon = new("Segoe MDL2 Assets", 11f);
    public static readonly Font IconBig = new("Segoe MDL2 Assets", 14f);

    public static GraphicsPath BoTron(Rectangle r, int radius)
    {
        var path = new GraphicsPath();
        var d = radius * 2;

        if (d <= 0)
        {
            path.AddRectangle(r);
            return path;
        }

        if (d > r.Width) d = r.Width;
        if (d > r.Height) d = r.Height;

        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    public static void StyleGrid(DataGridView g)
    {
        g.BorderStyle = BorderStyle.None;
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        g.GridColor = Color.FromArgb(240, 243, 249);
        g.BackgroundColor = Surface;
        g.EnableHeadersVisualStyles = false;
        g.AutoGenerateColumns = false;
        g.AllowUserToAddRows = false;
        g.AllowUserToDeleteRows = false;
        g.AllowUserToResizeRows = false;
        g.ReadOnly = true;
        g.MultiSelect = false;
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        g.RowHeadersVisible = false;
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        g.ColumnHeadersHeight = 40;
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 253);
        g.ColumnHeadersDefaultCellStyle.ForeColor = Muted;
        g.ColumnHeadersDefaultCellStyle.Font = Bold;
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 253);
        g.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        g.DefaultCellStyle.Font = Base;
        g.DefaultCellStyle.ForeColor = Text;
        g.DefaultCellStyle.SelectionBackColor = PrimarySoft;
        g.DefaultCellStyle.SelectionForeColor = Text;
        g.DefaultCellStyle.Padding = new Padding(10, 0, 6, 0);
        g.RowTemplate.Height = 38;
        g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 253, 255);
    }

    public static Label Nhan(string text, Font? font = null, Color? color = null) => new()
    {
        Text = text,
        Font = font ?? Base,
        ForeColor = color ?? Text,
        AutoSize = true,
        BackColor = Color.Transparent
    };

    /// <summary>
    /// Một hàng thanh công cụ: dải trái co giãn + dải phải giữ nguyên kích thước.
    /// Dùng TableLayoutPanel để WinForms tự tính vị trí, không phụ thuộc thứ tự dock.
    /// </summary>
    public static TableLayoutPanel HangToolbar(Control trai, Control phai)
    {
        var hang = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        hang.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        hang.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        hang.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        trai.Dock = DockStyle.Fill;
        trai.Margin = new Padding(0);
        phai.Dock = DockStyle.Fill;
        phai.Margin = new Padding(0);

        hang.Controls.Add(trai, 0, 0);
        hang.Controls.Add(phai, 1, 0);
        return hang;
    }
}
