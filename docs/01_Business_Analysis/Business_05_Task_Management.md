# Nghiệp vụ 05 — Quản lý công việc

## Chức năng

Tạo/cập nhật/xem công việc, gắn với Project và tùy chọn WBS, tạo công việc con, phân công nhân
viên đủ điều kiện, mức ưu tiên, thời gian dự kiến/thực tế, trạng thái và phần trăm hoàn thành.

## Business Rules

- Mỗi Task thuộc một Project hợp lệ.
- WBS và công việc cha phải thuộc cùng Project.
- Chỉ tài khoản đang hoạt động và đã tham gia Project mới được phân công.
- Ngày kết thúc không được trước ngày bắt đầu.
- Phần trăm hoàn thành từ 0 đến 100.
- Task yêu cầu nghiệm thu không được chuyển Completed trước khi có nghiệm thu đạt; hiện trạng
  thái hợp lệ trung gian là PendingInspection.
- TaskAssignment có unique index theo Task/User để không phân công trùng.

## Liên kết

Task sử dụng Project/ProjectMember và WorkItem của Nghiệp vụ 02/04. Lịch sử tiến độ, nghiệm thu,
nhật ký và báo cáo sẽ tích hợp ở các nghiệp vụ tiếp theo.
