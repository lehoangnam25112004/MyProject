namespace ConstructionProjectManagement.ViewModels;

public sealed class DashboardViewModel
{
    public int ActiveUserCount { get; init; }
    public int ProjectCount { get; init; }
    public int ActiveProjectCount { get; init; }
    public int CompletedProjectCount { get; init; }
    public int OpenTaskCount { get; init; }
    public int OverdueTaskCount { get; init; }
    public int PendingMaterialRequestCount { get; init; }
    public int PendingInspectionCount { get; init; }
    public decimal TotalExpense { get; init; }
}
