using System.Drawing.Drawing2D;

namespace LibraryUI;

/// <summary>Thẻ nền trắng bo góc, có viền mảnh - dùng làm khung nội dung.</summary>
public class Card : Panel
{
    public int Radius { get; set; } = 14;
    public Color BorderColor { get; set; } = Theme.Border;
    public bool ShowBorder { get; set; } = true;

    public Card()
    {
        DoubleBuffered = true;
        BackColor = Theme.Surface;
        Padding = new Padding(16);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        using var path = Theme.BoTron(rect, Radius);
        using var brush = new SolidBrush(BackColor);
        e.Graphics.FillPath(brush, path);

        if (ShowBorder)
        {
            using var pen = new Pen(BorderColor);
            e.Graphics.DrawPath(pen, path);
        }

        base.OnPaint(e);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (Width > 4 && Height > 4)
        {
            using var path = Theme.BoTron(new Rectangle(0, 0, Width, Height), Radius);
            Region = new Region(path);
        }
    }
}

/// <summary>Nút bo góc, tự vẽ để đổi màu theo trạng thái (thường / hover / disabled).</summary>
public class RoundedButton : Button
{
    public Color MauNen { get; set; } = Theme.Primary;
    public Color MauChu { get; set; } = Color.White;
    public Color? MauVien { get; set; }
    public int Radius { get; set; } = 10;
    public string Icon { get; set; } = string.Empty;

    private bool _hover;

    public RoundedButton()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Cursor = Cursors.Hand;
        Font = Theme.Bold;
        Size = new Size(120, 38);
        BackColor = Color.Transparent;
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Parent?.BackColor ?? Theme.Bg);

        var bg = !Enabled ? Color.FromArgb(210, 216, 226)
               : _hover ? Darken(MauNen, 0.12f)
               : MauNen;

        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using (var path = Theme.BoTron(rect, Radius))
        using (var brush = new SolidBrush(bg))
            e.Graphics.FillPath(brush, path);

        if (MauVien is Color vien)
        {
            using var path = Theme.BoTron(rect, Radius);
            using var pen = new Pen(Enabled ? vien : Color.FromArgb(210, 216, 226));
            e.Graphics.DrawPath(pen, path);
        }

        var mau = Enabled ? MauChu : Color.White;

        if (string.IsNullOrEmpty(Icon))
        {
            TextRenderer.DrawText(e.Graphics, Text, Font, rect, mau,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            return;
        }

        // Icon vẽ bằng font Segoe MDL2 Assets, không dùng font chữ thường.
        using var iconFont = new Font("Segoe MDL2 Assets", 10.5f);
        var coIcon = TextRenderer.MeasureText(e.Graphics, Icon, iconFont, Size.Empty, TextFormatFlags.NoPadding);
        var coChu = TextRenderer.MeasureText(e.Graphics, Text, Font, Size.Empty, TextFormatFlags.NoPadding);
        var tongRong = coIcon.Width + 7 + coChu.Width;
        var batDau = rect.X + Math.Max(6, (rect.Width - tongRong) / 2);

        TextRenderer.DrawText(e.Graphics, Icon, iconFont,
            new Rectangle(batDau, rect.Y, coIcon.Width + 2, rect.Height), mau,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

        TextRenderer.DrawText(e.Graphics, Text, Font,
            new Rectangle(batDau + coIcon.Width + 7, rect.Y, Math.Max(10, rect.Right - (batDau + coIcon.Width + 7) - 4), rect.Height), mau,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
    }

    private static Color Darken(Color c, float amount) => Color.FromArgb(
        c.A,
        (int)Math.Max(0, c.R - 255 * amount),
        (int)Math.Max(0, c.G - 255 * amount),
        (int)Math.Max(0, c.B - 255 * amount));
}

/// <summary>Nút điều hướng ở sidebar: có vạch màu khi đang chọn.</summary>
public class NavButton : Button
{
    public bool DangChon { get; set; }
    public string Icon { get; set; } = string.Empty;

    private bool _hover;

    public NavButton()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Cursor = Cursors.Hand;
        Font = Theme.Base;
        ForeColor = Theme.SidebarText;
        BackColor = Color.Transparent;
        TextAlign = ContentAlignment.MiddleLeft;
        Height = 46;
        Dock = DockStyle.Top;
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Theme.SidebarBg);

        if (DangChon || _hover)
        {
            using var brush = new SolidBrush(DangChon ? Theme.SidebarActive : Color.FromArgb(30, 45, 74));
            e.Graphics.FillRectangle(brush, new Rectangle(10, 4, Width - 20, Height - 8));
        }

        if (DangChon)
        {
            using var accent = new SolidBrush(Theme.Primary);
            e.Graphics.FillRectangle(accent, new Rectangle(10, 10, 3, Height - 20));
        }

        // icon
        if (!string.IsNullOrEmpty(Icon))
        {
            using var iconFont = new Font("Segoe MDL2 Assets", 12f);
            TextRenderer.DrawText(e.Graphics, Icon, iconFont,
                new Rectangle(28, 0, 26, Height),
                DangChon ? Color.White : Theme.SidebarText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPadding);
        }

        TextRenderer.DrawText(e.Graphics, Text, DangChon ? Theme.Bold : Font,
            new Rectangle(60, 0, Width - 70, Height),
            DangChon ? Color.White : Theme.SidebarText,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
    }
}

/// <summary>Thẻ số liệu lớn cho màn hình tổng quan / thống kê.</summary>
public class StatCard : Card
{
    public string Nhan { get; set; } = string.Empty;
    public string GiaTri { get; set; } = "0";
    public string GhiChu { get; set; } = string.Empty;
    public string Icon { get; set; } = "\uE8F1";
    public Color MauNhan { get; set; } = Theme.Primary;
    public Color MauIconNen { get; set; } = Theme.PrimarySoft;

    public StatCard()
    {
        Height = 104;
        Padding = new Padding(16);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // icon tròn góc phải
        var iconRect = new Rectangle(Width - 62, 18, 42, 42);
        using (var path = Theme.BoTron(iconRect, 12))
        using (var brush = new SolidBrush(MauIconNen))
            g.FillPath(brush, path);

        using (var iconFont = new Font("Segoe MDL2 Assets", 15f))
            TextRenderer.DrawText(g, Icon, iconFont, iconRect, MauNhan,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

        TextRenderer.DrawText(g, Nhan, Theme.Small, new Rectangle(16, 14, Width - 80, 20),
            Theme.Muted, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

        TextRenderer.DrawText(g, GiaTri, Theme.Big, new Rectangle(14, 34, Width - 80, 36),
            Theme.Text, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

        if (!string.IsNullOrEmpty(GhiChu))
        {
            TextRenderer.DrawText(g, GhiChu, Theme.Small, new Rectangle(16, Height - 32, Width - 30, 20),
                Theme.Muted, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}
