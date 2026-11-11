# Nghiệp vụ 01 — Quản lý tài khoản và phân quyền

## Phạm vi

Quản lý đăng nhập, đăng xuất, hồ sơ cơ bản, tạo/cập nhật tài khoản, khóa/kích hoạt tài khoản và
gán một vai trò trong tám vai trò đã thống nhất. ASP.NET Core Identity là cơ chế xác thực duy nhất
của module.

## Actor và quyền

| Actor | Quyền trong module |
|---|---|
| Admin | Xem, tạo, cập nhật, khóa/kích hoạt và gán vai trò |
| Người dùng đã đăng nhập | Xem/cập nhật hồ sơ cá nhân |
| Người chưa đăng nhập | Đăng nhập qua Identity |

## Business Rules áp dụng

- Tài khoản phải có tên đăng nhập và mật khẩu hợp lệ khi tạo.
- Vai trò được chọn từ danh sách Role lưu trong Identity, không hardcode trong giao diện.
- Tài khoản bị khóa được biểu diễn bằng `IsActive = false` và `LockoutEnd`, không thể đăng nhập.
- Chỉ Admin được quản trị tài khoản khác.
- Kết quả thao tác Identity thất bại phải được hiển thị rõ trên form.
- Phạm vi dữ liệu theo dự án sẽ được tích hợp khi module Project và ProjectMember được triển khai.

## Phạm vi chưa triển khai

Phân quyền theo từng chức năng chi tiết và lọc dữ liệu theo dự án cần các entity Project/ProjectMember
của Nghiệp vụ 02. Không giả lập các quan hệ đó trong module hiện tại.
