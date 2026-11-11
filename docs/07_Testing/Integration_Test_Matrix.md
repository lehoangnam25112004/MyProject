# Ma trận kiểm thử tích hợp 6 quy trình

## Phạm vi

Kiểm thử liên kết giữa các nghiệp vụ 01–12 theo sáu quy trình bắt buộc trong `Rule.md`.
Các kiểm thử HTTP route được thực hiện khi chưa đăng nhập; kết quả `302` về Identity Login
xác nhận middleware xác thực/phân quyền được áp dụng. Các bước nghiệp vụ có dữ liệu thật
cần được người dùng thực hiện sau khi tạo tài khoản và dữ liệu mẫu.

| Quy trình | Chuỗi nghiệp vụ chính | Kiểm tra tự động đã thực hiện | Trạng thái |
|---|---|---|---|
| 1. Lập kế hoạch và khởi tạo dự án | Dự án → hợp đồng → WBS → task → nhân sự | Build, migration, route và ràng buộc dữ liệu từng module | READY_FOR_USER_TEST |
| 2. Giao việc và tổ chức thi công | Task → phân công → tiến độ → nhật ký → chờ kiểm tra | Build, route, kiểm tra phạm vi task/người dùng | READY_FOR_USER_TEST |
| 3. Yêu cầu và cấp phát vật tư | Yêu cầu → duyệt → xuất → tồn kho → giao dịch | Migration, route, service không cho xuất vượt tồn | READY_FOR_USER_TEST |
| 4. Theo dõi tiến độ và xử lý chậm | Tiến độ → lịch sử → sự cố/RFI → xử lý | Build, route, lưu lịch sử tiến độ và quality | READY_FOR_USER_TEST |
| 5. Kiểm tra chất lượng và nghiệm thu | PendingInspection → kiểm tra → đạt/không đạt → lịch sử | Build, migration, ràng buộc `Completed`/`NeedsRework` | READY_FOR_USER_TEST |
| 6. Hoàn thành và bàn giao | Task/WBS → quality → chi phí → bàn giao → archive | Migration, route, kiểm tra công việc/sự cố/RFI/nghiệm thu mở | READY_FOR_USER_TEST |

## Kết quả smoke test HTTP

- `/` trả `200`.
- Ứng dụng khởi động bằng SQLite và tự động áp dụng migration pending qua
  `Database.MigrateAsync()`.
- File `Data/Databases/ConstructionProjectManagement.db` tồn tại; kiểm tra trực tiếp
  ghi nhận 37 bảng, 7 bảng Identity, 8 role và 1 tài khoản Admin được seed qua biến môi trường.
- `__EFMigrationsHistory` chỉ có migration `20261111092125_InitialSqlite`; không có migration
  SQL Server nào bị áp dụng vào SQLite.
- `/Dashboard`, `/Projects`, `/Contracts`, `/Wbs`, `/Tasks`, `/Progress`,
  `/Employees`, `/Teams`, `/Materials`, `/Equipment`, `/ConstructionLogs`,
  `/Quality` và `/Completion` đều trả `302` về Identity Login khi chưa đăng nhập.
- Kết quả xác nhận route và middleware xác thực hoạt động; chưa thay thế kiểm thử
  nghiệp vụ end-to-end bằng tài khoản người dùng thật.

## Các quy tắc đã xác minh trong code

- BR08: task cần nghiệm thu không được chuyển `Completed` trực tiếp.
- BR09: nghiệm thu không đạt bắt buộc có nội dung khắc phục.
- BR10: xuất vật tư không vượt `CurrentStock`.
- BR11: giao dịch vật tư lưu người thực hiện và thời điểm.
- BR12: cập nhật tiến độ lưu lịch sử.
- BR16: không bàn giao/lưu trữ khi còn điều kiện mở.

## Hướng dẫn kiểm thử người dùng

1. Cấu hình `CPMS_ADMIN_EMAIL` và `CPMS_ADMIN_PASSWORD` trong biến môi trường trước khi
   khởi động để tạo tài khoản Admin an toàn; không đặt mật khẩu trong source code.
2. Có thể đặt `CPMS_SEED_SAMPLE_DATA=true` để tạo duy nhất dự án mẫu `DEMO-001`.
3. Đăng nhập bằng tài khoản có vai trò phù hợp.
4. Tạo một dự án mẫu và thêm Project Manager/member.
5. Tạo WBS, task, nhân sự, vật tư và thiết bị.
6. Chạy lần lượt sáu quy trình theo bảng trên.
7. Ghi nhận kết quả từng bước và xác nhận trong `Implementation_Progress.md`.
