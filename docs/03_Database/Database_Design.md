# Thiết kế cơ sở dữ liệu

Database dùng SQL Server và EF Core Code First. Các nhóm bảng chính gồm Identity,
dự án/hợp đồng/WBS/task, nhân sự, vật tư, thiết bị, nhật ký, chất lượng và
chi phí/bàn giao.

Quan hệ và khóa được cấu hình tập trung trong `ApplicationDbContext`. Dữ liệu
nghiệp vụ quan trọng dùng lịch sử giao dịch hoặc lưu trữ mềm thay vì xóa cứng.

Sơ đồ: [04_Database_ERD.mmd](../06_Diagrams/04_Database_ERD.mmd).
