# Nghiệp vụ 06 — Quản lý kế hoạch và tiến độ

## Chức năng

Theo dõi ngày kế hoạch/thực tế, phần trăm hoàn thành, trạng thái, công việc quá hạn, nguyên nhân
chậm và lịch sử cập nhật. Dashboard tiến độ cung cấp biểu đồ so sánh tiến độ kế hoạch tính theo
thời gian với phần trăm thực tế từ Task.

## Business Rules

- Mỗi cập nhật tiến độ tạo một `TaskProgressHistory` chứa giá trị/trạng thái trước và sau,
  người cập nhật, thời điểm và nguyên nhân chậm nếu có.
- Phần trăm hoàn thành từ 0 đến 100.
- Công việc quá hạn và chưa đạt 100% phải có nguyên nhân chậm.
- Công việc yêu cầu nghiệm thu không được chuyển Completed trước khi nghiệm thu đạt.
- Ngày thực tế bắt đầu được ghi khi phần trăm tăng lần đầu; ngày kết thúc ghi khi đạt 100%.
- Dữ liệu biểu đồ lấy từ Database, không dùng số liệu giả.
