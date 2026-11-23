# Báo cáo bài tập: Xây dựng phần mềm quản lý quán cà phê T1

## 1. Giới thiệu

Đề tài xây dựng một phần mềm desktop quản lý quán cà phê bằng **C# WinForms**, sử dụng **ADO.NET** kết nối **SQL Server**. Phần mềm phục vụ các nghiệp vụ hằng ngày của quán: đăng nhập và phân quyền, quản lý khu vực – bàn, quản lý thực đơn, gọi món – thanh toán, quản lý nhân viên, tra cứu – in hóa đơn và thống kê doanh thu.

Toàn bộ giao diện và thông báo của phần mềm được viết bằng **tiếng Việt**, phù hợp với người dùng cuối là nhân viên và quản lý quán.

Cơ sở dữ liệu được **tự động khởi tạo** ngay lần đầu chạy: phần mềm kiểm tra kết nối tới SQL Server, tạo database `QuanLyQuanCaPheT1`, chạy script `Database/ScriptDB.sql` để tạo bảng và chèn dữ liệu mẫu. Nhờ đó, khi tải source từ GitHub về chỉ cần có SQL Server (hoặc LocalDB) là chương trình có thể chạy được ngay.

## 2. Mục tiêu của bài tập

- Vận dụng kiến trúc **3 lớp (3-Tier Architecture)** vào một ứng dụng thực tế.
- Thành thạo thao tác ADO.NET: `SqlConnection`, `SqlCommand`, `SqlParameter`, `SqlTransaction`, `SqlDataAdapter`.
- Rèn luyện kỹ năng thiết kế cơ sở dữ liệu quan hệ: khóa chính, khóa ngoại, ràng buộc duy nhất, cột tính toán, chỉ mục.
- Xây dựng giao diện WinForms gọn gàng, dễ dùng, có phân quyền theo vai trò.
- Viết báo cáo và tổ chức mã nguồn rõ ràng để có thể bàn giao, kiểm thử.

## 3. Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ | C# 7.3 |
| Nền tảng | .NET Framework 4.8 |
| Giao diện | Windows Forms (Windows Forms Designer được thay bằng mã dựng giao diện trong code) |
| Truy cập dữ liệu | ADO.NET (`System.Data.SqlClient`) |
| Cơ sở dữ liệu | Microsoft SQL Server (mặc định LocalDB `(localdb)\MSSQLLocalDB`) |
| In ấn | `System.Drawing.Printing` + GDI+ vẽ hóa đơn |
| Công cụ | Visual Studio, Git/GitHub |

## 4. Kiến trúc hệ thống

Phần mềm được tổ chức theo mô hình **3 lớp**, tách biệt rõ ràng giữa giao diện, nghiệp vụ và truy cập dữ liệu:

```
Người dùng
   │
   ▼
GUI (Forms)  ──►  BUS (Business Logic)  ──►  DAL (Data Access)  ──►  SQL Server
   │                    │                        │
   └──── DTO ◄──────────┘        DTO ◄───────────┘
```

- **GUI (Presentation Layer):** các Form/UserControl, tiếp nhận thao tác người dùng, hiển thị dữ liệu. GUI chỉ gọi BUS, không viết câu SQL.
- **BUS (Business Logic Layer):** kiểm tra dữ liệu đầu vào, xử lý nghiệp vụ (băm mật khẩu, kiểm tra quản lý cuối cùng, chuyển đổi trạng thái...), gọi DAL.
- **DAL (Data Access Layer):** thực hiện truy vấn ADO.NET, trả về `DataTable` hoặc danh sách đối tượng.
- **DTO (Data Transfer Object):** các lớp dữ liệu đơn giản dùng để chuyển dữ liệu giữa các tầng (`TaiKhoanDTO`, `NhanVienDTO`, `HoaDonDTO`, `ChiTietHoaDonDTO`, `CaiDatDTO`).

