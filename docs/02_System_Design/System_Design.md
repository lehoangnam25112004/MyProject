# Thiết kế hệ thống

- **Presentation:** Controllers, ViewModels và Razor Views tiếng Việt.
- **Business:** các Service theo module: dự án, hợp đồng, WBS, task, tiến độ,
  nhân sự, vật tư, thiết bị, nhật ký, chất lượng và hoàn thành.
- **Data:** `ApplicationDbContext`, Code First migrations và SQL Server Express.
- **Security:** ASP.NET Core Identity, Roles và kiểm tra phạm vi dự án.
- **Storage:** tệp tài liệu/ảnh lưu ngoài `wwwroot` bằng tên ngẫu nhiên.

Các thao tác quan trọng như xuất vật tư, nghiệm thu và lưu trữ dự án đều kiểm tra
điều kiện nghiệp vụ trước khi ghi dữ liệu.
