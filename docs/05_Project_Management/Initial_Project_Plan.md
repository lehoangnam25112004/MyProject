# Kế hoạch dự án ban đầu

## 1. Kết quả khảo sát

Workspace `D:\MYPROJECT` đã có dự án `BuildManagerMVC`. Dự án mới được tạo độc lập tại
`D:\MYPROJECT\ConstructionProjectManagement` để tránh ghi đè hoặc làm thay đổi dự án cũ.
Môi trường có .NET SDK 10.0.101 và mục tiêu framework là `net10.0`.

## 2. Mục tiêu và phạm vi

Hệ thống quản lý các hoạt động từ khởi tạo dự án, lập kế hoạch, tổ chức thi công, theo dõi tiến
độ, quản lý nguồn lực, kiểm soát chất lượng đến hoàn thành và bàn giao. Phạm vi cố định gồm 12
nghiệp vụ và 6 quy trình trong Rule.md; không triển khai ERP toàn diện, BIM, AI dự báo, IoT hoặc
kế toán chuyên sâu.

## 3. Kiến trúc

Luồng xử lý chuẩn:

`Views → Controllers → Services → ApplicationDbContext → SQL Server`

ASP.NET Core Identity và Authorization quản lý tài khoản, vai trò, quyền và phạm vi dữ liệu
theo dự án. Entity được tách khỏi ViewModel để bảo vệ ranh giới giao diện và dữ liệu.

## 4. Database sơ bộ

Database Code First dự kiến gồm nhóm Identity; dự án/hợp đồng/tài liệu; giai đoạn/WBS/công việc;
nhân sự và đội thi công; vật tư; thiết bị; nhật ký; sự cố/RFI/nghiệm thu; chi phí/bàn giao/báo
cáo. Mỗi quan hệ sẽ có khóa chính, khóa ngoại và ràng buộc phù hợp. Tiền tệ dùng Decimal,
nghiệp vụ cần theo dõi có trạng thái và lịch sử, dữ liệu quan trọng hỗ trợ xóa mềm hoặc lưu trữ.

ERD và Data Dictionary sẽ được chốt trước khi triển khai Entity nghiệp vụ.

## 5. Kế hoạch 10 tuần

| Tuần | Nội dung |
|---|---|
| 1 | Khảo sát, phạm vi, kiến trúc, ERD, tài liệu và sơ đồ thiết kế |
| 2 | Khung project, SQL Server, EF Core, Identity, cấu trúc thư mục |
| 3 | Tài khoản, vai trò, phân quyền và phạm vi dự án |
| 4 | Dự án, hợp đồng, tài liệu, giai đoạn và WBS |
| 5 | Công việc, phân công, kế hoạch và tiến độ |
| 6 | Nhân sự, đội thi công, vật tư và cấp phát |
| 7 | Thiết bị, nhật ký thi công và tích hợp nguồn lực |
| 8 | Sự cố, RFI, kiểm tra và nghiệm thu |
| 9 | Chi phí, hoàn thành, bàn giao và báo cáo |
| 10 | Kiểm thử tích hợp 6 quy trình, hoàn thiện tài liệu và sơ đồ |

## 6. Trạng thái kiểm soát

| Hạng mục | Trạng thái |
|---|---|
| Khảo sát workspace | APPROVED |
| Tạo khung dự án | WAITING_FOR_USER_TEST |
| Tài liệu thiết kế ban đầu | CODING |
| Nền tảng MVC/EF/Identity | WAITING_FOR_USER_TEST |
| 12 nghiệp vụ | NOT_STARTED |
| Người dùng xác nhận | Chưa xác nhận |