Ưu điểm của cách tổ chức này: khi thay đổi cơ sở dữ liệu hay quy tắc nghiệp vụ, chỉ cần sửa một tầng; giao diện không phụ thuộc trực tiếp vào SQL; mã nguồn dễ đọc, dễ kiểm thử và dễ mở rộng.

## 5. Cấu trúc thư mục solution

```
├── Database/
│   └── ScriptDB.sql               # Script khởi tạo CSDL SQL Server và chèn dữ liệu mẫu
├── QuanLyQuanCaPhe/
│   ├── DTO/                       # Data Transfer Object
│   │   ├── TaiKhoanDTO.cs
│   │   ├── NhanVienDTO.cs
│   │   ├── HoaDonDTO.cs
│   │   ├── ChiTietHoaDonDTO.cs
│   │   └── CaiDatDTO.cs
│   ├── DAL/                       # Data Access Layer
│   │   ├── DatabaseHelper.cs      # Kết nối, Query/Execute/Scalar, tham số hóa
│   │   ├── DatabaseInitializer.cs # Tự tạo CSDL từ ScriptDB.sql
│   │   ├── TaiKhoanDAL.cs
│   │   ├── NhanVienDAL.cs
│   │   ├── KhuVucDAL.cs
│   │   ├── BanDAL.cs
│   │   ├── BanHangDAL.cs          # Mở hóa đơn, gọi món, thanh toán
│   │   ├── ThucDonDAL.cs
│   │   ├── HoaDonDAL.cs           # Tra cứu, chi tiết và hủy hóa đơn
│   │   ├── ThongKeDAL.cs
│   │   └── CaiDatDAL.cs
│   ├── BUS/                       # Business Logic Layer
│   │   ├── MatKhau.cs             # Băm mật khẩu SHA-256
│   │   ├── TaiKhoanBUS.cs         # Đăng nhập
│   │   ├── NhanVienBUS.cs
│   │   ├── KhuVucBUS.cs
│   │   ├── BanBUS.cs
│   │   ├── ThucDonBUS.cs
│   │   ├── BanHangBUS.cs          # Gọi món, thanh toán
│   │   ├── HoaDonBUS.cs
│   │   ├── ThongKeBUS.cs
│   │   └── CaiDatBUS.cs
│   ├── GUI/                       # Presentation Layer
│   │   ├── Theme.cs               # Màu sắc, font, control dùng chung
│   │   ├── Utils.cs               # Định dạng tiền, ngày, mã hóa đơn
│   │   ├── Session.cs             # Phiên đăng nhập hiện tại
│   │   ├── Controls/              # UiGrid, BarChart, BufferedPanel
│   │   ├── Printing/              # InvoicePrinter (xem trước và in hóa đơn)
│   │   └── Forms/                 # frmDangNhap, frmMain, ucTrangChu, ucBanHang,
│   │                              # ucQuanLyBan, ucThucDon, ucNhanVien,
│   │                              # ucHoaDon, ucThongKe, frmKetNoiCSDL
│   ├── App.config                 # Chuỗi kết nối SQL Server
│   ├── Program.cs                 # Điểm khởi chạy, chế độ --selftest
│   ├── Properties/AssemblyInfo.cs
│   └── QuanLyQuanCaPhe.csproj
├── README.md
└── QuanLyQuanCaPhe.sln
```

## 6. Chức năng của phần mềm

### 6.1. Đăng nhập và phân quyền

- Đăng nhập bằng tên đăng nhập và mật khẩu; mật khẩu được lưu dưới dạng **băm SHA-256**, không lưu văn bản gốc.
- Kiểm tra tài khoản phải đang ở trạng thái "Đang làm" mới được đăng nhập.
- Có hai vai trò:
  - **Quản lý:** sử dụng đầy đủ các chức năng, gồm quản lý nhân viên, thực đơn, hóa đơn, thống kê.
  - **Nhân viên:** bán hàng, quản lý bàn, xem hóa đơn; các mục dành riêng cho quản lý được ẩn.
