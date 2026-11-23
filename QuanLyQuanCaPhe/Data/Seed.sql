SET NOCOUNT ON;

DECLARE @VaiTroQuanLy INT;
DECLARE @VaiTroNhanVien INT;
DECLARE @KhuTangTret INT;
DECLARE @KhuSanVuon INT;
DECLARE @KhuTang1 INT;
DECLARE @LoaiCaPheViet INT;
DECLARE @LoaiCaPheY INT;
DECLARE @LoaiTra INT;
DECLARE @LoaiNuocEp INT;
DECLARE @LoaiBanh INT;
DECLARE @Ngay INT;
DECLARE @SoHoaDon INT;
DECLARE @IHoaDon INT;
DECLARE @MaHD INT;
DECLARE @MaBan INT;
DECLARE @MaNV INT;
DECLARE @ThoiDiem DATETIME;
DECLARE @J INT;
DECLARE @SoMon INT;
DECLARE @MaMon INT;
DECLARE @Gia DECIMAL(18, 0);
DECLARE @SoLuong INT;
DECLARE @TongTien DECIMAL(18, 0);
DECLARE @GiamGia DECIMAL(18, 0);

IF NOT EXISTS (SELECT 1 FROM dbo.VaiTro)
BEGIN
    INSERT INTO dbo.VaiTro (TenVaiTro) VALUES (N'Quản lý'), (N'Nhân viên');
END

SET @VaiTroQuanLy = (SELECT TOP 1 MaVaiTro FROM dbo.VaiTro WHERE TenVaiTro = N'Quản lý');
SET @VaiTroNhanVien = (SELECT TOP 1 MaVaiTro FROM dbo.VaiTro WHERE TenVaiTro = N'Nhân viên');

