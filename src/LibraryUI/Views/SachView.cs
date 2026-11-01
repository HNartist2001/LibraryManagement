using LibraryDTO;

namespace LibraryUI;

/// <summary>Màn hình Quản lý sách: lưới sách + thanh công cụ tìm kiếm + thẻ thông tin sách.</summary>
public class SachView : UserControl, IRefreshableView
{
    private readonly DataGridView _grid = new();
    private readonly TextBox _txtTimKiem = new();
    private readonly ComboBox _cboTheLoai = new();
    private readonly Label _lblDem = new();
    private readonly RoundedButton _btnSua = new();
    private readonly RoundedButton _btnXoa = new();

    private readonly Label _lblTenSach = new();
    private readonly Label _lblMa = new();
    private readonly Label _lblTacGia = new();
    private readonly Label _lblTheLoai = new();
    private readonly Label _lblNam = new();
    private readonly Label _lblSoLuong = new();
    private readonly Label _lblDangMuon = new();
    private readonly Label _lblConLai = new();
    private readonly Label _lblTinhTrang = new();
    private readonly RoundedButton _btnMuon = new();

    private List<SachHienThi> _duLieu = new();
    private SachHienThi? _dangChon;
    private string _theLoaiDangLoc = string.Empty;

    public SachView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Bg;
        Font = Theme.Base;

        var thanhCongCu = TaoThanhCongCu();
        var thanhDuoi = TaoThanhDuoi();
        var cardChiTiet = TaoTheChiTiet();
        var cardLuoi = TaoCardLuoi();

