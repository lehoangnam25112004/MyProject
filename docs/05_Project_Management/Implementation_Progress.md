# Tiến độ triển khai

| Nghiệp vụ | Phân tích | Code | Tự kiểm thử | Người dùng xác nhận |
|---|---|---|---|---|
| 01. Tài khoản và phân quyền | DONE | DONE | PARTIAL (build, route, DB/roles) | WAITING_FOR_USER_TEST |
| 02. Quản lý dự án | DONE | DONE | PARTIAL (build, migration, route) | WAITING_FOR_USER_TEST |
| 03. Hợp đồng và tài liệu | DONE | DONE | PARTIAL (build, migration, route) | WAITING_FOR_USER_TEST |
| 04. Giai đoạn và WBS | DONE | DONE | PARTIAL (build, migration, route) | WAITING_FOR_USER_TEST |
| 05. Quản lý công việc | DONE | DONE | PARTIAL (build, migration, route) | WAITING_FOR_USER_TEST |
| 06. Kế hoạch và tiến độ | DONE | DONE | PARTIAL (build, migration, route) | WAITING_FOR_USER_TEST |
| 07. Nhân sự và đội thi công | DONE | DONE | PARTIAL (build, migration, route) | WAITING_FOR_USER_TEST |
| 08. Vật tư | DONE | DONE | PARTIAL (build, migration, route) | WAITING_FOR_USER_TEST |
| 09. Thiết bị | DONE | DONE | PARTIAL (build, migration, route) | WAITING_FOR_USER_TEST |
| 10. Nhật ký thi công | DONE | DONE | PARTIAL (build, migration, route) | WAITING_FOR_USER_TEST |
| 11. Sự cố, RFI và nghiệm thu | DONE | DONE | PARTIAL (build, migration, route) | WAITING_FOR_USER_TEST |
| 12. Chi phí, bàn giao và báo cáo | DONE | DONE | PARTIAL (build, migration, route) | WAITING_FOR_USER_TEST |

## Giai đoạn hiện tại

- Giai đoạn 1 — Khảo sát và lập kế hoạch: APPROVED.
- Giai đoạn 2 — Tạo khung dự án và tài liệu thiết kế: WAITING_FOR_USER_TEST.
- Giai đoạn 3 — Xây dựng nền tảng: WAITING_FOR_USER_TEST.
- 12 nghiệp vụ bắt buộc: CODE_COMPLETE; đang chờ kiểm thử tích hợp và xác nhận người dùng.
- Kiểm thử tích hợp 6 quy trình: ROUTE_SMOKE_COMPLETE; đang chờ chạy end-to-end với dữ liệu mẫu và tài khoản người dùng.

## Kiểm thử gần nhất

- Đã chuyển provider chính sang SQLite 10.0.1; connection string trỏ đến
  `Data/Databases/ConstructionProjectManagement.db`.
- Migration SQLite `20261111092125_InitialSqlite` đã áp dụng thành công; migration SQL Server
  được bảo toàn riêng trong `Migrations.SqlServer` và không được compile vào provider SQLite.
- Ứng dụng tự động gọi `Database.MigrateAsync()` khi khởi động và không ghi đè database hiện có.
- Đã seed đủ 8 vai trò Identity; Admin chỉ được tạo khi có `CPMS_ADMIN_EMAIL` và
  `CPMS_ADMIN_PASSWORD`, nên không có mật khẩu mặc định hardcode.
- Kiểm tra trực tiếp database: 37 bảng, đủ các bảng `AspNet*`, 1 Admin và 1 liên kết role.
- Ứng dụng trả HTTP 200 cho trang chủ và trang đăng nhập; `/Dashboard` chuyển hướng người chưa
  đăng nhập về Identity Login trên SQLite.

Các trạng thái chỉ được chuyển tiếp sau khi có kết quả kiểm thử thực tế và xác nhận của người dùng.
