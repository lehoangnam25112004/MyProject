# Hướng dẫn cho tác nhân phát triển

## Phạm vi

Đây là hệ thống quản lý dự án thi công xây dựng theo đúng 12 nghiệp vụ và 6 quy trình trong
[Rule.md](../BuildManagerMVC/Rule.md). Không mở rộng sang ERP, kế toán chuyên sâu, BIM, IoT hoặc
các chức năng ngoài phạm vi đã thống nhất.

## Kiến trúc

Tuân thủ luồng `Views → Controllers → Services → ApplicationDbContext → SQL Server`.
Controller chỉ tiếp nhận request, kiểm tra quyền/dữ liệu và gọi Service. Business rule phải nằm
trong Service hoặc lớp domain phù hợp. Không truy vấn Database trong View.

## Quy ước

- Tên class, method, property, biến và bảng Database dùng tiếng Anh theo quy ước C#.
- Nội dung giao diện, tài liệu và thông báo người dùng dùng tiếng Việt.
- Mỗi Entity, Controller và Service chính nằm ở file riêng.
- Không dùng dữ liệu hardcode thay cho Database thật.
- Không xóa cứng dữ liệu quan trọng đã phát sinh giao dịch.
- Sau mỗi module phải build, kiểm thử và cập nhật tài liệu/sơ đồ phản ánh code thực tế.
- Không chuyển sang module tiếp theo khi module trước chưa được người dùng xác nhận.