- Hỗ trợ đăng xuất để quay lại màn hình đăng nhập mà không phải tắt phần mềm.
- Form `frmKetNoiCSDL` cho phép cấu hình lại máy chủ SQL Server nếu máy khác không dùng LocalDB mặc định.

### 6.2. Bán hàng

- Chọn khu vực (hoặc tất cả khu vực), sau đó chọn bàn cần phục vụ.
- Danh sách món ăn – đồ uống lấy từ thực đơn, lọc theo loại món và tìm kiếm theo tên.
- Nhấn vào món để thêm vào hóa đơn đang mở của bàn. Nếu bàn chưa có hóa đơn, hệ thống tự tạo hóa đơn mới.
- Tăng/giảm số lượng hoặc xóa món khỏi hóa đơn.
- Hiển thị tạm tính: tổng tiền, giảm giá (theo %), thành tiền.
- Thanh toán bằng tiền mặt hoặc chuyển khoản, có thể ghi chú; sau khi thanh toán, bàn trở về trạng thái "Trống".
- Có thể xem trước phiếu tạm tính hoặc in hóa đơn.

### 6.3. Quản lý bàn và khu vực

- Hai tab: **Quản lý bàn** và **Khu vực**.
- Thêm, sửa, xóa bàn (tên bàn, khu vực, số chỗ, trạng thái).
- Thêm, sửa, xóa khu vực (tầng trệt, sân vườn, tầng 1...).
- Chặn xóa khu vực khi vẫn còn bàn thuộc khu vực đó; chặn xóa bàn khi đã có hóa đơn.
- Nút **Đồng bộ trạng thái** cập nhật lại trạng thái bàn theo hóa đơn thực tế (Trống / Đang sử dụng).

### 6.4. Quản lý thực đơn

- Hai tab: **Thực đơn** và **Loại món**.
- Thêm, sửa món: tên món, loại, giá, đơn vị tính, mô tả, trạng thái đang bán/ngừng bán.
- Thêm, sửa, xóa loại món; chặn xóa loại món khi còn món thuộc loại đó.
- Chặn xóa món đã phát sinh trong hóa đơn, chỉ cho phép chuyển sang trạng thái ngừng bán.

### 6.5. Quản lý nhân viên

- Danh sách nhân viên: họ tên, tên đăng nhập, vai trò, điện thoại, ngày vào làm, trạng thái.
- Thêm nhân viên mới kèm mật khẩu ban đầu (bắt buộc); khi sửa có thể để trống mật khẩu nếu không đổi.
- Kiểm tra trùng tên đăng nhập, thông báo thân thiện.
- Chuyển nhân viên sang trạng thái "Nghỉ việc" thay vì xóa nếu đã lập hóa đơn.
- Không cho xóa tài khoản đang đăng nhập.
- Bảo vệ hệ thống: không cho xóa, hạ quyền hoặc cho nghỉ việc **quản lý duy nhất đang hoạt động**.

### 6.6. Hóa đơn

- Lọc hóa đơn theo khoảng ngày, trạng thái và từ khóa (mã hóa đơn, tên bàn, tên nhân viên).
- Hiển thị danh sách hóa đơn kèm số món, tổng tiền, trạng thái.
- Xem chi tiết hóa đơn: mã, bàn, khu vực, nhân viên lập, thời gian, phương thức thanh toán, danh sách món và tổng tiền.
- In hóa đơn thanh toán hoặc phiếu tạm tính qua hộp thoại xem trước in.
- Hủy hóa đơn chưa thanh toán (có xác nhận).

### 6.7. Thống kê

- Lọc theo khoảng ngày hoặc chọn nhanh "7 ngày gần nhất", "30 ngày gần nhất", "Tháng này".
- Các chỉ số tổng quan: doanh thu, số hóa đơn đã thanh toán, số món bán ra, số bàn đang sử dụng.
- Biểu đồ cột doanh thu theo ngày (vẽ bằng GDI+).
- Bảng món bán chạy và tỉ lệ hình thức thanh toán.

