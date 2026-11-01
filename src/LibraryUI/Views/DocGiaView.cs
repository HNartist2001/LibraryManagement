using LibraryDTO;

namespace LibraryUI;

/// <summary>Màn hình Quản lý độc giả.</summary>
public class DocGiaView : UserControl, IRefreshableView
{
    private readonly DataGridView _grid = new();
    private readonly TextBox _txtTimKiem = new();
    private readonly Label _lblDem = new();
    private readonly RoundedButton _btnSua = new();
    private readonly RoundedButton _btnXoa = new();

    private readonly Label _lblHoTen = new();
    private readonly Label _lblMa = new();
    private readonly Label _lblLop = new();
    private readonly Label _lblEmail = new();
    private readonly Label _lblDangMuon = new();
    private readonly Label _lblTinhTrang = new();
    private readonly RoundedButton _btnMuon = new();

    private List<DocGiaHienThi> _duLieu = new();
    private DocGiaHienThi? _dangChon;

    public DocGiaView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Bg;
        Font = Theme.Base;

        var thanhCongCu = TaoThanhCongCu();
        var thanhDuoi = TaoThanhDuoi();
        var cardChiTiet = TaoTheChiTiet();
        var cardLuoi = TaoCardLuoi();

        var than = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.Bg,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0),
            Padding = new Padding(0, 0, 0, 8)
        };
        than.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        than.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320f));
        than.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        cardLuoi.Dock = DockStyle.Fill;
        cardChiTiet.Dock = DockStyle.Fill;
        cardLuoi.Margin = new Padding(0, 0, 10, 0);
        cardChiTiet.Margin = new Padding(0);

        than.Controls.Add(cardLuoi, 0, 0);
        than.Controls.Add(cardChiTiet, 1, 0);

        Controls.Add(than);
        Controls.Add(thanhDuoi);
        Controls.Add(thanhCongCu);

        _grid.SelectionChanged += (s, e) => CapNhatChiTiet();
        _grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) MoSua(); };
    }

    private Control TaoThanhCongCu()
    {
        var bar = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Theme.Bg };
        var card = new Card { Dock = DockStyle.Fill, Padding = new Padding(12, 7, 12, 7) };

        var daiTrai = new FlowLayoutPanel
        {
            Dock = DockStyle.Left, AutoSize = true, WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight, BackColor = Color.Transparent, Margin = new Padding(0)
        };
        var daiPhai = new FlowLayoutPanel
        {
            Dock = DockStyle.Right, AutoSize = true, WrapContents = false,
            FlowDirection = FlowDirection.RightToLeft, BackColor = Color.Transparent, Margin = new Padding(0)
        };

        _txtTimKiem.PlaceholderText = "Nhập họ tên, mã hoặc lớp…";
        _txtTimKiem.BorderStyle = BorderStyle.FixedSingle;
        _txtTimKiem.Font = Theme.Base;
        _txtTimKiem.Size = new Size(250, 28);
        _txtTimKiem.Margin = new Padding(0, 4, 8, 0);
        _txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) TimKiem(); };

        var btnTim = new RoundedButton
        {
            Text = "Tìm kiếm", Icon = "\uE721", Size = new Size(108, 34),
            MauNen = Theme.Primary, Margin = new Padding(0, 1, 8, 0)
        };
        btnTim.Click += (s, e) => TimKiem();

        var btnLamMoi = new RoundedButton
        {
            Text = "Làm mới", Icon = "\uE72C", Size = new Size(100, 34),
            MauNen = Color.White, MauChu = Theme.Text, MauVien = Theme.Border, Margin = new Padding(0, 1, 0, 0)
        };
        btnLamMoi.Click += (s, e) => { _txtTimKiem.Clear(); TaiDuLieu(); };

        var btnThem = new RoundedButton
        {
            Text = "Thêm độc giả", Icon = "\uE710", Size = new Size(134, 34),
            MauNen = Theme.Primary, Margin = new Padding(10, 1, 0, 0)
        };
        btnThem.Click += (s, e) => MoThem();

        _btnSua.Text = "Cập nhật";
        _btnSua.Icon = "\uE70F";
        _btnSua.Size = new Size(108, 34);
        _btnSua.MauNen = Color.White;
        _btnSua.MauChu = Theme.Text;
        _btnSua.MauVien = Theme.Border;
        _btnSua.Margin = new Padding(8, 1, 0, 0);
        _btnSua.Click += (s, e) => MoSua();

        _btnXoa.Text = "Xóa";
        _btnXoa.Icon = "\uE74D";
        _btnXoa.Size = new Size(80, 34);
        _btnXoa.MauNen = Theme.DangerSoft;
        _btnXoa.MauChu = Theme.Danger;
        _btnXoa.Margin = new Padding(8, 1, 0, 0);
        _btnXoa.Click += (s, e) => XoaDocGia();

        daiTrai.Controls.Add(_txtTimKiem);
        daiTrai.Controls.Add(btnTim);
        daiTrai.Controls.Add(btnLamMoi);

        daiPhai.Controls.Add(_btnXoa);
        daiPhai.Controls.Add(_btnSua);
        daiPhai.Controls.Add(btnThem);

        card.Controls.Add(Theme.HangToolbar(daiTrai, daiPhai));

        bar.Controls.Add(card);
        return bar;
    }

    private Control TaoCardLuoi()
    {
        Theme.StyleGrid(_grid);
        _grid.Dock = DockStyle.Fill;

        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã độc giả", DataPropertyName = nameof(DocGiaHienThi.MaDocGia), FillWeight = 14 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Họ và tên", DataPropertyName = nameof(DocGiaHienThi.HoTen), FillWeight = 30 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Lớp", DataPropertyName = nameof(DocGiaHienThi.Lop), FillWeight = 14 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Email", DataPropertyName = nameof(DocGiaHienThi.Email), FillWeight = 30 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đang mượn", DataPropertyName = nameof(DocGiaHienThi.DangMuon), FillWeight = 10 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tình trạng", DataPropertyName = nameof(DocGiaHienThi.TinhTrang), FillWeight = 18 });

        var card = new Card { Dock = DockStyle.Fill, Padding = new Padding(6) };
        card.Controls.Add(_grid);
        return card;
    }

    private Control TaoThanhDuoi()
    {
        var bar = new Panel { Dock = DockStyle.Bottom, Height = 30, BackColor = Theme.Bg };
        _lblDem.Font = Theme.Small;
        _lblDem.ForeColor = Theme.Muted;
        _lblDem.AutoSize = true;
        _lblDem.Location = new Point(4, 6);
        bar.Controls.Add(_lblDem);
        return bar;
    }

    private Card TaoTheChiTiet()
    {
        var card = new Card { Dock = DockStyle.Right, Width = 330, Padding = new Padding(20, 18, 20, 18) };

        var tieuDe = new Label
        {
            Text = "THÔNG TIN ĐỘC GIẢ",
            Font = new Font("Segoe UI Semibold", 8f),
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(20, 18),
            BackColor = Color.Transparent
        };

        _lblHoTen.Font = Theme.H2;
        _lblHoTen.ForeColor = Theme.Text;
        _lblHoTen.AutoSize = false;
        _lblHoTen.Size = new Size(288, 56);
        _lblHoTen.Location = new Point(20, 44);
        _lblHoTen.BackColor = Color.Transparent;

        var y = 112;
        void Dong(string nhan, Label giaTri)
        {
            card.Controls.Add(new Label
            {
                Text = nhan,
                Font = Theme.Small,
                ForeColor = Theme.Muted,
                AutoSize = true,
                Location = new Point(20, y + 3),
                BackColor = Color.Transparent
            });

            giaTri.Font = Theme.Bold;
            giaTri.ForeColor = Theme.Text;
            giaTri.AutoSize = false;
            giaTri.Size = new Size(180, 20);
            giaTri.TextAlign = ContentAlignment.MiddleRight;
            giaTri.Location = new Point(128, y);
            giaTri.BackColor = Color.Transparent;
            card.Controls.Add(giaTri);
            y += 30;
        }

        Dong("Mã độc giả", _lblMa);
        Dong("Lớp", _lblLop);
        Dong("Email", _lblEmail);
        Dong("Đang mượn", _lblDangMuon);

        _lblTinhTrang.Text = "Chọn một độc giả để xem chi tiết";
        _lblTinhTrang.Font = Theme.Bold;
        _lblTinhTrang.ForeColor = Theme.Muted;
        _lblTinhTrang.TextAlign = ContentAlignment.MiddleCenter;
        _lblTinhTrang.AutoSize = false;
        _lblTinhTrang.Size = new Size(288, 34);
        _lblTinhTrang.Location = new Point(20, y + 8);
        _lblTinhTrang.BackColor = Theme.Bg;
        _lblTinhTrang.SizeChanged += (s, e) =>
        {
            using var path = Theme.BoTron(new Rectangle(0, 0, _lblTinhTrang.Width, _lblTinhTrang.Height), 10);
            _lblTinhTrang.Region = new Region(path);
        };

        _btnMuon.Text = "Tạo phiếu mượn";
        _btnMuon.Icon = "\uE8AB";
        _btnMuon.Size = new Size(288, 40);
        _btnMuon.Location = new Point(20, y + 54);
        _btnMuon.MauNen = Theme.Primary;
        _btnMuon.Enabled = false;
        _btnMuon.Click += (s, e) => MoMuon();

        card.Controls.Add(tieuDe);
        card.Controls.Add(_lblHoTen);
        card.Controls.Add(_lblTinhTrang);
        card.Controls.Add(_btnMuon);
        return card;
    }

    public void RefreshData() => TaiDuLieu();

    private void TaiDuLieu(string? tuKhoa = null)
    {
        var ketQua = tuKhoa is null ? AppServices.DocGia.LayTatCa() : AppServices.DocGia.TimKiem(tuKhoa);
        if (!ketQua.ThanhCong)
        {
            MessageBox.Show(ketQua.ThongBao, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _duLieu = (ketQua.DuLieu ?? Array.Empty<DocGiaHienThi>()).ToList();
        _grid.DataSource = _duLieu;
        _lblDem.Text = $"Hiển thị {_duLieu.Count} độc giả";

        if (_duLieu.Count == 0) _dangChon = null;
        CapNhatChiTiet();
    }

    private void TimKiem() => TaiDuLieu(_txtTimKiem.Text.Trim());

    private void CapNhatChiTiet()
    {
        _dangChon = _grid.CurrentRow?.DataBoundItem as DocGiaHienThi;

        if (_dangChon is null)
        {
            _lblHoTen.Text = "(chưa chọn độc giả)";
            _lblMa.Text = _lblLop.Text = _lblEmail.Text = _lblDangMuon.Text = "—";
            _lblTinhTrang.Text = "Chọn một độc giả để xem chi tiết";
            _lblTinhTrang.ForeColor = Theme.Muted;
            _lblTinhTrang.BackColor = Theme.Bg;
            _btnMuon.Enabled = false;
            _btnSua.Enabled = false;
            _btnXoa.Enabled = false;
            return;
        }

        _lblHoTen.Text = _dangChon.HoTen;
        _lblMa.Text = _dangChon.MaDocGia;
        _lblLop.Text = string.IsNullOrEmpty(_dangChon.Lop) ? "—" : _dangChon.Lop;
        _lblEmail.Text = string.IsNullOrEmpty(_dangChon.Email) ? "—" : _dangChon.Email;
        _lblDangMuon.Text = _dangChon.DangMuon.ToString();

        if (_dangChon.DangMuon == 0)
        {
            _lblTinhTrang.Text = "Không có sách đang mượn";
            _lblTinhTrang.ForeColor = Theme.Accent;
            _lblTinhTrang.BackColor = Theme.AccentSoft;
        }
        else
        {
            _lblTinhTrang.Text = $"Đang mượn {_dangChon.DangMuon} cuốn";
            _lblTinhTrang.ForeColor = Theme.Warning;
            _lblTinhTrang.BackColor = Theme.WarningSoft;
        }

        _lblTinhTrang.Invalidate();
        _btnMuon.Enabled = true;
        _btnSua.Enabled = true;
        _btnXoa.Enabled = true;
    }

    private void MoThem()
    {
        using var dlg = new DocGiaDialog(null);
        if (dlg.ShowDialog(this) == DialogResult.OK) TaiDuLieu();
    }

    private void MoSua()
    {
        if (_dangChon is null) return;

        using var dlg = new DocGiaDialog(_dangChon);
        if (dlg.ShowDialog(this) == DialogResult.OK) TaiDuLieu();
    }

    private void XoaDocGia()
    {
        if (_dangChon is null) return;

        var traLoi = MessageBox.Show($"Xóa độc giả \"{_dangChon.HoTen}\" ({_dangChon.MaDocGia})?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (traLoi != DialogResult.Yes) return;

        var ketQua = AppServices.DocGia.Xoa(_dangChon.MaDocGia);
        MessageBox.Show(ketQua.ThongBao, ketQua.ThanhCong ? "Thành công" : "Không thể xóa",
            MessageBoxButtons.OK, ketQua.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (ketQua.ThanhCong) TaiDuLieu();
    }

    private void MoMuon()
    {
        if (_dangChon is null) return;

        using var dlg = new MuonDialog(null, _dangChon.MaDocGia);
        if (dlg.ShowDialog(this) == DialogResult.OK) TaiDuLieu();
    }
}
