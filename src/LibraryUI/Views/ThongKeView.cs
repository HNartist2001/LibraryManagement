using LibraryDTO;

namespace LibraryUI;

/// <summary>Màn hình Thống kê: thẻ số liệu + bảng số sách theo thể loại.</summary>
public class ThongKeView : UserControl, IRefreshableView
{
    private readonly StatCard _cardDauSach = new();
    private readonly StatCard _cardTongCuon = new();
    private readonly StatCard _cardDangMuon = new();
    private readonly StatCard _cardQuaHan = new();
    private readonly StatCard _cardDocGia = new();
    private readonly DataGridView _grid = new();
    private readonly Label _lblCapNhat = new();

    public ThongKeView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Bg;
        Font = Theme.Base;

        var hangThe = new Panel { Dock = DockStyle.Top, Height = 118, BackColor = Theme.Bg };
        var the = new[] { _cardDauSach, _cardTongCuon, _cardDangMuon, _cardQuaHan, _cardDocGia };
        var chuoi = new[]
        {
            ("Tổng đầu sách", "\uE8F1", Theme.Primary, Theme.PrimarySoft),
            ("Tổng số cuốn", "\uE8B7", Theme.Accent, Theme.AccentSoft),
            ("Đang mượn", "\uE8AB", Theme.Warning, Theme.WarningSoft),
            ("Quá hạn", "\uE7BA", Theme.Danger, Theme.DangerSoft),
            ("Độc giả", "\uE716", Color.FromArgb(112, 72, 232), Color.FromArgb(238, 233, 255))
        };

        for (var i = 0; i < the.Length; i++)
        {
            var (nhan, icon, mau, nen) = chuoi[i];
            the[i].Nhan = nhan;
            the[i].Icon = icon;
            the[i].MauNhan = mau;
            the[i].MauIconNen = nen;
            hangThe.Controls.Add(the[i]);
        }

        void XepThe()
        {
            var khoang = 12;
            var rong = (hangThe.Width - khoang * (the.Length - 1)) / the.Length;
            for (var i = 0; i < the.Length; i++)
            {
                the[i].Location = new Point(i * (rong + khoang), 0);
                the[i].Size = new Size(rong, 104);
            }
        }

        hangThe.Resize += (s, e) => XepThe();
        hangThe.HandleCreated += (s, e) => XepThe();

        Theme.StyleGrid(_grid);
        _grid.Dock = DockStyle.Fill;
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thể loại", DataPropertyName = nameof(ThongKeTheLoai.TheLoai), FillWeight = 40 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số đầu sách", DataPropertyName = nameof(ThongKeTheLoai.SoDauSach), FillWeight = 25 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số cuốn", DataPropertyName = nameof(ThongKeTheLoai.SoCuon), FillWeight = 25 });

        var card = new Card { Dock = DockStyle.Fill, Padding = new Padding(18, 14, 18, 10) };

        var tieuDe = new Label
        {
            Text = "SÁCH THEO THỂ LOẠI",
            Font = new Font("Segoe UI Semibold", 8f),
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(18, 14),
            BackColor = Color.Transparent
        };

        _lblCapNhat.Font = Theme.Small;
        _lblCapNhat.ForeColor = Theme.Muted;
        _lblCapNhat.AutoSize = true;
        _lblCapNhat.BackColor = Color.Transparent;
        _lblCapNhat.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        var luoi = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 36, 0, 0) };
        luoi.Controls.Add(_grid);
        card.Controls.Add(luoi);
        card.Controls.Add(tieuDe);
        card.Controls.Add(_lblCapNhat);

        Controls.Add(card);
        Controls.Add(hangThe);

        Load += (s, e) => RefreshData();
    }

    public void RefreshData()
    {
        var tk = AppServices.ThongKe.LayThongKe();
        if (tk.ThanhCong && tk.DuLieu is not null)
        {
            var d = tk.DuLieu;
            _cardDauSach.GiaTri = d.TongDauSach.ToString();
            _cardDauSach.GhiChu = "đầu sách trong thư viện";
            _cardTongCuon.GiaTri = d.TongCuon.ToString();
            _cardTongCuon.GhiChu = $"còn {d.ConLai} cuốn có thể mượn";
            _cardDangMuon.GiaTri = d.DangMuon.ToString();
            _cardDangMuon.GhiChu = "phiếu chưa trả";
            _cardQuaHan.GiaTri = d.PhieuQuaHan.ToString();
            _cardQuaHan.GhiChu = "phiếu quá hạn trả";
            _cardDocGia.GiaTri = d.TongDocGia.ToString();
            _cardDocGia.GhiChu = "độc giả đã đăng ký";

            foreach (var c in new[] { _cardDauSach, _cardTongCuon, _cardDangMuon, _cardQuaHan, _cardDocGia }) c.Invalidate();
            _lblCapNhat.Text = "Cập nhật lúc " + DateTime.Now.ToString("HH:mm:ss");
        }
        else
        {
            MessageBox.Show(tk.ThongBao, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        var theoLoai = AppServices.ThongKe.LayTheoTheLoai();
        if (theoLoai.ThanhCong && theoLoai.DuLieu is not null)
        {
            _grid.DataSource = theoLoai.DuLieu.ToList();
        }
    }
}
