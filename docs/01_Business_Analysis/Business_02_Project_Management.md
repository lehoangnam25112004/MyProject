# Nghiệp vụ 02 — Quản lý dự án

## Chức năng

Tạo, xem danh sách, xem chi tiết và cập nhật dự án; quản lý mã, tên, địa điểm, chủ đầu tư, thời
gian, ngân sách, trạng thái, Project Manager và thành viên dự án.

## Actor và quyền

| Actor | Quyền |
|---|---|
| Admin, Project Director | Tạo và cập nhật dự án; gán người quản lý và thành viên |
| Người dùng thuộc dự án | Xem dự án được phân công |

## Business Rules

- Mã dự án là duy nhất trong dữ liệu chưa lưu trữ.
- Ngày kết thúc dự kiến không được trước ngày bắt đầu.
- Ngân sách không được âm và lưu bằng Decimal.
- Chỉ tài khoản đang hoạt động được gán làm Project Manager hoặc thành viên.
- Người không phải Admin/Project Director chỉ xem được dự án do mình quản lý hoặc tham gia.
- Thành viên dự án không được trùng trong cùng một dự án.
- Chưa có thao tác xóa cứng; dữ liệu hỗ trợ `IsArchived` để lưu trữ.

## Liên kết

Project là nền tảng cho hợp đồng, WBS, công việc, nhân sự, vật tư, thiết bị, nhật ký, nghiệm thu,
chi phí và bàn giao ở các nghiệp vụ tiếp theo.
