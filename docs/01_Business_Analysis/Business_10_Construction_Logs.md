# Nghiệp vụ 10 — Nhật ký thi công

## Mục tiêu

Ghi nhận hoạt động thi công theo ngày và tra cứu lịch sử theo dự án.

## Phạm vi triển khai

- Tạo và cập nhật nhật ký theo ngày, gắn với dự án.
- Ghi nhận người lập, thời tiết, nhân lực, công việc đã thực hiện.
- Ghi nhận vật tư, thiết bị sử dụng và vấn đề phát sinh.
- Đính kèm ảnh JPG/PNG tối đa 10 MB.
- Lưu ảnh ngoài `wwwroot` bằng tên lưu trữ ngẫu nhiên.
- Lọc lịch sử nhật ký theo dự án.

## Quy tắc dữ liệu

- Chỉ dự án chưa lưu trữ mới được lập nhật ký.
- Người lập lấy từ tài khoản đang đăng nhập.
- Tệp đính kèm chỉ nhận JPG/PNG, giới hạn 10 MB.
- Tệp không được phục vụ trực tiếp từ `wwwroot`.

## Màn hình

- `/ConstructionLogs`: lịch sử nhật ký và lọc theo dự án.
- `/ConstructionLogs/Create`: tạo nhật ký.
- `/ConstructionLogs/Edit/{id}`: cập nhật nhật ký.

## Kiểm thử đã thực hiện

- Build thành công.
- Migration `AddConstructionLogs` đã tạo và áp dụng trên SQL Server Express.
- Kiểm tra giới hạn và whitelist ảnh trong `ConstructionLogService`.
