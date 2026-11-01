using LibraryDTO;

namespace LibraryUI;

/// <summary>Form thêm / cập nhật sách.</summary>
public class SachDialog : Form
{
    private readonly TextBox _txtMa = new();
    private readonly TextBox _txtTen = new();
    private readonly TextBox _txtTacGia = new();
    private readonly ComboBox _cboTheLoai = new();
    private readonly NumericUpDown _numNam = new();
    private readonly NumericUpDown _numSoLuong = new();
    private readonly Label _lblLoi = new();
    private readonly Sach? _sachGoc;

    public SachDialog(Sach? sach)
    {
        _sachGoc = sach;
        var laSua = sach is not null;

        var card = DialogUi.DungDialog(this,
            laSua ? "Cập nhật sách" : "Thêm sách mới",
            laSua ? "Sửa thông tin đầu sách đang chọn" : "Nhập thông tin đầu sách cần thêm",
            520, 520, out var nutLuu, out _);

        var x = 16;
        var w = 440;
        var y = 14;

        card.Controls.Add(DialogUi.NhanO("Mã sách *", new Point(x, y)));

        _txtMa.SetBounds(x, y + 20, 180, 26);
        _txtMa.Font = Theme.Base;
        _txtMa.BorderStyle = BorderStyle.FixedSingle;
        _txtMa.ReadOnly = laSua;
        _txtMa.BackColor = laSua ? Color.FromArgb(246, 248, 252) : Color.White;
        if (laSua) _txtMa.Text = sach!.MaSach;
        card.Controls.Add(_txtMa);

        card.Controls.Add(DialogUi.NhanO("Năm xuất bản", new Point(x + 200, y)));
        _numNam.SetBounds(x + 200, y + 20, 120, 26);
        _numNam.Font = Theme.Base;
        _numNam.Minimum = 1900;
        _numNam.Maximum = DateTime.Today.Year + 1;
        _numNam.Value = sach?.NamXuatBan ?? DateTime.Today.Year;
        card.Controls.Add(_numNam);

        y += 62;
        card.Controls.Add(DialogUi.NhanO("Tên sách *", new Point(x, y)));
        _txtTen.SetBounds(x, y + 20, w, 26);
        _txtTen.Font = Theme.Base;
        _txtTen.BorderStyle = BorderStyle.FixedSingle;
        _txtTen.Text = sach?.TenSach ?? string.Empty;
        card.Controls.Add(_txtTen);

        y += 62;
        card.Controls.Add(DialogUi.NhanO("Tác giả *", new Point(x, y)));
        _txtTacGia.SetBounds(x, y + 20, w, 26);
        _txtTacGia.Font = Theme.Base;
        _txtTacGia.BorderStyle = BorderStyle.FixedSingle;
        _txtTacGia.Text = sach?.TacGia ?? string.Empty;
        card.Controls.Add(_txtTacGia);

        y += 62;
        card.Controls.Add(DialogUi.NhanO("Thể loại *", new Point(x, y)));
        _cboTheLoai.SetBounds(x, y + 20, 200, 26);
        _cboTheLoai.Font = Theme.Base;
        _cboTheLoai.DropDownStyle = ComboBoxStyle.DropDown;
        _cboTheLoai.FlatStyle = FlatStyle.Standard;

        var theLoai = AppServices.Sach.LayTheLoai();
        if (theLoai.ThanhCong && theLoai.DuLieu is not null)
            foreach (var tl in theLoai.DuLieu) _cboTheLoai.Items.Add(tl);
        _cboTheLoai.Text = sach?.TheLoai ?? string.Empty;
        card.Controls.Add(_cboTheLoai);

        card.Controls.Add(DialogUi.NhanO("Số lượng *", new Point(x + 220, y)));
        _numSoLuong.SetBounds(x + 220, y + 20, 120, 26);
        _numSoLuong.Font = Theme.Base;
        _numSoLuong.Maximum = 10000;
        _numSoLuong.Value = sach?.SoLuong ?? 1;
        card.Controls.Add(_numSoLuong);

        _lblLoi.SetBounds(x, y + 62, w, 34);
        _lblLoi.Font = Theme.Small;
        _lblLoi.ForeColor = Theme.Danger;
        _lblLoi.BackColor = Color.Transparent;
        card.Controls.Add(_lblLoi);

        nutLuu.Click += (s, e) => Luu();
        AcceptButton = nutLuu;
    }

    private void Luu()
    {
        var sach = new Sach
        {
            MaSach = _txtMa.Text.Trim(),
            TenSach = _txtTen.Text.Trim(),
            TacGia = _txtTacGia.Text.Trim(),
            TheLoai = _cboTheLoai.Text.Trim(),
            NamXuatBan = (int)_numNam.Value,
            SoLuong = (int)_numSoLuong.Value
        };

        var ketQua = _sachGoc is null ? AppServices.Sach.Them(sach) : AppServices.Sach.Sua(sach);

        if (!ketQua.ThanhCong)
        {
            _lblLoi.Text = ketQua.ThongBao;
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