### 6.8. Trang chủ

- Lời chào theo tên nhân viên đang đăng nhập.
- Thẻ thống kê nhanh trong ngày: doanh thu hôm nay, số hóa đơn hôm nay, số bàn đang sử dụng.
- Danh sách hóa đơn gần đây và món bán chạy hôm nay.
- Nút thao tác nhanh mở màn hình bán hàng hoặc thống kê.

## 7. Thiết kế cơ sở dữ liệu

Cơ sở dữ liệu gồm 9 bảng:

| Bảng | Ý nghĩa | Khóa chính | Ghi chú |
|---|---|---|---|
| `VaiTro` | Vai trò người dùng | `MaVaiTro` | Quản lý / Nhân viên |
| `NhanVien` | Thông tin nhân viên | `MaNV` | FK `MaVaiTro`; `TenDangNhap` duy nhất |
| `KhuVuc` | Khu vực trong quán | `MaKhuVuc` | `TenKhuVuc` duy nhất |
| `Ban` | Bàn của quán | `MaBan` | FK `MaKhuVuc`; tên bàn duy nhất trong khu vực |
| `LoaiMon` | Loại món | `MaLoai` | `TenLoai` duy nhất |
| `Mon` | Món ăn/đồ uống | `MaMon` | FK `MaLoai`; `TrangThai` (1 = đang bán) |
| `HoaDon` | Hóa đơn | `MaHD` | FK `MaBan`, `MaNV`; `ThanhTien` là cột tính toán |
| `ChiTietHoaDon` | Chi tiết hóa đơn | `MaCT` | FK `MaHD` (xóa cascade), `MaMon`; `ThanhTien` tính toán |
| `CaiDat` | Cấu hình quán | `Khoa` | Tên quán, địa chỉ, điện thoại, lời chào |

Một số điểm thiết kế đáng chú ý:

- `HoaDon.ThanhTien` là **cột tính toán** `TongTien - GiamGia`, `ChiTietHoaDon.ThanhTien` là `SoLuong * DonGia` — giúp dữ liệu luôn nhất quán, tránh sai lệch khi cập nhật.
- Ràng buộc duy nhất trên tên đăng nhập, tên khu vực, tên loại món và cặp (tên bàn, khu vực).
- Chỉ mục trên ngày thanh toán và mã bàn của hóa đơn để tăng tốc lọc/thống kê.
- Dữ liệu mẫu: 2 vai trò, 3 nhân viên, 3 khu vực, 14 bàn, 5 loại món, 21 món và hơn 100 hóa đơn đã thanh toán trong 21 ngày gần nhất để biểu đồ thống kê có dữ liệu trực quan.

## 8. Một số xử lý nghiệp vụ nổi bật

### 8.1. Tự khởi tạo cơ sở dữ liệu

`DatabaseInitializer.EnsureCreated()` được gọi khi khởi động:

1. Xác định máy chủ SQL Server khả dụng theo thứ tự: chuỗi kết nối trong `App.config` → LocalDB → `.\SQLEXPRESS` → `(local)\SQLExpress` → `.` → `(local)`, và ghi nhớ lựa chọn vào `%APPDATA%\QuanLyQuanCaPheT1\server.txt`.
2. Tạo database `QuanLyQuanCaPheT1` nếu chưa có.
3. Đọc script `Database/ScriptDB.sql` nhúng trong assembly, chạy phần tạo bảng ở mọi lần khởi động (an toàn nhờ `IF OBJECT_ID(...) IS NULL`).
4. Chỉ chèn dữ liệu mẫu **một lần**: nếu bảng `CaiDat` chưa có khóa `DaKhoiTao` thì chạy phần dữ liệu mẫu trong một transaction và ghi khóa này.

Danh sách bàn, món, nhân viên... được giữ nguyên qua các lần chạy; người dùng có thể yên tâm sử dụng dữ liệu thật sau khi đã xóa dữ liệu mẫu.

