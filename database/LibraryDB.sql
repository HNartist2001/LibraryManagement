/* ============================================================
   LibraryDB - Cơ sở dữ liệu cho bài thực hành Buổi 5
   Chạy:  sqlcmd -S .\SQLEXPRESS05 -E -i LibraryDB.sql
   Script này xóa và tạo lại database (dùng cho môi trường học tập).
   ============================================================ */

IF DB_ID('LibraryDB') IS NOT NULL
BEGIN
    ALTER DATABASE LibraryDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LibraryDB;
END
GO

CREATE DATABASE LibraryDB;
GO

USE LibraryDB;
GO

/* ---------------- Sách ---------------- */
CREATE TABLE Sach
(
    MaSach      NVARCHAR(10)  NOT NULL CONSTRAINT PK_Sach PRIMARY KEY,
    TenSach     NVARCHAR(150) NOT NULL,
    TacGia      NVARCHAR(100) NOT NULL,
    TheLoai     NVARCHAR(50)  NOT NULL,
    NamXuatBan  INT           NULL,
    SoLuong     INT           NOT NULL CONSTRAINT CK_Sach_SoLuong CHECK (SoLuong >= 0)
);
GO

/* ---------------- Độc giả ---------------- */
CREATE TABLE DocGia
(
    MaDocGia  NVARCHAR(10)  NOT NULL CONSTRAINT PK_DocGia PRIMARY KEY,
    HoTen     NVARCHAR(100) NOT NULL,
    Lop       NVARCHAR(20)  NULL,
    Email     NVARCHAR(100) NULL
);
GO

/* ---------------- Phiếu mượn ----------------
   NgayTra IS NULL  => phiếu đang mượn (sách chưa trả)
   --------------------------------------------- */
CREATE TABLE PhieuMuon
(
    MaPhieu   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PhieuMuon PRIMARY KEY,
    MaSach    NVARCHAR(10) NOT NULL,
    MaDocGia  NVARCHAR(10) NOT NULL,
    NgayMuon  DATE         NOT NULL,
    HanTra    DATE         NOT NULL,
    NgayTra   DATE         NULL,
    CONSTRAINT FK_PhieuMuon_Sach   FOREIGN KEY (MaSach)   REFERENCES Sach(MaSach),
    CONSTRAINT FK_PhieuMuon_DocGia FOREIGN KEY (MaDocGia) REFERENCES DocGia(MaDocGia),
    CONSTRAINT CK_PhieuMuon_HanTra CHECK (HanTra >= NgayMuon)
);
GO

CREATE INDEX IX_PhieuMuon_MaSach   ON PhieuMuon(MaSach)   INCLUDE (NgayTra);
CREATE INDEX IX_PhieuMuon_MaDocGia ON PhieuMuon(MaDocGia);
GO

/* ---------------- Dữ liệu mẫu ---------------- */
INSERT INTO Sach (MaSach, TenSach, TacGia, TheLoai, NamXuatBan, SoLuong) VALUES
 (N'S001', N'Lập trình C# căn bản',                    N'Nguyễn Văn Hùng',  N'CNTT',      2023, 10),
 (N'S002', N'Cơ sở dữ liệu SQL Server',                N'Trần Thị Mai',     N'CNTT',      2022,  8),
 (N'S003', N'Giải thuật và Cấu trúc dữ liệu',          N'Lê Minh Tuấn',     N'CNTT',      2021,  7),
 (N'S004', N'Quản trị dự án phần mềm',                 N'Phạm Quốc Dũng',   N'CNTT',      2020,  5),
 (N'S005', N'Kỹ năng làm việc nhóm trong CNTT',        N'Võ Thị Hà',        N'Kỹ năng',   2022, 12),
 (N'S006', N'Kiến trúc phần mềm hiện đại',             N'Đặng Thế Hân',     N'CNTT',      2024,  6),
 (N'S007', N'Tiếng Anh chuyên ngành CNTT',             N'Nguyễn Thu Trang', N'Ngoại ngữ', 2021,  9),
 (N'S008', N'Lập trình hướng đối tượng với C#',        N'Nguyễn Văn Hùng',  N'CNTT',      2023,  4),
 (N'S009', N'Phân tích và thiết kế hệ thống',          N'Lê Minh Tuấn',     N'CNTT',      2019,  6),
 (N'S010', N'Mạng máy tính căn bản',                   N'Trần Thị Mai',     N'CNTT',      2020,  5),
 (N'S011', N'Văn học Việt Nam hiện đại',               N'Nguyễn Công Hoan', N'Văn học',   2018,  3),
 (N'S012', N'Kinh tế học vi mô',                       N'Vũ Minh Khang',    N'Kinh tế',   2021,  4);

