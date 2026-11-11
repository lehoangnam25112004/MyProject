# Nghiệp vụ 11 — Sự cố, RFI và nghiệm thu

## Mục tiêu

Theo dõi sự cố, yêu cầu làm rõ thông tin và nghiệm thu công việc trong dự án.

## Phạm vi triển khai

- Tạo sự cố gắn với dự án/công việc, mức độ ưu tiên và người xử lý.
- Tạo RFI, ghi nhận câu hỏi và phản hồi.
- Tạo yêu cầu nghiệm thu cho công việc có `RequiresInspection`.
- Ghi nhận kết quả đạt hoặc không đạt, nội dung và phương án khắc phục.
- Lưu lịch sử các lần kiểm tra.
- Công việc đạt nghiệm thu chuyển `Completed`; không đạt chuyển `NeedsRework`.

## Quy tắc dữ liệu

- Dự án phải đang hoạt động; công việc nếu có phải thuộc cùng dự án.
- Không tạo yêu cầu nghiệm thu cho công việc không yêu cầu nghiệm thu.
- Không hoàn thành trực tiếp công việc yêu cầu nghiệm thu; phải qua nghiệm thu đạt.
- Nghiệm thu không đạt bắt buộc thể hiện trạng thái khắc phục.

## Màn hình

- `/Quality`: danh sách sự cố, RFI và yêu cầu nghiệm thu.
- `/Quality/CreateIncident`: tạo sự cố.
- `/Quality/CreateRfi`: tạo RFI.
- `/Quality/RespondRfi/{id}`: phản hồi RFI.
- `/Quality/CreateInspection`: tạo yêu cầu nghiệm thu.

## Kiểm thử đã thực hiện

- Build thành công.
- Migration `AddQualityAndInspection` đã tạo và áp dụng trên SQL Server Express.
- Quy tắc nghiệm thu và chuyển trạng thái công việc được kiểm tra trong `QualityService`.