### 8.2. Gọi món và thanh toán bằng transaction

- `BanHangDAL.GetOrCreateInvoice` dùng khóa `UPDLOCK, HOLDLOCK` để tránh hai máy tạo trùng hóa đơn cho cùng một bàn.
- Thêm món: nếu món đã có trong hóa đơn thì tăng số lượng, ngược lại thêm dòng mới.
- Thanh toán: mở `SqlTransaction`, cập nhật hóa đơn (trạng thái, giảm giá, phương thức, ngày thanh toán), sau đó cập nhật bàn về "Trống"; nếu lỗi thì rollback toàn bộ.

### 8.3. Bảo mật mật khẩu

- `MatKhau.Bam` băm mật khẩu bằng SHA-256, trả về chuỗi hex 64 ký tự.
- Đăng nhập so sánh trực tiếp giá trị băm; nhân viên đổi mật khẩu cũng lưu giá trị băm.
- Tài khoản demo dùng chung mật khẩu `123456`.

### 8.4. In hóa đơn

`InvoicePrinter` dùng `PrintDocument` và GDI+ để vẽ hóa đơn với: tên quán, địa chỉ, điện thoại (đọc từ bảng `CaiDat`), mã hóa đơn, bàn, nhân viên, thời gian, bảng món, tổng tiền, giảm giá, thành tiền và lời cảm ơn. Hỗ trợ phân trang khi hóa đơn dài và xem trước trước khi in.

## 9. Hướng dẫn cài đặt và chạy

### Yêu cầu

- Windows 10/11.
- Visual Studio 2019 hoặc 2022 (đã cài workload **.NET desktop development**).
- .NET Framework 4.8.
- SQL Server 2017 trở lên **hoặc** SQL Server LocalDB (đi kèm Visual Studio).

### Các bước chạy

1. Clone repository:
   ```
   git clone https://github.com/nghiabao2006-commits/coffee.git
   ```
2. Mở `QuanLyQuanCaPhe.sln` bằng Visual Studio.
3. Nhấn **F5** (hoặc Ctrl+F5) để chạy. Lần chạy đầu tiên, phần mềm tự tạo database và dữ liệu mẫu; quá trình này có thể mất vài giây.
4. Đăng nhập bằng tài khoản demo:

   | Vai trò | Tên đăng nhập | Mật khẩu |
   |---|---|---|
   | Quản lý | `admin` | `123456` |
   | Nhân viên | `nhanvien` | `123456` |
   | Nhân viên | `nhanvien2` | `123456` |

5. Nếu máy không có LocalDB/SQL Server mặc định, bấm **Cấu hình kết nối SQL Server** ở màn hình đăng nhập và nhập máy chủ phù hợp (ví dụ `.\SQLEXPRESS`).

### Kiểm tra tự động

Chạy từ dòng lệnh để kiểm tra nhanh kết nối, dữ liệu mẫu và nghiệp vụ thanh toán:

```
QuanLyQuanCaPhe.exe --selftest
```

Kết quả được ghi vào `selftest.log` cạnh file chạy, dòng cuối là `KẾT QUẢ: OK` nếu mọi bước thành công.

## 10. Kết quả kiểm thử