IF NOT EXISTS (SELECT 1 FROM dbo.NhanVien)
BEGIN
    -- Tất cả tài khoản mẫu đều dùng mật khẩu: 123456
    INSERT INTO dbo.NhanVien (TenNV, TenDangNhap, MatKhau, MaVaiTro, DienThoai, DiaChi, NgayVaoLam, TrangThai)
    VALUES
    (N'Nguyễn Văn An', 'admin', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', @VaiTroQuanLy, '0901234567', N'12 Nguyễn Huệ, Quận 1', '2024-01-15', N'Đang làm'),
    (N'Trần Thị Bình', 'nhanvien', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', @VaiTroNhanVien, '0912345678', N'45 Lê Lợi, Quận 1', '2024-03-01', N'Đang làm'),
    (N'Lê Minh Châu', 'nhanvien2', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', @VaiTroNhanVien, '0923456789', N'78 Hai Bà Trưng, Quận 3', '2025-02-10', N'Đang làm');
END

IF NOT EXISTS (SELECT 1 FROM dbo.KhuVuc)
BEGIN
    INSERT INTO dbo.KhuVuc (TenKhuVuc, MoTa)
    VALUES
    (N'Tầng trệt', N'Khu vực gần quầy pha chế, thuận tiện cho khách đi nhanh'),
    (N'Sân vườn', N'Khu vực ngoài trời, nhiều cây xanh và thoáng mát'),
    (N'Tầng 1', N'Khu vực yên tĩnh, phù hợp làm việc và học nhóm');
END

SET @KhuTangTret = (SELECT TOP 1 MaKhuVuc FROM dbo.KhuVuc WHERE TenKhuVuc = N'Tầng trệt');
SET @KhuSanVuon = (SELECT TOP 1 MaKhuVuc FROM dbo.KhuVuc WHERE TenKhuVuc = N'Sân vườn');
SET @KhuTang1 = (SELECT TOP 1 MaKhuVuc FROM dbo.KhuVuc WHERE TenKhuVuc = N'Tầng 1');

IF NOT EXISTS (SELECT 1 FROM dbo.Ban)
BEGIN
    INSERT INTO dbo.Ban (TenBan, MaKhuVuc, SoCho) VALUES
    (N'B01', @KhuTangTret, 2), (N'B02', @KhuTangTret, 2), (N'B03', @KhuTangTret, 4),
    (N'B04', @KhuTangTret, 4), (N'B05', @KhuTangTret, 4), (N'B06', @KhuTangTret, 6),
    (N'S01', @KhuSanVuon, 4), (N'S02', @KhuSanVuon, 4), (N'S03', @KhuSanVuon, 6), (N'S04', @KhuSanVuon, 8),
    (N'L01', @KhuTang1, 4), (N'L02', @KhuTang1, 6), (N'L03', @KhuTang1, 6), (N'L04', @KhuTang1, 8);
END

IF NOT EXISTS (SELECT 1 FROM dbo.LoaiMon)
BEGIN
    INSERT INTO dbo.LoaiMon (TenLoai, MoTa) VALUES
    (N'Cà phê Việt Nam', N'Các món cà phê pha phin truyền thống'),
    (N'Cà phê Ý', N'Espresso, Latte, Cappuccino và các món pha máy'),
    (N'Trà & Trà sữa', N'Trà trái cây, trà sữa và matcha'),
    (N'Nước ép & Sinh tố', N'Đồ uống từ trái cây tươi'),
    (N'Bánh ngọt', N'Bánh dùng kèm cà phê và trà');
END

SET @LoaiCaPheViet = (SELECT TOP 1 MaLoai FROM dbo.LoaiMon WHERE TenLoai = N'Cà phê Việt Nam');
SET @LoaiCaPheY = (SELECT TOP 1 MaLoai FROM dbo.LoaiMon WHERE TenLoai = N'Cà phê Ý');
SET @LoaiTra = (SELECT TOP 1 MaLoai FROM dbo.LoaiMon WHERE TenLoai = N'Trà & Trà sữa');
SET @LoaiNuocEp = (SELECT TOP 1 MaLoai FROM dbo.LoaiMon WHERE TenLoai = N'Nước ép & Sinh tố');
SET @LoaiBanh = (SELECT TOP 1 MaLoai FROM dbo.LoaiMon WHERE TenLoai = N'Bánh ngọt');

IF NOT EXISTS (SELECT 1 FROM dbo.Mon)
BEGIN
    INSERT INTO dbo.Mon (TenMon, MaLoai, Gia, DonViTinh, TrangThai, MoTa) VALUES
    (N'Cà phê đen đá', @LoaiCaPheViet, 25000, N'Ly', 1, N'Cà phê phin truyền thống, đậm vị'),
    (N'Cà phê sữa đá', @LoaiCaPheViet, 30000, N'Ly', 1, N'Cà phê phin kết hợp sữa đặc'),
    (N'Bạc xỉu', @LoaiCaPheViet, 35000, N'Ly', 1, N'Nhiều sữa, ít cà phê, vị dịu'),
    (N'Cà phê muối', @LoaiCaPheViet, 35000, N'Ly', 1, N'Cà phê kem muối đang được yêu thích'),
    (N'Espresso', @LoaiCaPheY, 35000, N'Ly', 1, N'Cà phê Ý đậm đặc'),
    (N'Americano', @LoaiCaPheY, 40000, N'Ly', 1, N'Espresso pha loãng với nước'),
    (N'Cappuccino', @LoaiCaPheY, 45000, N'Ly', 1, N'Espresso, sữa nóng và bọt sữa'),
    (N'Latte', @LoaiCaPheY, 45000, N'Ly', 1, N'Sữa nóng kết hợp espresso'),
    (N'Mocha', @LoaiCaPheY, 50000, N'Ly', 1, N'Cà phê, sô-cô-la và sữa'),
    (N'Trà đào cam sả', @LoaiTra, 45000, N'Ly', 1, N'Trà đào thơm mát với cam và sả'),
    (N'Trà sữa trân châu', @LoaiTra, 45000, N'Ly', 1, N'Trà sữa béo nhẹ, trân châu dai'),
    (N'Trà xanh matcha', @LoaiTra, 45000, N'Ly', 1, N'Matcha thanh mát'),
    (N'Trà chanh', @LoaiTra, 30000, N'Ly', 1, N'Trà chanh giải nhiệt'),
    (N'Nước cam', @LoaiNuocEp, 40000, N'Ly', 1, N'Cam tươi vắt mỗi ngày'),
    (N'Nước ép dưa hấu', @LoaiNuocEp, 40000, N'Ly', 1, N'Dưa hấu tươi mát'),
    (N'Sinh tố bơ', @LoaiNuocEp, 45000, N'Ly', 1, N'Bơ sáp béo mịn'),
    (N'Sinh tố xoài', @LoaiNuocEp, 45000, N'Ly', 1, N'Xoài chín ngọt tự nhiên'),
    (N'Bánh croissant', @LoaiBanh, 30000, N'Cái', 1, N'Bánh sừng bò bơ Pháp'),
    (N'Tiramisu', @LoaiBanh, 45000, N'Miếng', 1, N'Bánh tiramisu mềm mịn'),
    (N'Bánh mì que', @LoaiBanh, 25000, N'Cái', 1, N'Bánh mì que giòn, dùng kèm cà phê'),
    (N'Cookie bơ', @LoaiBanh, 25000, N'Cái', 1, N'Cookie bơ thơm giòn');
END

-- Sinh hóa đơn mẫu trong 21 ngày gần nhất để phần thống kê có dữ liệu trực quan.
IF NOT EXISTS (SELECT 1 FROM dbo.HoaDon)
BEGIN
    SET @Ngay = 21;
    WHILE @Ngay >= 0
    BEGIN
        SET @SoHoaDon = 4 + ABS(CHECKSUM(NEWID())) % 4;
        SET @IHoaDon = 1;

        WHILE @IHoaDon <= @SoHoaDon
        BEGIN
            SELECT TOP 1 @MaBan = MaBan FROM dbo.Ban ORDER BY NEWID();
            SELECT TOP 1 @MaNV = MaNV FROM dbo.NhanVien WHERE MaVaiTro = @VaiTroNhanVien ORDER BY NEWID();

            IF @Ngay = 0
            BEGIN
                SET @ThoiDiem = DATEADD(MINUTE, -(ABS(CHECKSUM(NEWID())) % 420), GETDATE());
            END
            ELSE
            BEGIN
                SET @ThoiDiem = DATEADD(MINUTE, ABS(CHECKSUM(NEWID())) % 60,
                    DATEADD(HOUR, 7 + ABS(CHECKSUM(NEWID())) % 14,
                    CAST(CAST(DATEADD(DAY, -@Ngay, GETDATE()) AS DATE) AS DATETIME)));
            END

            INSERT INTO dbo.HoaDon (MaBan, MaNV, NgayLap, NgayThanhToan, PhuongThucThanhToan, TrangThai)
            VALUES (@MaBan, @MaNV, @ThoiDiem, @ThoiDiem,
                    CASE WHEN ABS(CHECKSUM(NEWID())) % 2 = 0 THEN N'Tiền mặt' ELSE N'Chuyển khoản' END,
                    N'Đã thanh toán');

            SET @MaHD = CAST(SCOPE_IDENTITY() AS INT);
            SET @SoMon = 1 + ABS(CHECKSUM(NEWID())) % 3;
            SET @J = 1;

            WHILE @J <= @SoMon
            BEGIN
                SELECT TOP 1 @MaMon = MaMon, @Gia = Gia FROM dbo.Mon ORDER BY NEWID();
                SET @SoLuong = 1 + ABS(CHECKSUM(NEWID())) % 3;

                IF NOT EXISTS (SELECT 1 FROM dbo.ChiTietHoaDon WHERE MaHD = @MaHD AND MaMon = @MaMon)
                BEGIN
                    INSERT INTO dbo.ChiTietHoaDon (MaHD, MaMon, SoLuong, DonGia)
                    VALUES (@MaHD, @MaMon, @SoLuong, @Gia);
                END

                SET @J = @J + 1;
            END

            SET @TongTien = (SELECT ISNULL(SUM(ThanhTien), 0) FROM dbo.ChiTietHoaDon WHERE MaHD = @MaHD);
            SET @GiamGia = CASE WHEN @TongTien >= 50000 AND ABS(CHECKSUM(NEWID())) % 4 = 0 THEN 5000 ELSE 0 END;

            UPDATE dbo.HoaDon
            SET TongTien = @TongTien, GiamGia = @GiamGia
            WHERE MaHD = @MaHD;

            SET @IHoaDon = @IHoaDon + 1;
        END

        SET @Ngay = @Ngay - 1;
    END
END

IF NOT EXISTS (SELECT 1 FROM dbo.CaiDat)
BEGIN
    INSERT INTO dbo.CaiDat (Khoa, GiaTri) VALUES
    (N'TenQuan', N'Quán cà phê T1'),
    (N'DiaChi', N'123 Đường Cà Phê, Phường Bến Nghé, Quận 1'),
    (N'DienThoai', N'028 1234 5678'),
    (N'LoiChao', N'Cảm ơn quý khách và hẹn gặp lại!');
END
