# Cấu trúc mã nguồn

Mã nguồn được chia theo trách nhiệm:

- `Controllers/`: tiếp nhận request, kiểm tra quyền, gọi Service và trả View.
- `Services/`: quy tắc nghiệp vụ và truy cập dữ liệu qua DbContext.
- `Models/`: Entity và enum nghiệp vụ.
- `ViewModels/`: dữ liệu vào/ra cho giao diện.
- `Data/`: DbContext, seed vai trò và migrations.
- `Views/`: giao diện Razor theo từng Controller.

Sơ đồ phụ thuộc được duy trì tại [03_Code_Dependencies.mmd](../06_Diagrams/03_Code_Dependencies.mmd).