| STT | Trường hợp kiểm thử | Kết quả mong đợi | Kết quả |
|---|---|---|---|
| 1 | Đăng nhập đúng tài khoản `admin/123456` | Vào trang chủ với quyền quản lý | Đạt |
| 2 | Đăng nhập sai mật khẩu | Báo lỗi, không cho vào | Đạt |
| 3 | Đăng nhập tài khoản "Nghỉ việc" | Báo lỗi | Đạt |
| 4 | Nhân viên đăng nhập | Ẩn các mục Nhân viên, Thực đơn, Thống kê | Đạt |
| 5 | Chọn bàn trống, thêm món | Hóa đơn mới được tạo, bàn chuyển "Đang sử dụng" | Đạt |
| 6 | Giảm số lượng / xóa món | Tổng tiền cập nhật đúng | Đạt |
| 7 | Thanh toán | Hóa đơn "Đã thanh toán", bàn về "Trống" | Đạt |
| 8 | In hóa đơn | Hiện hộp thoại xem trước với đúng thông tin | Đạt |
| 9 | Thêm nhân viên trùng tên đăng nhập | Báo "Tên đăng nhập này đã được sử dụng" | Đạt |
| 10 | Xóa quản lý duy nhất | Bị chặn kèm thông báo | Đạt |
| 11 | Xóa khu vực còn bàn | Bị chặn kèm thông báo | Đạt |
| 12 | Thống kê theo khoảng ngày | Doanh thu, số hóa đơn, biểu đồ khớp dữ liệu | Đạt |
| 13 | Tải source về máy khác | Tự tạo CSDL và chạy được | Đạt (kiểm tra bằng `--selftest`) |

## 11. Ưu điểm, hạn chế và hướng phát triển

### Ưu điểm

- Kiến trúc 3 lớp rõ ràng, dễ bảo trì và mở rộng.
- Cơ sở dữ liệu tự khởi tạo, thuận tiện khi chấm bài hoặc triển khai thử trên máy khác.
- Giao diện tiếng Việt thống nhất, thao tác đơn giản.
- Có phân quyền, bảo vệ quản lý cuối cùng và các ràng buộc dữ liệu chặt chẽ.
- Sử dụng tham số hóa SQL, hạn chế lỗi SQL Injection; mật khẩu được băm.

### Hạn chế

- Chưa hỗ trợ in trực tiếp ra máy in nhiệt khổ 58 mm (hiện tại in qua hộp thoại chuẩn của Windows).
- Chưa có chức năng quản lý nhập kho nguyên vật liệu, khuyến mãi, hoặc đặt bàn trước.
- Chưa đồng bộ nhiều máy tính cùng lúc qua mạng LAN (dù cơ sở dữ liệu đã sẵn sàng cho việc này).

### Hướng phát triển

- Thêm báo cáo xuất Excel/PDF.
- Quản lý kho nguyên vật liệu và công thức pha chế.
- Hỗ trợ máy in hóa đơn khổ nhỏ và quét mã QR thanh toán.
- Xây dựng thêm ứng dụng di động hoặc web cho khách gọi món tại bàn.

## 12. Kết luận

Bài tập đã xây dựng hoàn chỉnh một phần mềm quản lý quán cà phê đáp ứng các nghiệp vụ cơ bản của quán, được tổ chức theo chuẩn 3 lớp và sử dụng ADO.NET kết nối SQL Server. Qua quá trình thực hiện, em đã rèn luyện được kỹ năng phân tích yêu cầu, thiết kế cơ sở dữ liệu, lập trình giao diện WinForms, xử lý transaction và tổ chức mã nguồn khoa học. Phần mềm có thể chạy ngay sau khi tải source và đã được kiểm thử các luồng nghiệp vụ chính.

## 13. Phụ lục: Phân công công việc nhóm 3 người

Mỗi thành viên đảm nhận tối thiểu 3 màn hình chức năng (UserControl được tính là một màn hình WinForms). Cách chia dưới đây cắt theo cụm nghiệp vụ để mỗi người sở hữu trọn gói GUI → BUS → DAL của phần mình.

### 13.1. Bảng phân công

