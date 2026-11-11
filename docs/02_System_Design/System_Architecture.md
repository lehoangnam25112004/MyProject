# Kiến trúc hệ thống

Hệ thống sử dụng ASP.NET Core MVC .NET 10, Service Layer, Entity Framework Core,
SQL Server và ASP.NET Core Identity.

Luồng xử lý:

`Razor View → Controller → Service → ApplicationDbContext → SQL Server`

Identity cung cấp xác thực, vai trò và phân quyền; các Service thực thi quy tắc
nghiệp vụ trước khi lưu Entity.

Sơ đồ chi tiết: [02_System_Architecture.mmd](../06_Diagrams/02_System_Architecture.mmd).
