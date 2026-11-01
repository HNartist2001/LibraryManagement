using LibraryDTO;

namespace LibraryUI;

/// <summary>Form thêm / cập nhật độc giả.</summary>
public class DocGiaDialog : Form
{
    private readonly TextBox _txtMa = new();
    private readonly TextBox _txtHoTen = new();
    private readonly TextBox _txtLop = new();
    private readonly TextBox _txtEmail = new();
    private readonly Label _lblLoi = new();
    private readonly DocGia? _goc;

    public DocGiaDialog(DocGia? docGia)
    {
        _goc = docGia;
        var laSua = docGia is not null;

        var card = DialogUi.DungDialog(this,
            laSua ? "Cập nhật độc giả" : "Thêm độc giả",
            laSua ? "Sửa thông tin độc giả đang chọn" : "Nhập thông tin độc giả mới",
            500, 430, out var nutLuu, out _);

        var x = 16;
        var y = 14;

        card.Controls.Add(DialogUi.NhanO("Mã độc giả *", new Point(x, y)));
        _txtMa.SetBounds(x, y + 20, 200, 26);
        _txtMa.Font = Theme.Base;
        _txtMa.BorderStyle = BorderStyle.FixedSingle;
        _txtMa.ReadOnly = laSua;
        _txtMa.BackColor = laSua ? Color.FromArgb(246, 248, 252) : Color.White;
        if (laSua) _txtMa.Text = docGia!.MaDocGia;
        card.Controls.Add(_txtMa);

        card.Controls.Add(DialogUi.NhanO("Lớp", new Point(x + 220, y)));
        _txtLop.SetBounds(x + 220, y + 20, 180, 26);
        _txtLop.Font = Theme.Base;
        _txtLop.BorderStyle = BorderStyle.FixedSingle;
        _txtLop.Text = docGia?.Lop ?? string.Empty;
        card.Controls.Add(_txtLop);

        y += 62;
        card.Controls.Add(DialogUi.NhanO("Họ và tên *", new Point(x, y)));
        _txtHoTen.SetBounds(x, y + 20, 400, 26);
        _txtHoTen.Font = Theme.Base;
        _txtHoTen.BorderStyle = BorderStyle.FixedSingle;
        _txtHoTen.Text = docGia?.HoTen ?? string.Empty;
        card.Controls.Add(_txtHoTen);

        y += 62;
        card.Controls.Add(DialogUi.NhanO("Email", new Point(x, y)));
        _txtEmail.SetBounds(x, y + 20, 400, 26);
        _txtEmail.Font = Theme.Base;
        _txtEmail.BorderStyle = BorderStyle.FixedSingle;
        _txtEmail.Text = docGia?.Email ?? string.Empty;
        card.Controls.Add(_txtEmail);

        _lblLoi.SetBounds(x, y + 58, 400, 32);
        _lblLoi.Font = Theme.Small;
        _lblLoi.ForeColor = Theme.Danger;
        _lblLoi.BackColor = Color.Transparent;
        card.Controls.Add(_lblLoi);

        nutLuu.Click += (s, e) => Luu();
    }

    private void Luu()
    {
        var d = new DocGia
        {
            MaDocGia = _txtMa.Text.Trim(),
            HoTen = _txtHoTen.Text.Trim(),
            Lop = _txtLop.Text.Trim(),
            Email = _txtEmail.Text.Trim()
        };

        var ketQua = _goc is null ? AppServices.DocGia.Them(d) : AppServices.DocGia.Sua(d);

        if (!ketQua.ThanhCong)
        {
            _lblLoi.Text = ketQua.ThongBao;
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
