# Quản lý Thư viện — Bài thực hành Buổi 5

Ứng dụng **Windows Forms (C# / .NET 8)** quản lý thư viện, kết nối **Microsoft SQL Server** bằng
**ADO.NET**, kiến trúc nhiều tầng, ghi log ra file văn bản, có **Unit Test** và quản lý mã nguồn bằng Git.

---

## 1. Kiến trúc

```
LibraryUI  ──▶  LibraryBLL  ──▶  LibraryDAL  ──▶  SQL Server (LibraryDB)
    │               │                │
    └───────────────┴────────────────┴──▶  LibraryDTO  (đối tượng dữ liệu dùng chung)
                                  └──────▶  LibraryCommon (ghi log)
```

| Project | Vai trò | Phụ thuộc |
|---|---|---|
| `LibraryDTO` | Các lớp dữ liệu: `Sach`, `DocGia`, `PhieuMuon`, `ThongKe`, `KetQua` | — |
| `LibraryCommon` | `Logger` ghi log ra file `.txt` | — |
| `LibraryDAL` | Truy cập dữ liệu bằng `SqlConnection` / `SqlCommand` / `SqlDataReader`; khai báo interface `ISachRepository`… | DTO, Common |
| `LibraryBLL` | Quy tắc nghiệp vụ, kiểm tra dữ liệu, xử lý lỗi; chỉ làm việc với interface của DAL | DTO, DAL, Common |
| `LibraryUI` | Giao diện WinForms (4 màn hình + 3 hộp thoại) | BLL, DAL, DTO, Common |
| `LibraryTests` | Unit Test (MSTest) cho toàn bộ nghiệp vụ | BLL, DAL (interface), DTO |

Điểm quan trọng: **BLL không biết SQL Server tồn tại**. Nó nhận `ISachRepository`… qua constructor,
nên Unit Test thay được bằng lớp giả (`Fakes.cs`) mà không cần cài SQL Server.

## 2. Chức năng

| Nhóm | Chức năng |
|---|---|
| Quản lý sách | Xem danh sách, thêm, sửa, xóa, tìm theo **tên sách / tác giả / thể loại**; xem chi tiết (số lượng, đang mượn, còn lại) |
| Quản lý độc giả | Xem, thêm, sửa, xóa, tìm theo họ tên / mã / lớp |
| Mượn – trả | Tạo phiếu mượn (chọn độc giả, sách, ngày mượn, hạn trả), ghi nhận ngày trả, lọc theo trạng thái |
| Thống kê | Tổng đầu sách, tổng số cuốn, số cuốn còn có thể mượn, số phiếu đang mượn, số phiếu quá hạn, số độc giả, số sách theo thể loại |

**Quy tắc nghiệp vụ đã cài đặt**

- **Không cho mượn khi hết sách**: `Số lượng − số đang mượn = 0` thì chặn, báo rõ trên form.
- Không xóa sách / độc giả khi còn phiếu mượn chưa trả.
- Không giảm số lượng sách xuống dưới số cuốn đang cho mượn.
- Hạn trả ≥ ngày mượn và không quá 60 ngày; ngày mượn không ở tương lai.
- Ngày trả không được trước ngày mượn; phiếu đã trả thì không trả lại lần nữa.
- Kiểm tra mã trùng, mã rỗng, tên rỗng, email sai định dạng, năm xuất bản ngoài khoảng 1900..năm sau.

## 3. Cài đặt và chạy

### Bước 1 — Tạo cơ sở dữ liệu

```powershell
sqlcmd -S .\SQLEXPRESS05 -E -i database\LibraryDB.sql
```

Script tạo database `LibraryDB` gồm 3 bảng (`Sach`, `DocGia`, `PhieuMuon`), khóa ngoại, ràng buộc
CHECK và dữ liệu mẫu (12 đầu sách, 8 độc giả, 9 phiếu mượn).

> **Lưu ý:** file `.sql` chứa tiếng Việt nên phải lưu **UTF-8 có BOM** trước khi chạy bằng `sqlcmd`,
> nếu không dữ liệu sẽ bị lỗi font.

### Bước 2 — Chỉnh chuỗi kết nối (nếu máy khác)

Mở `src/LibraryUI/App.config`, sửa `Data Source` cho đúng tên máy chủ SQL Server của bạn:

```xml
<add name="LibraryDB"
     connectionString="Server=.\SQLEXPRESS05;Database=LibraryDB;Integrated Security=True;TrustServerCertificate=True"
     providerName="Microsoft.Data.SqlClient" />
```

### Bước 3 — Chạy ứng dụng

Mở `LibraryManagement.sln` bằng Visual Studio 2022 → chuột phải `LibraryUI` → **Set as Startup
Project** → `F5`. Hoặc bằng dòng lệnh:

```powershell
dotnet run --project src/LibraryUI
```

## 4. Ghi log

- File log: `Logs/Library_yyyyMMdd.txt` **nằm cạnh file chạy** (`bin/Debug/net8.0-windows/Logs`).
- Mỗi dòng: `thời gian | mức | nội dung`; mức dùng: `INFO`, `WARN`, `ERROR`, `CRIT`.
- Ghi các sự kiện: khởi động/đóng ứng dụng, thêm–sửa–xóa sách và độc giả, tạo phiếu mượn, trả sách,
  tìm kiếm, dữ liệu không hợp lệ, lỗi kết nối SQL Server (kèm `Number`, `Class` và Stack Trace).
- `Logger` **không bao giờ ném exception**: nếu không ghi được file thì tự tắt im lặng, để lỗi log
  không làm chết ứng dụng.

## 5. Unit Test

```powershell
dotnet test
```

- **42 test**, tất cả Passed, chạy bằng MSTest runner trong cùng process (không cần SQL Server).
- Phủ: thêm/sửa/xóa sách, kiểm tra mã trùng, số lượng âm, năm xuất bản sai; độc giả (email, mã,
  xóa khi còn phiếu); mượn–trả (hết sách, hạn trả, ngày tương lai, trả trước ngày mượn, trả hai lần);
  thống kê.
- Lớp giả nằm ở `tests/LibraryTests/Fakes.cs` — mô phỏng `FakeDb` trong bộ nhớ.

## 6. Cấu trúc thư mục

```
LibraryManagement.sln
database/LibraryDB.sql          # script tạo CSDL + dữ liệu mẫu
src/LibraryCommon/              # Logger
src/LibraryDTO/                 # đối tượng dữ liệu
src/LibraryDAL/                 # DAL + interface
src/LibraryBLL/                 # nghiệp vụ
src/LibraryUI/                  # WinForms
tests/LibraryTests/             # MSTest + fake repository
docs/shots/                     # ảnh chụp màn hình
```
