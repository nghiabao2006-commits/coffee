IF OBJECT_ID(N'dbo.VaiTro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.VaiTro
    (
        MaVaiTro INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_VaiTro PRIMARY KEY,
        TenVaiTro NVARCHAR(50) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.NhanVien', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NhanVien
    (
        MaNV INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_NhanVien PRIMARY KEY,
        TenNV NVARCHAR(100) NOT NULL,
        TenDangNhap VARCHAR(50) NOT NULL,
        MatKhau VARCHAR(64) NOT NULL,
        MaVaiTro INT NOT NULL,
        DienThoai VARCHAR(15) NULL,
        DiaChi NVARCHAR(200) NULL,
        NgayVaoLam DATE NULL,
        TrangThai NVARCHAR(20) NOT NULL CONSTRAINT DF_NhanVien_TrangThai DEFAULT (N'Đang làm'),
        CONSTRAINT FK_NhanVien_VaiTro FOREIGN KEY (MaVaiTro) REFERENCES dbo.VaiTro (MaVaiTro)
    );
    CREATE UNIQUE INDEX UX_NhanVien_TenDangNhap ON dbo.NhanVien (TenDangNhap);
END
GO

IF OBJECT_ID(N'dbo.KhuVuc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.KhuVuc
    (
        MaKhuVuc INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_KhuVuc PRIMARY KEY,
        TenKhuVuc NVARCHAR(50) NOT NULL,
        MoTa NVARCHAR(200) NULL
    );
    CREATE UNIQUE INDEX UX_KhuVuc_TenKhuVuc ON dbo.KhuVuc (TenKhuVuc);
END
GO

IF OBJECT_ID(N'dbo.Ban', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Ban
    (
        MaBan INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Ban PRIMARY KEY,
        TenBan NVARCHAR(20) NOT NULL,
        MaKhuVuc INT NOT NULL,
        SoCho INT NOT NULL CONSTRAINT DF_Ban_SoCho DEFAULT (4),
        TrangThai NVARCHAR(20) NOT NULL CONSTRAINT DF_Ban_TrangThai DEFAULT (N'Trống'),
        CONSTRAINT FK_Ban_KhuVuc FOREIGN KEY (MaKhuVuc) REFERENCES dbo.KhuVuc (MaKhuVuc),
        CONSTRAINT UQ_Ban_TenKhuVuc UNIQUE (TenBan, MaKhuVuc)
    );
END
GO

IF OBJECT_ID(N'dbo.LoaiMon', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LoaiMon
    (
        MaLoai INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_LoaiMon PRIMARY KEY,
        TenLoai NVARCHAR(50) NOT NULL,
        MoTa NVARCHAR(200) NULL
    );
    CREATE UNIQUE INDEX UX_LoaiMon_TenLoai ON dbo.LoaiMon (TenLoai);
END
GO

IF OBJECT_ID(N'dbo.Mon', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Mon
    (
        MaMon INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Mon PRIMARY KEY,
        TenMon NVARCHAR(100) NOT NULL,
        MaLoai INT NOT NULL,
        Gia DECIMAL(18, 0) NOT NULL,
        DonViTinh NVARCHAR(20) NOT NULL CONSTRAINT DF_Mon_DonViTinh DEFAULT (N'Ly'),
        TrangThai BIT NOT NULL CONSTRAINT DF_Mon_TrangThai DEFAULT (1),
        MoTa NVARCHAR(200) NULL,
        CONSTRAINT FK_Mon_LoaiMon FOREIGN KEY (MaLoai) REFERENCES dbo.LoaiMon (MaLoai)
    );
    CREATE INDEX IX_Mon_MaLoai ON dbo.Mon (MaLoai);
END
GO

IF OBJECT_ID(N'dbo.HoaDon', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HoaDon
    (
        MaHD INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_HoaDon PRIMARY KEY,
        MaBan INT NOT NULL,
        MaNV INT NOT NULL,
        NgayLap DATETIME NOT NULL CONSTRAINT DF_HoaDon_NgayLap DEFAULT (GETDATE()),
        NgayThanhToan DATETIME NULL,
        TongTien DECIMAL(18, 0) NOT NULL CONSTRAINT DF_HoaDon_TongTien DEFAULT (0),
        GiamGia DECIMAL(18, 0) NOT NULL CONSTRAINT DF_HoaDon_GiamGia DEFAULT (0),
        ThanhTien AS (TongTien - GiamGia) PERSISTED,
        PhuongThucThanhToan NVARCHAR(30) NULL,
        TrangThai NVARCHAR(30) NOT NULL CONSTRAINT DF_HoaDon_TrangThai DEFAULT (N'Chưa thanh toán'),
        GhiChu NVARCHAR(250) NULL,
        CONSTRAINT FK_HoaDon_Ban FOREIGN KEY (MaBan) REFERENCES dbo.Ban (MaBan),
        CONSTRAINT FK_HoaDon_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien (MaNV)
    );
    CREATE INDEX IX_HoaDon_NgayThanhToan ON dbo.HoaDon (NgayThanhToan, TrangThai);
    CREATE INDEX IX_HoaDon_MaBan ON dbo.HoaDon (MaBan, TrangThai);
END
GO

IF OBJECT_ID(N'dbo.ChiTietHoaDon', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChiTietHoaDon
    (
        MaCT INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY,
        MaHD INT NOT NULL,
        MaMon INT NOT NULL,
        SoLuong INT NOT NULL,
        DonGia DECIMAL(18, 0) NOT NULL,
        ThanhTien AS (SoLuong * DonGia) PERSISTED,
        GhiChu NVARCHAR(200) NULL,
        CONSTRAINT FK_ChiTietHoaDon_HoaDon FOREIGN KEY (MaHD) REFERENCES dbo.HoaDon (MaHD) ON DELETE CASCADE,
        CONSTRAINT FK_ChiTietHoaDon_Mon FOREIGN KEY (MaMon) REFERENCES dbo.Mon (MaMon)
    );
    CREATE INDEX IX_ChiTietHoaDon_MaHD ON dbo.ChiTietHoaDon (MaHD);
    CREATE INDEX IX_ChiTietHoaDon_MaMon ON dbo.ChiTietHoaDon (MaMon);
END
GO

IF OBJECT_ID(N'dbo.CaiDat', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CaiDat
    (
        Khoa NVARCHAR(50) NOT NULL CONSTRAINT PK_CaiDat PRIMARY KEY,
        GiaTri NVARCHAR(250) NULL
    );
END
GO
