namespace LibraryUI;

/// <summary>Các hàm dùng chung để dựng form nhập liệu cho nhanh và đẹp.</summary>
public static class DialogUi
{
    public const int Padding = 24;

    public static Label NhanO(string text, Point location) => new()
    {
        Text = text,
        Font = Theme.Small,
        ForeColor = Theme.Muted,
        AutoSize = true,
        Location = location,
        BackColor = Color.Transparent
    };

    public static TextBox OChu(Point location, int width, bool chiDoc = false) => new()
    {
        Location = location,
        Width = width,
        Font = Theme.Base,
        BorderStyle = BorderStyle.FixedSingle,
        ReadOnly = chiDoc,
        BackColor = chiDoc ? Color.FromArgb(246, 248, 252) : Color.White
    };

    public static ComboBox OChon(Point location, int width) => new()
    {
        Location = location,
        Width = width,
        Font = Theme.Base,
        DropDownStyle = ComboBoxStyle.DropDownList,
        FlatStyle = FlatStyle.Flat
    };

    public static NumericUpDown OSo(Point location, int width, int min, int max, int value) => new()
    {
        Location = location,
        Width = width,
        Font = Theme.Base,
        Minimum = min,
        Maximum = max,
        Value = Math.Clamp(value, min, max),
        BorderStyle = BorderStyle.FixedSingle
    };

    public static DateTimePicker ONgay(Point location, int width, DateTime value,
        DateTimePickerFormat format = DateTimePickerFormat.Custom, string custom = "dd/MM/yyyy") => new()
    {
        Location = location,
        Width = width,
        Font = Theme.Base,
        Format = format,
        CustomFormat = custom,
        Value = value
    };

    public static RoundedButton NutLuu(string text = "Lưu") => new()
    {
        Text = text,
        Icon = "\uE74E",
        Size = new Size(126, 40),
        MauNen = Theme.Primary,
        DialogResult = DialogResult.None
    };

    public static RoundedButton NutHuy() => new()
    {
        Text = "Hủy",
        Icon = "\uE711",
        Size = new Size(96, 40),
        MauNen = Color.White,
        MauChu = Theme.Muted,
        MauVien = Theme.Border,
        DialogResult = DialogResult.Cancel
    };

    /// <summary>Khung nền trắng bo góc cho dialog + nút Lưu/Hủy ở đáy. Trả về panel để add control vào.</summary>
    public static Panel DungDialog(Form form, string tieuDe, string phuDe, int width, int height,
        out RoundedButton nutLuu, out RoundedButton nutHuy)
    {
        form.Text = tieuDe;
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.StartPosition = FormStartPosition.CenterParent;
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        form.ShowInTaskbar = false;
        form.BackColor = Theme.Bg;
        form.Font = Theme.Base;
        form.AutoScaleMode = AutoScaleMode.Dpi;
        form.ClientSize = new Size(width, height);

        var tieuDeLbl = new Label
        {
            Text = tieuDe,
            Font = Theme.H1,
            ForeColor = Theme.Text,
            AutoSize = true,
            Location = new Point(Padding, 18),
            BackColor = Color.Transparent
        };
        var phuDeLbl = new Label
        {
            Text = phuDe,
            Font = Theme.Small,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(Padding + 2, 48),
            BackColor = Color.Transparent
        };

        var card = new Card
        {
            Location = new Point(Padding - 6, 78),
            Size = new Size(width - (Padding - 6) * 2, height - 78 - 76),
            Padding = new Padding(16),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        nutHuy = NutHuy();
        nutLuu = NutLuu();

        var nutHuyLocal = nutHuy;
        var nutLuuLocal = nutLuu;

        var footer = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 70,
            BackColor = Color.Transparent
        };
        nutHuyLocal.Location = new Point(width - Padding - 6 - 96 - 8 - 126, 12);
        nutLuuLocal.Location = new Point(width - Padding - 6 - 126, 12);
        nutHuyLocal.Anchor = nutLuuLocal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        nutHuyLocal.Click += (s, e) => { form.DialogResult = DialogResult.Cancel; form.Close(); };

        footer.Controls.Add(nutHuyLocal);
        footer.Controls.Add(nutLuuLocal);
        footer.Resize += (s, e) =>
        {
            nutLuuLocal.Left = footer.Width - 126;
            nutHuyLocal.Left = nutLuuLocal.Left - 96 - 10;
        };

        form.Controls.Add(footer);
        form.Controls.Add(card);
        form.Controls.Add(tieuDeLbl);
        form.Controls.Add(phuDeLbl);
        form.AcceptButton = nutLuu;
        form.CancelButton = nutHuy;

        return card;
    }
}
