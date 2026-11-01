using LibraryCommon;

namespace LibraryUI;

public class MainForm : Form
{
    private readonly Panel _content;
    private readonly Label _lblTieuDe = new();
    private readonly Label _lblPhuDe = new();
    private readonly Dictionary<string, Control> _manHinh = new();
    private readonly Dictionary<string, NavButton> _nutDieuHuong = new();

    private const string TrangSach = "sach";
    private const string TrangDocGia = "docgia";
    private const string TrangMuonTra = "muontra";
    private const string TrangThongKe = "thongke";

    public MainForm()
    {
        Text = "Quản lý thư viện — LIBRA";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 620);

        // Kích thước theo vùng làm việc thật của màn hình, chia tỉ lệ DPI,
        // để cửa sổ luôn nằm trọn trong màn hình (kể cả khi Windows đặt 125%).
        // Ứng dụng chạy DpiUnaware (xem ApplicationHighDpiMode trong .csproj) nên toạ độ
        // WinForms trùng với toạ độ màn hình Windows báo; không chia thêm tỉ lệ DPI nữa.
        var khuVuc = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1280, 800);

        Size = new Size(
            Math.Max(980, khuVuc.Width - 110),
            Math.Max(600, khuVuc.Height - 120));

        BackColor = Theme.Bg;
        Font = Theme.Base;
        KeyPreview = true;

        var sidebar = TaoSidebar();

        _content = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Bg, Padding = new Padding(22, 18, 22, 14) };

        var thanhTrangThai = TaoThanhTrangThai();
        var header = TaoHeader();

        var khungPhai = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Bg };
        khungPhai.Controls.Add(_content);
        khungPhai.Controls.Add(header);
        khungPhai.Controls.Add(thanhTrangThai);

        Controls.Add(khungPhai);
        Controls.Add(sidebar);

        ChonManHinh(TrangSach);
        Logger.Info("Mở ứng dụng Quản lý thư viện.");
    }

    // ------------------------------------------------------------------ sidebar
    private Panel TaoSidebar()
    {
        var sidebar = new Panel { Dock = DockStyle.Left, Width = 232, BackColor = Theme.SidebarBg };

        var logo = new Panel { Dock = DockStyle.Top, Height = 96, BackColor = Theme.SidebarBg };
        var iconLogo = new Label
        {
            Text = "\uE8F1",
            Font = new Font("Segoe MDL2 Assets", 19f),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(47, 91, 234),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(20, 30),
            Size = new Size(42, 42)
        };
        iconLogo.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var path = Theme.BoTron(new Rectangle(0, 0, iconLogo.Width, iconLogo.Height), 12);
            using var brush = new SolidBrush(Theme.Primary);
            e.Graphics.FillPath(brush, path);
        };

        var tenApp = new Label
        {
            Text = "LIBRA",
            Font = new Font("Segoe UI Semibold", 15f),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(74, 32),
            BackColor = Color.Transparent
        };
        var phuDe = new Label
        {
            Text = "Quản lý thư viện",
            Font = Theme.Small,
            ForeColor = Theme.SidebarText,
            AutoSize = true,
            Location = new Point(75, 60),
            BackColor = Color.Transparent
        };
        logo.Controls.Add(iconLogo);
        logo.Controls.Add(tenApp);
        logo.Controls.Add(phuDe);

        var nutSach = TaoNut("Quản lý sách", "\uE8F1", TrangSach);
        var nutDocGia = TaoNut("Quản lý độc giả", "\uE716", TrangDocGia);
        var nutMuonTra = TaoNut("Mượn – Trả sách", "\uE8AB", TrangMuonTra);
        var nutThongKe = TaoNut("Thống kê", "\uE9D9", TrangThongKe);

        var nhomNut = new Panel { Dock = DockStyle.Top, Height = 46 * 4 + 10, BackColor = Theme.SidebarBg, Padding = new Padding(0, 10, 0, 0) };
        // Dock.Top xếp ngược thứ tự add -> add từ dưới lên để hiện đúng thứ tự trên xuống
        nhomNut.Controls.Add(nutThongKe);
        nhomNut.Controls.Add(nutMuonTra);
        nhomNut.Controls.Add(nutDocGia);
        nhomNut.Controls.Add(nutSach);

        var nhanNhom = new Label
        {
            Text = "NGHIỆP VỤ",
            Font = new Font("Segoe UI Semibold", 7.5f),
            ForeColor = Color.FromArgb(120, 138, 168),
            Dock = DockStyle.Top,
            Height = 26,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(28, 0, 0, 0),
            BackColor = Theme.SidebarBg
        };

        var chan = new Panel { Dock = DockStyle.Bottom, Height = 92, BackColor = Color.FromArgb(18, 28, 48), Padding = new Padding(20, 16, 16, 16) };
        var avatar = new Label
        {
            Text = "AD",
            Font = Theme.Bold,
            ForeColor = Color.White,
            BackColor = Theme.Primary,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(38, 38),
            Location = new Point(20, 22)
        };
        avatar.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var path = Theme.BoTron(new Rectangle(0, 0, avatar.Width, avatar.Height), 19);
            using var brush = new SolidBrush(Theme.Primary);
            e.Graphics.FillPath(brush, path);
        };
        var tenNguoiDung = new Label
        {
            Text = "admin",
            Font = Theme.Bold,
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(68, 26),
            BackColor = Color.Transparent
        };
        var vaiTro = new Label
        {
            Text = "Thủ thư",
            Font = Theme.Small,
            ForeColor = Theme.SidebarText,
            AutoSize = true,
            Location = new Point(68, 44),
            BackColor = Color.Transparent
        };
        chan.Controls.Add(avatar);
        chan.Controls.Add(tenNguoiDung);
        chan.Controls.Add(vaiTro);

        sidebar.Controls.Add(nhomNut);
        sidebar.Controls.Add(nhanNhom);
        sidebar.Controls.Add(chan);
        sidebar.Controls.Add(logo);
        return sidebar;
    }

    private NavButton TaoNut(string text, string icon, string khoa)
    {
        var nut = new NavButton { Text = text, Icon = icon, Height = 46 };
        nut.Click += (s, e) => ChonManHinh(khoa);
        _nutDieuHuong[khoa] = nut;
        return nut;
    }

    // ------------------------------------------------------------------ header
    private Panel TaoHeader()
    {
        var header = new Panel { Dock = DockStyle.Top, Height = 86, BackColor = Theme.Surface, Padding = new Padding(24, 0, 24, 0) };

        _lblTieuDe.Font = Theme.H1;
        _lblTieuDe.ForeColor = Theme.Text;
        _lblTieuDe.AutoSize = true;
        _lblTieuDe.Location = new Point(24, 12);
        _lblTieuDe.BackColor = Color.Transparent;

        _lblPhuDe.Font = Theme.Small;
        _lblPhuDe.ForeColor = Theme.Muted;
        _lblPhuDe.AutoSize = true;
        _lblPhuDe.Location = new Point(26, 50);
        _lblPhuDe.BackColor = Color.Transparent;

        var ngay = new Label
        {
            Text = DateTime.Now.ToString("dddd, dd/MM/yyyy", new System.Globalization.CultureInfo("vi-VN")),
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = false,
            Width = 200,
            Height = 24,
            TextAlign = ContentAlignment.MiddleRight,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BackColor = Color.Transparent
        };
        ngay.Location = new Point(header.Width - 230, 30);
        header.Resize += (s, e) => ngay.Location = new Point(header.Width - 230, 30);

        header.Controls.Add(_lblTieuDe);
        header.Controls.Add(_lblPhuDe);
        header.Controls.Add(ngay);

        var vien = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Theme.Border };
        header.Controls.Add(vien);
        return header;
    }

    private Panel TaoThanhTrangThai()
    {
        var bar = new Panel { Dock = DockStyle.Bottom, Height = 30, BackColor = Theme.Surface, Padding = new Padding(24, 0, 24, 0) };

        var csdl = new Label
        {
            Text = AppServices.MoTaKetNoi(),
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(24, 6),
            BackColor = Color.Transparent
        };

        var log = new Label
        {
            Text = "Log: " + Logger.CurrentFile,
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = false,
            Height = 20,
            Width = 520,
            TextAlign = ContentAlignment.MiddleRight,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BackColor = Color.Transparent
        };
        log.Location = new Point(bar.Width - 544, 6);
        bar.Resize += (s, e) => log.Location = new Point(bar.Width - 544, 6);

        bar.Controls.Add(csdl);
        bar.Controls.Add(log);
        return bar;
    }

    // ------------------------------------------------------------------ điều hướng
    private void ChonManHinh(string khoa)
    {
        if (!_manHinh.TryGetValue(khoa, out var view))
        {
            view = TaoManHinh(khoa);
            _manHinh[khoa] = view;
            _content.Controls.Add(view);
        }

        foreach (Control c in _content.Controls) c.Visible = false;
        view.Visible = true;
        view.Dock = DockStyle.Fill;
        view.BringToFront();

        foreach (var (k, nut) in _nutDieuHuong)
        {
            nut.DangChon = k == khoa;
            nut.Invalidate();
        }

        (_lblTieuDe.Text, _lblPhuDe.Text) = khoa switch
        {
            TrangSach => ("Quản lý sách", "Tra cứu, thêm, sửa, xóa sách trong thư viện"),
            TrangDocGia => ("Quản lý độc giả", "Thông tin độc giả và số sách đang mượn"),
            TrangMuonTra => ("Mượn – Trả sách", "Tạo phiếu mượn, ghi nhận ngày trả"),
            TrangThongKe => ("Thống kê", "Tổng quan tình hình thư viện"),
            _ => ("", "")
        };

        if (view is IRefreshableView r) r.RefreshData();
    }

    private Control TaoManHinh(string khoa) => khoa switch
    {
        TrangSach => new SachView(),
        TrangDocGia => new DocGiaView(),
        TrangMuonTra => new MuonTraView(),
        TrangThongKe => new ThongKeView(),
        _ => new Panel()
    };
}
