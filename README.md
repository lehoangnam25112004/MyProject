# Construction Project Management System

Hệ thống quản lý dự án thi công xây dựng bằng ASP.NET Core MVC .NET 10, Entity Framework Core,
SQL Server và ASP.NET Core Identity.

## Trạng thái

Giai đoạn hiện tại: **Giai đoạn 2 — Tạo khung dự án và tài liệu thiết kế**.

Dự án mới được tạo độc lập tại `D:\MYPROJECT\ConstructionProjectManagement`. Phạm vi triển khai
chỉ gồm 12 nghiệp vụ và 6 quy trình được mô tả trong Rule.md.

## Chạy dự án

```powershell
dotnet restore
dotnet build
dotnet run
```

Mặc định sử dụng SQL Server Express (`.\SQLEXPRESS`) với connection string trong `appsettings.json`. Có thể thay
đổi connection string theo môi trường triển khai; không commit thông tin xác thực nhạy cảm.

## Tài liệu

- [Kế hoạch dự án ban đầu](docs/05_Project_Management/Initial_Project_Plan.md)
- [Tài liệu sơ đồ](docs/06_Diagrams/README_Diagrams.md)
- [Rule.md](../BuildManagerMVC/Rule.md)