| Thành viên | Màn hình phụ trách | BUS/DAL phụ trách |
|---|---|---|
| **Người 1** – Nền tảng, đăng nhập, khung chương trình | `frmDangNhap`, `frmKetNoiCSDL`, `frmMain` | `TaiKhoanBUS`, `MatKhau`, `TaiKhoanDAL`; chủ trì `DatabaseHelper`, `DatabaseInitializer`, `ScriptDB.sql`, `Program.cs`, `App.config`, `.sln`, `.csproj` |
| **Người 2** – Bán hàng, bàn, thực đơn | `ucBanHang`, `ucQuanLyBan`, `ucThucDon` | `BanHangBUS`, `BanBUS`, `KhuVucBUS`, `ThucDonBUS`, `BanHangDAL`, `BanDAL`, `KhuVucDAL`, `ThucDonDAL` |
| **Người 3** – Trang chủ, nhân viên, hóa đơn, thống kê, in | `ucTrangChu`, `ucNhanVien`, `ucHoaDon`, `ucThongKe`, `InvoicePrinter` | `NhanVienBUS`, `HoaDonBUS`, `ThongKeBUS`, `CaiDatBUS`, `NhanVienDAL`, `HoaDonDAL`, `ThongKeDAL`, `CaiDatDAL` |

### 13.2. File dùng chung để gắn kết 3 phần

- `Database/ScriptDB.sql`: nguồn sự thật duy nhất về bảng và dữ liệu mẫu; chỉ sửa khi cả nhóm thống nhất.
- 5 lớp DTO (`TaiKhoanDTO`, `NhanVienDTO`, `HoaDonDTO`, `ChiTietHoaDonDTO`, `CaiDatDTO`): hợp đồng dữ liệu, không đổi tên thuộc tính.
- `DAL/DatabaseHelper.cs`, `DAL/DatabaseInitializer.cs`: kết nối và tự khởi tạo cơ sở dữ liệu.
- `GUI/Theme.cs`, `GUI/Utils.cs`, `GUI/Session.cs` và các control `UiGrid`, `BufferedPanel`, `BarChart`.
- `frmMain.cs` (Người 1): gọi 7 màn hình theo tên class cố định.
- `InvoicePrinter.cs` (Người 3): Người 2 dùng lại để in tạm tính.
- `Program.cs --selftest`, `.sln`, `.csproj`, `App.config`: kiểm thử và cấu hình chung.

### 13.3. Quy ước dữ liệu chung

| Bảng | Người ghi | Người đọc | Cột trọng yếu |
|---|---|---|---|
| `VaiTro`, `NhanVien` | Người 3 | Người 1 (đăng nhập) | `TenDangNhap`, `MatKhau`, `MaVaiTro`, `TrangThai` |
| `KhuVuc`, `Ban` | Người 2 | Người 3 (thống kê, hủy hóa đơn) | `MaBan`, `MaKhuVuc`, `TrangThai` |
| `LoaiMon`, `Mon` | Người 2 | Người 2 | `MaMon`, `MaLoai`, `TrangThai` |
| `HoaDon`, `ChiTietHoaDon` | Người 2 (tạo/thanh toán) | Người 3 (xem/hủy/thống kê) | `MaHD`, `MaBan`, `MaNV`, `TongTien`, `GiamGia`, `ThanhTien`, `PhuongThucThanhToan`, `TrangThai` |
| `CaiDat` | Dữ liệu mẫu | Người 3 (in hóa đơn) | `Khoa`, `GiaTri` |

Chuỗi trạng thái dùng thống nhất: `Trống` / `Đang sử dụng`; `Chưa thanh toán` / `Đã thanh toán` / `Đã hủy`; `Tiền mặt` / `Chuyển khoản`; `Đang làm` / `Nghỉ việc`.

### 13.4. Quy trình ghép bài

1. Chốt trước tên class màn hình, chữ ký hàm BUS, chuỗi trạng thái và script CSDL.
2. Người 1 hoàn thiện hạ tầng, đăng nhập và khung chính (dùng màn hình rỗng đúng tên để biên dịch được).
3. Người 2 và Người 3 làm song song trên nhánh riêng, chỉ sửa file thuộc phần mình.
4. Ghép theo thứ tự: hạ tầng → Người 2 → Người 3; Người 1 chạy `--selftest` và kiểm thử end-to-end (đăng nhập → gọi món → thanh toán → in hóa đơn → thống kê → quản lý nhân viên).
5. Mỗi người commit phần của mình và viết một mục trong báo cáo; Người 1 tổng hợp README.
