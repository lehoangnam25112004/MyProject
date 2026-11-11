# Từ điển dữ liệu rút gọn

| Nhóm | Bảng/Entity | Mục đích |
|---|---|---|
| Identity | `AspNetUsers`, `AspNetRoles` | Tài khoản và vai trò |
| Dự án | `Projects`, `ProjectMembers` | Phạm vi và thành viên |
| Kế hoạch | `ProjectPhases`, `WorkItems`, `Tasks` | Giai đoạn, WBS và công việc |
| Vật tư | `Materials`, `MaterialRequests`, `MaterialTransactions` | Tồn kho và cấp phát |
| Thiết bị | `Equipment`, `EquipmentAssignments` | Danh mục và bàn giao |
| Nhật ký | `ConstructionLogs` | Nhật ký thi công theo ngày |
| Chất lượng | `Incidents`, `RfiRequests`, `InspectionRequests`, `InspectionHistories` | Sự cố, RFI, nghiệm thu |
| Hoàn thành | `ProjectExpenses`, `ProjectHandovers` | Chi phí và bàn giao |

Các Entity đều có khóa chính `Id`; quan hệ khóa ngoại được khai báo trong DbContext.
