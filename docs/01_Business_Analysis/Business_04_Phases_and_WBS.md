# Nghiệp vụ 04 — Quản lý giai đoạn và cấu trúc WBS

## Chức năng

Tạo giai đoạn trong dự án và phân rã hạng mục theo cây WBS. Mỗi WorkItem có mã WBS, tên, thứ
tự, cấp độ, trạng thái, giai đoạn và tùy chọn công việc cha.

## Business Rules

- Giai đoạn và WorkItem phải thuộc một Project hợp lệ.
- Công việc cha và con phải thuộc cùng Project.
- Mã WBS duy nhất trong một Project.
- Cấp độ nút gốc là 1; nút con được tính từ cấp độ cha.
- Thứ tự được lưu để hiển thị cây theo trình tự.
- Không tạo nút WBS với giai đoạn của Project khác.

## Phạm vi

Module chỉ quản lý cấu trúc WBS và trạng thái hạng mục. Giao việc, thời gian chi tiết, phần trăm
hoàn thành và lịch sử tiến độ thuộc Nghiệp vụ 05/06.
