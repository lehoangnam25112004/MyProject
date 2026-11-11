# Nghiệp vụ 07 — Nhân sự và đội thi công

## Mục tiêu

Quản lý hồ sơ nhân viên, phòng ban, đội thi công và thành viên của từng đội.

## Phạm vi

- Tạo và cập nhật mã nhân viên, chức danh, số điện thoại, ngày vào làm và trạng thái hoạt động.
- Gắn nhân viên với phòng ban.
- Tạo đội thi công và mô tả đội.
- Thêm nhân viên đang hoạt động vào đội; không tạo thành viên trùng.
- Chỉ Admin và Project Director được quản lý đội thi công.

## Quy tắc dữ liệu

- Nhân viên phải liên kết với tài khoản Identity hợp lệ.
- Chỉ nhân viên và tài khoản đang hoạt động được thêm vào đội.
- Không xóa cứng hồ sơ nhân viên; sử dụng trạng thái `IsActive`.
- Mã nhân viên và tên phòng ban được quản lý duy nhất theo cấu hình dữ liệu.

## Màn hình và endpoint

- `/Employees`: danh sách và CRUD hồ sơ nhân viên.
- `/ProjectMembers`: thêm nhân sự đang hoạt động vào dự án.
- `/Teams`: danh sách đội, tạo đội và thêm thành viên.

## Kiểm thử đã thực hiện

- Build dự án thành công.
- Migration `AddEmployeesAndTeams` đã tồn tại và database đã ở trạng thái up-to-date.
- `/Employees` và `/Teams` trả HTTP 302 khi chưa đăng nhập, đúng với `[Authorize]`.