INSERT INTO DocGia (MaDocGia, HoTen, Lop, Email) VALUES
 (N'DG001', N'Nguyễn Văn An',   N'CNTT01', N'an@sv.edu.vn'),
 (N'DG002', N'Trần Thị Bình',   N'CNTT01', N'binh@sv.edu.vn'),
 (N'DG003', N'Lê Minh Châu',    N'CNTT02', N'chau@sv.edu.vn'),
 (N'DG004', N'Phạm Quốc Dũng',  N'CNTT02', N'dung@sv.edu.vn'),
 (N'DG005', N'Võ Thị Hà',       N'CNTT03', N'ha@sv.edu.vn'),
 (N'DG006', N'Đỗ Hoàng Nam',    N'CNTT03', N'nam@sv.edu.vn'),
 (N'DG007', N'Nguyễn Thu Trang',N'KT01',   N'trang@sv.edu.vn'),
 (N'DG008', N'Bùi Anh Tuấn',    N'KT01',   N'tuan@sv.edu.vn');

/* Phiếu đã trả */
INSERT INTO PhieuMuon (MaSach, MaDocGia, NgayMuon, HanTra, NgayTra) VALUES
 (N'S001', N'DG001', CAST(DATEADD(day, -40, GETDATE()) AS DATE), CAST(DATEADD(day, -26, GETDATE()) AS DATE), CAST(DATEADD(day, -28, GETDATE()) AS DATE)),
 (N'S003', N'DG002', CAST(DATEADD(day, -30, GETDATE()) AS DATE), CAST(DATEADD(day, -16, GETDATE()) AS DATE), CAST(DATEADD(day, -18, GETDATE()) AS DATE)),
 (N'S005', N'DG003', CAST(DATEADD(day, -20, GETDATE()) AS DATE), CAST(DATEADD(day,  -6, GETDATE()) AS DATE), CAST(DATEADD(day,  -9, GETDATE()) AS DATE));

/* Phiếu đang mượn (NgayTra = NULL) - có 1 phiếu quá hạn */
INSERT INTO PhieuMuon (MaSach, MaDocGia, NgayMuon, HanTra, NgayTra) VALUES
 (N'S001', N'DG002', CAST(DATEADD(day, -10, GETDATE()) AS DATE), CAST(DATEADD(day,   4, GETDATE()) AS DATE), NULL),
 (N'S002', N'DG004', CAST(DATEADD(day,  -5, GETDATE()) AS DATE), CAST(DATEADD(day,   9, GETDATE()) AS DATE), NULL),
 (N'S003', N'DG005', CAST(DATEADD(day,  -3, GETDATE()) AS DATE), CAST(DATEADD(day,  11, GETDATE()) AS DATE), NULL),
 (N'S011', N'DG006', CAST(DATEADD(day, -25, GETDATE()) AS DATE), CAST(DATEADD(day, -11, GETDATE()) AS DATE), NULL),
 (N'S012', N'DG007', CAST(DATEADD(day,  -2, GETDATE()) AS DATE), CAST(DATEADD(day,  12, GETDATE()) AS DATE), NULL),
 (N'S005', N'DG008', CAST(DATEADD(day,  -1, GETDATE()) AS DATE), CAST(DATEADD(day,  13, GETDATE()) AS DATE), NULL);
GO

PRINT N'LibraryDB đã được tạo.';
SELECT (SELECT COUNT(*) FROM Sach) AS Sach,
       (SELECT COUNT(*) FROM DocGia) AS DocGia,
       (SELECT COUNT(*) FROM PhieuMuon) AS PhieuMuon,
       (SELECT COUNT(*) FROM PhieuMuon WHERE NgayTra IS NULL) AS DangMuon;
GO
