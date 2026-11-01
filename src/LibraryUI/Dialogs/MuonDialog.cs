using LibraryDTO;

namespace LibraryUI;

/// <summary>Form tạo phiếu mượn: chọn độc giả, sách, ngày mượn và hạn trả.</summary>
public class MuonDialog : Form
{
    private readonly ComboBox _cboDocGia = new();
    private readonly ComboBox _cboSach = new();
    private readonly DateTimePicker _dtNgayMuon = new();
    private readonly DateTimePicker _dtHanTra = new();
    private readonly Label _lblLoi = new();
    private readonly Label _lblThongTinSach = new();
    private readonly List<SachHienThi> _sach = new();

    public MuonDialog(string? maSach = null, string? maDocGia = null)
    {
        var card = DialogUi.DungDialog(this, "Mượn sách",
            "Tạo phiếu mượn và lưu hạn trả", 500, 470, out var nutLuu, out _);

        var x = 16;
        var y = 14;

        card.Controls.Add(DialogUi.NhanO("Độc giả *", new Point(x, y)));
        _cboDocGia.SetBounds(x, y + 20, 420, 26);
        _cboDocGia.Font = Theme.Base;
        _cboDocGia.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboDocGia.FlatStyle = FlatStyle.Standard;
        card.Controls.Add(_cboDocGia);

        y += 62;
        card.Controls.Add(DialogUi.NhanO("Sách *", new Point(x, y)));
        _cboSach.SetBounds(x, y + 20, 420, 26);
        _cboSach.Font = Theme.Base;
        _cboSach.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboSach.FlatStyle = FlatStyle.Standard;
        _cboSach.SelectedIndexChanged += (s, e) => CapNhatThongTinSach();
        card.Controls.Add(_cboSach);

        _lblThongTinSach.SetBounds(x, y + 50, 420, 22);
        _lblThongTinSach.Font = Theme.Small;
        _lblThongTinSach.ForeColor = Theme.Muted;
        _lblThongTinSach.BackColor = Color.Transparent;
        card.Controls.Add(_lblThongTinSach);

        y += 82;
        card.Controls.Add(DialogUi.NhanO("Ngày mượn", new Point(x, y)));
        _dtNgayMuon.SetBounds(x, y + 20, 190, 26);
        _dtNgayMuon.Font = Theme.Base;
        _dtNgayMuon.Format = DateTimePickerFormat.Custom;
        _dtNgayMuon.CustomFormat = "dd/MM/yyyy";
        _dtNgayMuon.Value = DateTime.Today;
        _dtNgayMuon.ValueChanged += (s, e) => GoiYHanTra();
        card.Controls.Add(_dtNgayMuon);

        card.Controls.Add(DialogUi.NhanO("Hạn trả", new Point(x + 220, y)));
        _dtHanTra.SetBounds(x + 220, y + 20, 190, 26);
        _dtHanTra.Font = Theme.Base;
        _dtHanTra.Format = DateTimePickerFormat.Custom;
        _dtHanTra.CustomFormat = "dd/MM/yyyy";
        card.Controls.Add(_dtHanTra);

        _lblLoi.SetBounds(x, y + 58, 420, 34);
        _lblLoi.Font = Theme.Small;
        _lblLoi.ForeColor = Theme.Danger;
        _lblLoi.BackColor = Color.Transparent;
        card.Controls.Add(_lblLoi);

        NapDuLieu(maSach, maDocGia);
        GoiYHanTra();

        nutLuu.Text = "Mượn sách";
        nutLuu.Click += (s, e) => Luu();
    }

    private void NapDuLieu(string? maSach, string? maDocGia)
    {
        var dsDocGia = AppServices.DocGia.LayTatCa();
        if (dsDocGia.ThanhCong && dsDocGia.DuLieu is not null)
        {
            foreach (var d in dsDocGia.DuLieu)
                _cboDocGia.Items.Add(new HienThiDocGia(d));

            if (maDocGia is not null)
            {
                for (var i = 0; i < _cboDocGia.Items.Count; i++)
                    if (((HienThiDocGia)_cboDocGia.Items[i]!).DocGia.MaDocGia == maDocGia) { _cboDocGia.SelectedIndex = i; break; }
            }

            if (_cboDocGia.SelectedIndex < 0 && _cboDocGia.Items.Count > 0) _cboDocGia.SelectedIndex = 0;
        }

        var dsSach = AppServices.Sach.LayTatCa();
        if (dsSach.ThanhCong && dsSach.DuLieu is not null)
        {
            _sach.AddRange(dsSach.DuLieu);
            foreach (var s in _sach) _cboSach.Items.Add(new HienThiSach(s));

            if (maSach is not null)
            {
                for (var i = 0; i < _cboSach.Items.Count; i++)
                    if (((HienThiSach)_cboSach.Items[i]!).Sach.MaSach == maSach) { _cboSach.SelectedIndex = i; break; }
            }

            if (_cboSach.SelectedIndex < 0 && _cboSach.Items.Count > 0) _cboSach.SelectedIndex = 0;
        }
    }

    private void GoiYHanTra()
    {
        if (_dtHanTra.Value.Date <= _dtNgayMuon.Value.Date)
            _dtHanTra.Value = _dtNgayMuon.Value.Date.AddDays(14);
    }

    private void CapNhatThongTinSach()
    {
        if (_cboSach.SelectedItem is not HienThiSach chon) return;

        var s = chon.Sach;
        _lblThongTinSach.ForeColor = s.ConChoMuon ? Theme.Muted : Theme.Danger;
        _lblThongTinSach.Text = s.ConChoMuon
            ? $"Thể loại: {s.TheLoai} · Còn {s.ConLai}/{s.SoLuong} cuốn"
            : $"Sách đã hết ({s.SoLuong}/{s.SoLuong} cuốn đang mượn) — không thể cho mượn";
    }

    private void Luu()
    {
        if (_cboDocGia.SelectedItem is not HienThiDocGia dg)
        {
            _lblLoi.Text = "Chưa chọn độc giả.";
            return;
        }

        if (_cboSach.SelectedItem is not HienThiSach s)
        {
            _lblLoi.Text = "Chưa chọn sách.";
            return;
        }

        var ketQua = AppServices.MuonTra.TaoPhieuMuon(
            s.Sach.MaSach, dg.DocGia.MaDocGia, _dtNgayMuon.Value, _dtHanTra.Value);

        if (!ketQua.ThanhCong)
        {
            _lblLoi.Text = ketQua.ThongBao;
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    /// <summary>Lớp bọc để ComboBox hiển thị "DG001 - Nguyễn Văn An · CNTT01".</summary>
    private sealed class HienThiDocGia
    {
        public DocGia DocGia { get; }
        public HienThiDocGia(DocGia d) => DocGia = d;
        public override string ToString() => $"{DocGia.MaDocGia} - {DocGia.HoTen} ({DocGia.Lop})";
    }

    private sealed class HienThiSach
    {
        public SachHienThi Sach { get; }
        public HienThiSach(SachHienThi s) => Sach = s;
        public override string ToString() => $"{Sach.MaSach} - {Sach.TenSach} (còn {Sach.ConLai})";
    }
}
