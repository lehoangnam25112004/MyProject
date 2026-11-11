# Nghiệp vụ 12 — Chi phí, hoàn thành, bàn giao và báo cáo

## Mục tiêu

Theo dõi chi phí, kiểm soát điều kiện hoàn thành, lập hồ sơ bàn giao và cung cấp báo cáo tổng hợp.

## Phạm vi triển khai

- Ghi nhận khoản chi theo dự án, loại chi phí, mô tả, số tiền và ngày.
- Tổng hợp chi phí đã ghi nhận trên màn hình quản lý.
- Kiểm tra công việc còn mở trước khi bàn giao.
- Lập hồ sơ bàn giao, ghi người bàn giao và người tiếp nhận.
- Xác nhận bàn giao.
- Chỉ lưu trữ dự án khi đã bàn giao xác nhận và không còn công việc mở.
- Hiển thị báo cáo chi phí, công việc còn mở và trạng thái bàn giao.

## Quy tắc dữ liệu

- Chi phí chỉ gắn với dự án đang hoạt động.
- Không lập bàn giao khi dự án còn công việc chưa hoàn thành.
- Không lưu trữ dự án nếu chưa có hồ sơ bàn giao xác nhận.
- Lưu trữ sử dụng `IsArchived`, không xóa cứng dự án.

## Màn hình

- `/Completion`: tổng hợp chi phí, bàn giao và công việc mở.
- `/Completion/CreateExpense`: ghi nhận chi phí.
- `/Completion/CreateHandover`: lập hồ sơ bàn giao.

## Kiểm thử đã thực hiện

- Build thành công.
- Migration `AddCompletionAndReporting` đã tạo và áp dụng trên SQL Server Express.
- Kiểm tra điều kiện bàn giao/lưu trữ trong `CompletionService`.
