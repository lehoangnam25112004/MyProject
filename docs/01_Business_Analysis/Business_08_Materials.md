# Nghiệp vụ 08 — Quản lý vật tư

## Mục tiêu

Quản lý danh mục vật tư, tồn kho và quy trình yêu cầu, duyệt, xuất vật tư theo dự án/công việc.

## Phạm vi triển khai

- Danh mục vật tư gồm mã, tên, đơn vị tính, tồn kho và trạng thái.
- Tạo phiếu yêu cầu gắn với dự án, tùy chọn công việc và nhiều dòng vật tư.
- Materials Staff hoặc người có quyền quản lý duyệt/từ chối phiếu.
- Chỉ phiếu đã duyệt mới được xuất.
- Khi xuất, hệ thống kiểm tra đủ tồn kho, trừ tồn kho và lưu giao dịch.

## Quy tắc dữ liệu

- Mã vật tư là duy nhất.
- Phiếu phải liên kết với dự án đang hoạt động.
- Chỉ vật tư đang hoạt động được đưa vào phiếu.
- Không xuất vượt tồn kho; cập nhật tồn kho và giao dịch trong cùng transaction.
- Lưu người tạo, người duyệt và người thực hiện xuất.

## Màn hình

- `/Materials`: danh mục, tồn kho, phiếu gần đây và thao tác duyệt/từ chối/xuất.
- `/Materials/Create`: thêm vật tư.
- `/Materials/Edit/{id}`: cập nhật vật tư.
- `/Materials/CreateRequest`: tạo phiếu nhiều dòng.

## Kiểm thử đã thực hiện

- Build thành công.
- Migration `AddMaterialsManagement` đã tạo và áp dụng trên SQL Server Express.
- Đã kiểm tra quy tắc tồn kho trong service trước khi ghi giao dịch.
