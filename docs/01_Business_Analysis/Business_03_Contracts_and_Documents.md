# Nghiệp vụ 03 — Quản lý hợp đồng và tài liệu dự án

## Chức năng

Quản lý hợp đồng gắn với dự án, số hợp đồng, chủ đầu tư, nhà thầu, giá trị, ngày ký, ngày hiệu
lực và trạng thái. Tài liệu được phân loại, lưu metadata trong Database và tệp trong vùng lưu
trữ riêng không phục vụ trực tiếp từ `wwwroot`.

## Actor và quyền

| Actor | Quyền |
|---|---|
| Admin, Project Director, Project Manager | Tạo/cập nhật hợp đồng và tải tài liệu |
| Người thuộc dự án | Xem hợp đồng/tải tài liệu thuộc phạm vi được cấp |

## Business Rules

- Mỗi hợp đồng phải thuộc một dự án còn hiệu lực.
- Số hợp đồng là duy nhất trong dữ liệu chưa lưu trữ.
- Giá trị hợp đồng không được âm.
- Ngày hiệu lực không được trước ngày ký.
- Tài khoản chỉ thao tác dữ liệu thuộc phạm vi dự án.
- Tệp tối đa 10 MB, chỉ nhận PDF, Word, Excel và ảnh thông dụng.
- Tên tệp lưu vật lý được sinh bằng GUID; tên gốc và metadata lưu trong Database.
- Không quản lý pháp lý hoặc đấu thầu chuyên sâu.