        // TableLayoutPanel: cột trái co giãn, cột phải cố định 320 -> không phụ thuộc thứ tự dock.
        var bang = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.Bg,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0),
            Padding = new Padding(0, 0, 0, 8)
        };
        bang.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        bang.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320f));
        bang.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        cardLuoi.Dock = DockStyle.Fill;
        cardChiTiet.Dock = DockStyle.Fill;
        cardLuoi.Margin = new Padding(0, 0, 10, 0);
        cardChiTiet.Margin = new Padding(0);

        bang.Controls.Add(cardLuoi, 0, 0);
        bang.Controls.Add(cardChiTiet, 1, 0);

        Controls.Add(bang);
        Controls.Add(thanhDuoi);
        Controls.Add(thanhCongCu);

        Load += (s, e) => RefreshData();
        _grid.SelectionChanged += (s, e) => CapNhatChiTiet();
        _grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) MoSua(); };
    }

    // ------------------------------------------------------------------ thanh công cụ
    private Control TaoThanhCongCu()
    {
        var bar = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Theme.Bg };

        var card = new Card { Dock = DockStyle.Fill, Padding = new Padding(12, 7, 12, 7) };

        // Hai dải FlowLayoutPanel: trái = tìm kiếm, phải = hành động. WinForms tự xếp, không tính toạ độ.
        var daiTrai = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent,
            Margin = new Padding(0)
        };

        var daiPhai = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = Color.Transparent,
            Margin = new Padding(0)
        };

        _txtTimKiem.PlaceholderText = "Nhập tên sách hoặc tác giả…";
        _txtTimKiem.BorderStyle = BorderStyle.FixedSingle;
        _txtTimKiem.Font = Theme.Base;
        _txtTimKiem.Size = new Size(230, 28);
        _txtTimKiem.Margin = new Padding(0, 4, 8, 0);
        _txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) TimKiem(); };

        _cboTheLoai.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboTheLoai.Font = Theme.Base;
        _cboTheLoai.FlatStyle = FlatStyle.Flat;
        _cboTheLoai.Size = new Size(150, 28);
        _cboTheLoai.Margin = new Padding(0, 4, 8, 0);
        _cboTheLoai.SelectedIndexChanged += (s, e) => TimKiem();

        var btnTim = new RoundedButton
        {
            Text = "Tìm kiếm", Icon = "\uE721", Size = new Size(108, 34),
            MauNen = Theme.Primary, Margin = new Padding(0, 1, 8, 0)
        };
        btnTim.Click += (s, e) => TimKiem();

        var btnLamMoi = new RoundedButton
        {
            Text = "Làm mới", Icon = "\uE72C", Size = new Size(100, 34),
            MauNen = Color.White, MauChu = Theme.Text, MauVien = Theme.Border,
            Margin = new Padding(0, 1, 0, 0)
        };
        btnLamMoi.Click += (s, e) => { _txtTimKiem.Clear(); _cboTheLoai.SelectedIndex = 0; RefreshData(); };

        var btnThem = new RoundedButton
        {
            Text = "Thêm sách", Icon = "\uE710", Size = new Size(116, 34),
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
        _btnXoa.Click += (s, e) => XoaSach();

        daiTrai.Controls.Add(_txtTimKiem);
        daiTrai.Controls.Add(_cboTheLoai);
        daiTrai.Controls.Add(btnTim);
        daiTrai.Controls.Add(btnLamMoi);

        // FlowDirection.RightToLeft: add theo thứ tự ngược để hiển thị trái->phải đúng ý
        daiPhai.Controls.Add(_btnXoa);
        daiPhai.Controls.Add(_btnSua);
        daiPhai.Controls.Add(btnThem);

        card.Controls.Add(Theme.HangToolbar(daiTrai, daiPhai));

        bar.Controls.Add(card);
        return bar;
    }

    // ------------------------------------------------------------------ lưới
    private Control TaoCardLuoi()
    {
        Theme.StyleGrid(_grid);
        _grid.Dock = DockStyle.Fill;

        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã sách", DataPropertyName = nameof(SachHienThi.MaSach), FillWeight = 12 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên sách", DataPropertyName = nameof(SachHienThi.TenSach), FillWeight = 33 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tác giả", DataPropertyName = nameof(SachHienThi.TacGia), FillWeight = 22 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thể loại", DataPropertyName = nameof(SachHienThi.TheLoai), FillWeight = 14 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Năm XB", DataPropertyName = nameof(SachHienThi.NamXuatBan), FillWeight = 9 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số lượng", DataPropertyName = nameof(SachHienThi.SoLuong), FillWeight = 9 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đang mượn", DataPropertyName = nameof(SachHienThi.DangMuon), FillWeight = 10 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tình trạng", DataPropertyName = nameof(SachHienThi.TinhTrang), FillWeight = 19 });

        _grid.CellFormatting += (s, e) =>
        {
            if (e.RowIndex < 0) return;
            if (_grid.Columns[e.ColumnIndex].HeaderText != "Tình trạng") return;

            var sach = _grid.Rows[e.RowIndex].DataBoundItem as SachHienThi;
            if (sach is null) return;

            e.CellStyle!.ForeColor = sach.ConChoMuon ? Theme.Accent : Theme.Danger;
            e.CellStyle!.Font = Theme.Bold;
        };

        var card = new Card { Dock = DockStyle.Fill, Padding = new Padding(6, 6, 6, 6) };
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

    // ------------------------------------------------------------------ thẻ chi tiết
    private Card TaoTheChiTiet()
    {
        var card = new Card
        {
            Dock = DockStyle.Right,
            Width = 330,
            Padding = new Padding(20, 18, 20, 18)
        };

        var tieuDe = new Label
        {
            Text = "THÔNG TIN SÁCH",
            Font = new Font("Segoe UI Semibold", 8f),
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(20, 18),
            BackColor = Color.Transparent
        };

        _lblTenSach.Font = Theme.H2;
        _lblTenSach.ForeColor = Theme.Text;
        _lblTenSach.AutoSize = false;
        _lblTenSach.Size = new Size(288, 56);
        _lblTenSach.Location = new Point(20, 44);
        _lblTenSach.BackColor = Color.Transparent;

        var y = 112;
        Label Dong(string nhan, Label giaTri)
        {
            var l = new Label
            {
                Text = nhan,
                Font = Theme.Small,
                ForeColor = Theme.Muted,
                AutoSize = true,
                Location = new Point(20, y + 3),
                BackColor = Color.Transparent
            };
            giaTri.Font = Theme.Bold;
            giaTri.ForeColor = Theme.Text;
            giaTri.AutoSize = false;
            giaTri.Size = new Size(180, 20);
            giaTri.TextAlign = ContentAlignment.MiddleRight;
            giaTri.Location = new Point(128, y);
            giaTri.BackColor = Color.Transparent;
            card.Controls.Add(l);
            card.Controls.Add(giaTri);
            y += 30;
            return l;
        }

        Dong("Mã sách", _lblMa);
        Dong("Tác giả", _lblTacGia);
        Dong("Thể loại", _lblTheLoai);
        Dong("Năm xuất bản", _lblNam);
        Dong("Số lượng", _lblSoLuong);
        Dong("Đang mượn", _lblDangMuon);
        Dong("Còn lại", _lblConLai);

        _lblTinhTrang.Text = "Chọn một sách để xem chi tiết";
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

        _btnMuon.Text = "Mượn sách này";
        _btnMuon.Icon = "\uE8AB";
        _btnMuon.Size = new Size(288, 40);
        _btnMuon.Location = new Point(20, y + 54);
        _btnMuon.MauNen = Theme.Primary;
        _btnMuon.Enabled = false;
        _btnMuon.Click += (s, e) => MoMuon();

        card.Controls.Add(tieuDe);
        card.Controls.Add(_lblTenSach);
        card.Controls.Add(_lblTinhTrang);
        card.Controls.Add(_btnMuon);

        return card;
    }

    // ------------------------------------------------------------------ dữ liệu
    public void RefreshData()
    {
        NapTheLoai();
        TaiDuLieu();
    }

    private void NapTheLoai()
    {
        var ketQua = AppServices.Sach.LayTheLoai();
        if (!ketQua.ThanhCong) return;

        var dangChon = _cboTheLoai.SelectedItem as string;
        _cboTheLoai.Items.Clear();
        _cboTheLoai.Items.Add("Tất cả thể loại");
        foreach (var tl in ketQua.DuLieu ?? Array.Empty<string>()) _cboTheLoai.Items.Add(tl);

        var index = dangChon is null ? 0 : Math.Max(0, _cboTheLoai.Items.IndexOf(dangChon));
        if (_cboTheLoai.Items.Count > 0) _cboTheLoai.SelectedIndex = index;
    }

    private void TaiDuLieu(string? tuKhoa = null, string? theLoai = null)
    {
        var ketQua = tuKhoa is null && theLoai is null
            ? AppServices.Sach.LayTatCa()
            : AppServices.Sach.TimKiem(tuKhoa, theLoai);

        if (!ketQua.ThanhCong)
        {
            MessageBox.Show(ketQua.ThongBao, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _duLieu = (ketQua.DuLieu ?? Array.Empty<SachHienThi>()).ToList();
        _grid.DataSource = _duLieu;
        _lblDem.Text = $"Hiển thị {_duLieu.Count} đầu sách";

        if (_duLieu.Count == 0) _dangChon = null;
        CapNhatChiTiet();
    }

    private void TimKiem()
    {
        var tuKhoa = _txtTimKiem.Text.Trim();
        var theLoai = _cboTheLoai.SelectedIndex > 0 ? _cboTheLoai.SelectedItem as string : null;
        TaiDuLieu(tuKhoa, theLoai);
    }

    private void CapNhatChiTiet()
    {
        _dangChon = _grid.CurrentRow?.DataBoundItem as SachHienThi;

        if (_dangChon is null)
        {
            _lblTenSach.Text = "(chưa chọn sách)";
            _lblMa.Text = _lblTacGia.Text = _lblTheLoai.Text = _lblNam.Text = "—";
            _lblSoLuong.Text = _lblDangMuon.Text = _lblConLai.Text = "—";
            _lblTinhTrang.Text = "Chọn một sách để xem chi tiết";
            _lblTinhTrang.ForeColor = Theme.Muted;
            _lblTinhTrang.BackColor = Theme.Bg;
            _btnMuon.Enabled = false;
            _btnSua.Enabled = false;
            _btnXoa.Enabled = false;
            return;
        }

        _lblTenSach.Text = _dangChon.TenSach;
        _lblMa.Text = _dangChon.MaSach;
        _lblTacGia.Text = _dangChon.TacGia;
        _lblTheLoai.Text = _dangChon.TheLoai;
        _lblNam.Text = _dangChon.NamXuatBan?.ToString() ?? "—";
        _lblSoLuong.Text = _dangChon.SoLuong.ToString();
        _lblDangMuon.Text = _dangChon.DangMuon.ToString();
        _lblConLai.Text = _dangChon.ConLai.ToString();

        if (_dangChon.ConChoMuon)
        {
            _lblTinhTrang.Text = $"Còn {_dangChon.ConLai} cuốn — có thể cho mượn";
            _lblTinhTrang.ForeColor = Theme.Accent;
            _lblTinhTrang.BackColor = Theme.AccentSoft;
        }
        else
        {
            _lblTinhTrang.Text = "Đã hết sách — không thể cho mượn";
            _lblTinhTrang.ForeColor = Theme.Danger;
            _lblTinhTrang.BackColor = Theme.DangerSoft;
        }

        _lblTinhTrang.Invalidate();
        _btnMuon.Enabled = _dangChon.ConChoMuon;
        _btnSua.Enabled = true;
        _btnXoa.Enabled = true;
    }

    // ------------------------------------------------------------------ hành động
    private void MoThem()
    {
        using var dlg = new SachDialog(null);
        if (dlg.ShowDialog(this) == DialogResult.OK) TaiDuLieu();
    }

    private void MoSua()
    {
        if (_dangChon is null) return;

        using var dlg = new SachDialog(_dangChon);
        if (dlg.ShowDialog(this) == DialogResult.OK) TaiDuLieu();
    }

    private void XoaSach()
    {
        if (_dangChon is null) return;

        var traLoi = MessageBox.Show(
            $"Xóa sách \"{_dangChon.TenSach}\" ({_dangChon.MaSach})?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (traLoi != DialogResult.Yes) return;

        var ketQua = AppServices.Sach.Xoa(_dangChon.MaSach);
        MessageBox.Show(ketQua.ThongBao, ketQua.ThanhCong ? "Thành công" : "Không thể xóa",
            MessageBoxButtons.OK, ketQua.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (ketQua.ThanhCong) TaiDuLieu();
    }

    private void MoMuon()
    {
        if (_dangChon is null) return;

        using var dlg = new MuonDialog(_dangChon.MaSach);
        if (dlg.ShowDialog(this) == DialogResult.OK) TaiDuLieu();
    }
}
