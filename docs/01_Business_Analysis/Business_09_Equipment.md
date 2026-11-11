# Nghiệp vụ 09 — Quản lý thiết bị thi công

## Mục tiêu

Quản lý danh mục thiết bị, tình trạng sẵn sàng/bảo trì/đang sử dụng và lịch sử bàn giao theo dự án.

## Phạm vi triển khai

- Quản lý mã, tên, nhóm, ghi chú và trạng thái thiết bị.
- Gán thiết bị đang sẵn sàng cho dự án, công việc và người nhận.
- Ghi nhận người bàn giao, thời điểm bàn giao.
- Thu hồi thiết bị và chuyển về trạng thái sẵn sàng.
- Chuyển thiết bị sẵn sàng sang bảo trì.
- Xem các thiết bị đang được sử dụng và dự án liên quan.

## Quy tắc dữ liệu

- Mã thiết bị là duy nhất.
- Chỉ thiết bị hoạt động và đang sẵn sàng mới được bàn giao.
- Người nhận phải là tài khoản đang hoạt động.
- Thiết bị đang sử dụng không được chuyển trực tiếp sang bảo trì hoặc sẵn sàng; phải thu hồi trước.
- Lịch sử bàn giao không bị xóa khi thu hồi.

## Màn hình

- `/Equipment`: danh mục, tình trạng và thiết bị đang sử dụng.
- `/Equipment/Create`: thêm thiết bị.
- `/Equipment/Edit/{id}`: cập nhật thiết bị.
- `/Equipment/Assign`: bàn giao thiết bị.

## Kiểm thử đã thực hiện

- Build thành công.
- Migration `AddEquipmentManagement` đã tạo và áp dụng trên SQL Server Express.
- Quy tắc trạng thái và bàn giao/thu hồi được kiểm tra trong `EquipmentService`.
