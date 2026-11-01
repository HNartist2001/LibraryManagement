using LibraryDTO;

namespace LibraryUI;

/// <summary>Màn hình Mượn – Trả: danh sách phiếu mượn, tạo phiếu và ghi nhận trả sách.</summary>
public class MuonTraView : UserControl, IRefreshableView
{
    private readonly DataGridView _grid = new();
    private readonly ComboBox _cboLoc = new();
    private readonly Label _lblDem = new();
    private readonly RoundedButton _btnTra = new();

    private List<PhieuMuonHienThi> _duLieu = new();
    private PhieuMuonHienThi? _dangChon;

    public MuonTraView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Bg;
        Font = Theme.Base;

        var thanhCongCu = TaoThanhCongCu();
        var thanhDuoi = TaoThanhDuoi();
        var cardLuoi = TaoCardLuoi();

        Controls.Add(cardLuoi);
        Controls.Add(thanhDuoi);
        Controls.Add(thanhCongCu);

        Load += (s, e) => RefreshData();
        _grid.SelectionChanged += (s, e) =>
        {
            _dangChon = _grid.CurrentRow?.DataBoundItem as PhieuMuonHienThi;
            _btnTra.Enabled = _dangChon is { DaTra: false };
        };
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

        _cboLoc.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboLoc.Font = Theme.Base;
        _cboLoc.FlatStyle = FlatStyle.Flat;
        _cboLoc.Size = new Size(180, 28);
        _cboLoc.Margin = new Padding(0, 4, 8, 0);
        _cboLoc.Items.AddRange(new object[] { "Tất cả phiếu", "Đang mượn", "Quá hạn", "Đã trả" });
        _cboLoc.SelectedIndex = 0;
        _cboLoc.SelectedIndexChanged += (s, e) => ApDungLoc();

        var btnLamMoi = new RoundedButton
        {
            Text = "Làm mới", Icon = "\uE72C", Size = new Size(100, 34),
            MauNen = Color.White, MauChu = Theme.Text, MauVien = Theme.Border, Margin = new Padding(0, 1, 0, 0)
        };
        btnLamMoi.Click += (s, e) => RefreshData();

        var btnTaoPhieu = new RoundedButton
        {
            Text = "Tạo phiếu mượn", Icon = "\uE710", Size = new Size(150, 34),
            MauNen = Theme.Primary, Margin = new Padding(10, 1, 0, 0)
        };
        btnTaoPhieu.Click += (s, e) => TaoPhieu();

        _btnTra.Text = "Trả sách";
        _btnTra.Icon = "\uE8FB";
        _btnTra.Size = new Size(108, 34);
        _btnTra.MauNen = Theme.Accent;
        _btnTra.Enabled = false;
        _btnTra.Margin = new Padding(8, 1, 0, 0);
        _btnTra.Click += (s, e) => TraSach();

        daiTrai.Controls.Add(_cboLoc);
        daiTrai.Controls.Add(btnLamMoi);

        daiPhai.Controls.Add(_btnTra);
        daiPhai.Controls.Add(btnTaoPhieu);

        card.Controls.Add(Theme.HangToolbar(daiTrai, daiPhai));

        bar.Controls.Add(card);
        return bar;
    }

    private Control TaoCardLuoi()
    {
        Theme.StyleGrid(_grid);
        _grid.Dock = DockStyle.Fill;

        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã phiếu", DataPropertyName = nameof(PhieuMuonHienThi.MaPhieu), FillWeight = 9 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sách", DataPropertyName = nameof(PhieuMuonHienThi.TenSach), FillWeight = 30 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Độc giả", DataPropertyName = nameof(PhieuMuonHienThi.HoTenDocGia), FillWeight = 20 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày mượn", DataPropertyName = nameof(PhieuMuonHienThi.NgayMuon), FillWeight = 12, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Hạn trả", DataPropertyName = nameof(PhieuMuonHienThi.HanTra), FillWeight = 12, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày trả", DataPropertyName = nameof(PhieuMuonHienThi.NgayTra), FillWeight = 12, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng thái", DataPropertyName = nameof(PhieuMuonHienThi.TrangThai), FillWeight = 12 });

        _grid.CellFormatting += (s, e) =>
        {
            if (e.RowIndex < 0) return;
            if (_grid.Columns[e.ColumnIndex].HeaderText != "Trạng thái") return;

            var p = _grid.Rows[e.RowIndex].DataBoundItem as PhieuMuonHienThi;
            if (p is null) return;

            e.CellStyle!.Font = Theme.Bold;
            e.CellStyle!.ForeColor = p.DaTra ? Theme.Muted : p.QuaHan ? Theme.Danger : Theme.Accent;
        };

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

    public void RefreshData()
    {
        var ketQua = AppServices.MuonTra.LayTatCa();
        if (!ketQua.ThanhCong)
        {
            MessageBox.Show(ketQua.ThongBao, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _duLieu = (ketQua.DuLieu ?? Array.Empty<PhieuMuonHienThi>()).ToList();
        ApDungLoc();
    }

    private void ApDungLoc()
    {
        IEnumerable<PhieuMuonHienThi> loc = _duLieu;

        loc = _cboLoc.SelectedIndex switch
        {
            1 => loc.Where(p => !p.DaTra),
            2 => loc.Where(p => p.QuaHan),
            3 => loc.Where(p => p.DaTra),
            _ => loc
        };

        var hienThi = loc.ToList();
        _grid.DataSource = hienThi;

        var dangMuon = _duLieu.Count(p => !p.DaTra);
        var quaHan = _duLieu.Count(p => p.QuaHan);
        _lblDem.Text = $"Hiển thị {hienThi.Count}/{_duLieu.Count} phiếu · Đang mượn: {dangMuon} · Quá hạn: {quaHan}";

        _dangChon = _grid.CurrentRow?.DataBoundItem as PhieuMuonHienThi;
        _btnTra.Enabled = _dangChon is { DaTra: false };
    }

    private void TaoPhieu()
    {
        using var dlg = new MuonDialog();
        if (dlg.ShowDialog(this) == DialogResult.OK) RefreshData();
    }

    private void TraSach()
    {
        if (_dangChon is null || _dangChon.DaTra) return;

        var traLoi = MessageBox.Show(
            $"Ghi nhận trả sách cho phiếu #{_dangChon.MaPhieu}\n" +
            $"Sách: {_dangChon.TenSach}\nĐộc giả: {_dangChon.HoTenDocGia}\n\nNgày trả: {DateTime.Today:dd/MM/yyyy}",
            "Xác nhận trả sách", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (traLoi != DialogResult.Yes) return;

        var ketQua = AppServices.MuonTra.TraSach(_dangChon.MaPhieu, DateTime.Today);
        MessageBox.Show(ketQua.ThongBao, ketQua.ThanhCong ? "Thành công" : "Không thể trả",
            MessageBoxButtons.OK, ketQua.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (ketQua.ThanhCong) RefreshData();
    }
}
