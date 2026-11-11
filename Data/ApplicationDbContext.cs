using ConstructionProjectManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskEntity = ConstructionProjectManagement.Models.Task;

namespace ConstructionProjectManagement.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Project> Projects => Set<Project>();

    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    public DbSet<Contract> Contracts => Set<Contract>();

    public DbSet<ProjectDocument> ProjectDocuments => Set<ProjectDocument>();

    public DbSet<ProjectPhase> ProjectPhases => Set<ProjectPhase>();

    public DbSet<WorkItem> WorkItems => Set<WorkItem>();

    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();

    public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();

    public DbSet<TaskDependency> TaskDependencies => Set<TaskDependency>();

    public DbSet<TaskProgressHistory> TaskProgressHistories => Set<TaskProgressHistory>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<ConstructionTeam> ConstructionTeams => Set<ConstructionTeam>();

    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    public DbSet<Material> Materials => Set<Material>();

    public DbSet<MaterialRequest> MaterialRequests => Set<MaterialRequest>();

    public DbSet<MaterialRequestItem> MaterialRequestItems => Set<MaterialRequestItem>();

    public DbSet<MaterialTransaction> MaterialTransactions => Set<MaterialTransaction>();

    public DbSet<Equipment> Equipment => Set<Equipment>();

    public DbSet<EquipmentAssignment> EquipmentAssignments => Set<EquipmentAssignment>();

    public DbSet<ConstructionLog> ConstructionLogs => Set<ConstructionLog>();

    public DbSet<Incident> Incidents => Set<Incident>();

    public DbSet<RfiRequest> RfiRequests => Set<RfiRequest>();

    public DbSet<InspectionRequest> InspectionRequests => Set<InspectionRequest>();

    public DbSet<InspectionHistory> InspectionHistories => Set<InspectionHistory>();

    public DbSet<ProjectExpense> ProjectExpenses => Set<ProjectExpense>();

    public DbSet<ProjectHandover> ProjectHandovers => Set<ProjectHandover>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Project>()
            .HasIndex(project => project.ProjectCode)
            .IsUnique();

        builder.Entity<Project>()
            .Property(project => project.Budget)
            .HasPrecision(18, 2);

        builder.Entity<Project>()
            .HasOne(project => project.ProjectManager)
            .WithMany()
            .HasForeignKey(project => project.ProjectManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProjectMember>()
            .HasIndex(member => new { member.ProjectId, member.UserId })
            .IsUnique();

        builder.Entity<ProjectMember>()
            .HasOne(member => member.Project)
            .WithMany(project => project.Members)
            .HasForeignKey(member => member.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProjectMember>()
            .HasOne(member => member.User)
            .WithMany()
            .HasForeignKey(member => member.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProjectPhase>()
            .HasOne(phase => phase.Project)
            .WithMany(project => project.Phases)
            .HasForeignKey(phase => phase.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProjectPhase>()
            .HasIndex(phase => new { phase.ProjectId, phase.SequenceOrder });

        builder.Entity<WorkItem>()
            .HasOne(item => item.Project)
            .WithMany()
            .HasForeignKey(item => item.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WorkItem>()
            .HasOne(item => item.ProjectPhase)
            .WithMany(phase => phase.WorkItems)
            .HasForeignKey(item => item.ProjectPhaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WorkItem>()
            .HasOne(item => item.ParentWorkItem)
            .WithMany(item => item.Children)
            .HasForeignKey(item => item.ParentWorkItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WorkItem>()
            .HasIndex(item => new { item.ProjectId, item.WbsCode })
            .IsUnique();

        builder.Entity<TaskEntity>()
            .Property(task => task.PercentComplete)
            .HasPrecision(5, 2);

        builder.Entity<TaskEntity>()
            .HasOne(task => task.Project)
            .WithMany()
            .HasForeignKey(task => task.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TaskEntity>()
            .HasOne(task => task.WorkItem)
            .WithMany()
            .HasForeignKey(task => task.WorkItemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<TaskEntity>()
            .HasOne(task => task.ParentTask)
            .WithMany(task => task.Children)
            .HasForeignKey(task => task.ParentTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TaskAssignment>()
            .HasIndex(assignment => new { assignment.TaskId, assignment.UserId })
            .IsUnique();

        builder.Entity<TaskAssignment>()
            .HasOne(assignment => assignment.Task)
            .WithMany(task => task.Assignments)
            .HasForeignKey(assignment => assignment.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TaskAssignment>()
            .HasOne(assignment => assignment.User)
            .WithMany()
            .HasForeignKey(assignment => assignment.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TaskDependency>()
            .HasOne(dependency => dependency.Task)
            .WithMany(task => task.Dependencies)
            .HasForeignKey(dependency => dependency.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TaskDependency>()
            .HasOne(dependency => dependency.PredecessorTask)
            .WithMany()
            .HasForeignKey(dependency => dependency.PredecessorTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TaskProgressHistory>()
            .Property(history => history.PreviousPercentComplete)
            .HasPrecision(5, 2);

        builder.Entity<TaskProgressHistory>()
            .Property(history => history.NewPercentComplete)
            .HasPrecision(5, 2);

        builder.Entity<TaskProgressHistory>()
            .HasOne(history => history.Task)
            .WithMany()
            .HasForeignKey(history => history.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TaskProgressHistory>()
            .HasOne(history => history.UpdatedBy)
            .WithMany()
            .HasForeignKey(history => history.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasIndex(employee => employee.EmployeeCode)
            .IsUnique();

        builder.Entity<Employee>()
            .HasIndex(employee => employee.UserId)
            .IsUnique();

        builder.Entity<Employee>()
            .HasOne(employee => employee.User)
            .WithOne()
            .HasForeignKey<Employee>(employee => employee.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasOne(employee => employee.Department)
            .WithMany(department => department.Employees)
            .HasForeignKey(employee => employee.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<TeamMember>()
            .HasIndex(member => new { member.TeamId, member.EmployeeId })
            .IsUnique();

        builder.Entity<TeamMember>()
            .HasOne(member => member.Team)
            .WithMany(team => team.Members)
            .HasForeignKey(member => member.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TeamMember>()
            .HasOne(member => member.Employee)
            .WithMany()
            .HasForeignKey(member => member.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Material>()
            .HasIndex(material => material.MaterialCode)
            .IsUnique();

        builder.Entity<Material>()
            .Property(material => material.CurrentStock)
            .HasPrecision(18, 3);

        builder.Entity<MaterialRequest>()
            .HasOne(request => request.Project)
            .WithMany()
            .HasForeignKey(request => request.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MaterialRequest>()
            .HasOne(request => request.Task)
            .WithMany()
            .HasForeignKey(request => request.TaskId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<MaterialRequest>()
            .HasOne(request => request.RequestedBy)
            .WithMany()
            .HasForeignKey(request => request.RequestedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MaterialRequest>()
            .HasOne(request => request.ApprovedBy)
            .WithMany()
            .HasForeignKey(request => request.ApprovedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MaterialRequestItem>()
            .HasIndex(item => new { item.MaterialRequestId, item.MaterialId })
            .IsUnique();

        builder.Entity<MaterialRequestItem>()
            .Property(item => item.QuantityRequested)
            .HasPrecision(18, 3);

        builder.Entity<MaterialRequestItem>()
            .Property(item => item.QuantityIssued)
            .HasPrecision(18, 3);

        builder.Entity<MaterialRequestItem>()
            .HasOne(item => item.MaterialRequest)
            .WithMany(request => request.Items)
            .HasForeignKey(item => item.MaterialRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MaterialRequestItem>()
            .HasOne(item => item.Material)
            .WithMany(material => material.RequestItems)
            .HasForeignKey(item => item.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MaterialTransaction>()
            .Property(transaction => transaction.Quantity)
            .HasPrecision(18, 3);

        builder.Entity<MaterialTransaction>()
            .HasOne(transaction => transaction.Material)
            .WithMany(material => material.Transactions)
            .HasForeignKey(transaction => transaction.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Equipment>()
            .HasIndex(equipment => equipment.EquipmentCode)
            .IsUnique();

        builder.Entity<EquipmentAssignment>()
            .HasOne(assignment => assignment.Equipment)
            .WithMany(equipment => equipment.Assignments)
            .HasForeignKey(assignment => assignment.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ConstructionLog>()
            .HasIndex(log => new { log.ProjectId, log.LogDate });

        builder.Entity<ConstructionLog>()
            .HasOne(log => log.Project)
            .WithMany()
            .HasForeignKey(log => log.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ConstructionLog>()
            .HasOne(log => log.CreatedBy)
            .WithMany()
            .HasForeignKey(log => log.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Incident>()
            .HasOne(item => item.Project).WithMany()
            .HasForeignKey(item => item.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Incident>()
            .HasOne(item => item.Task).WithMany()
            .HasForeignKey(item => item.TaskId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Incident>()
            .HasOne(item => item.AssignedTo).WithMany()
            .HasForeignKey(item => item.AssignedToId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Incident>()
            .HasOne(item => item.ReportedBy).WithMany()
            .HasForeignKey(item => item.ReportedById).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RfiRequest>()
            .HasOne(item => item.Project).WithMany()
            .HasForeignKey(item => item.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<RfiRequest>()
            .HasOne(item => item.Task).WithMany()
            .HasForeignKey(item => item.TaskId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<RfiRequest>()
            .HasOne(item => item.CreatedBy).WithMany()
            .HasForeignKey(item => item.CreatedById).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<RfiRequest>()
            .HasOne(item => item.RespondedBy).WithMany()
            .HasForeignKey(item => item.RespondedById).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InspectionRequest>()
            .HasOne(item => item.Project).WithMany()
            .HasForeignKey(item => item.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<InspectionRequest>()
            .HasOne(item => item.Task).WithMany()
            .HasForeignKey(item => item.TaskId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<InspectionRequest>()
            .HasOne(item => item.RequestedBy).WithMany()
            .HasForeignKey(item => item.RequestedById).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<InspectionRequest>()
            .HasOne(item => item.InspectedBy).WithMany()
            .HasForeignKey(item => item.InspectedById).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InspectionHistory>()
            .HasOne(item => item.InspectionRequest).WithMany(request => request.History)
            .HasForeignKey(item => item.InspectionRequestId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<InspectionHistory>()
            .HasOne(item => item.InspectedBy).WithMany()
            .HasForeignKey(item => item.InspectedById).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EquipmentAssignment>()
            .HasOne(assignment => assignment.Project)
            .WithMany()
            .HasForeignKey(assignment => assignment.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProjectExpense>()
            .Property(expense => expense.Amount).HasPrecision(18, 2);
        builder.Entity<ProjectExpense>()
            .HasOne(expense => expense.Project).WithMany()
            .HasForeignKey(expense => expense.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ProjectExpense>()
            .HasOne(expense => expense.RecordedBy).WithMany()
            .HasForeignKey(expense => expense.RecordedById).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ProjectHandover>()
            .HasOne(handover => handover.Project).WithMany()
            .HasForeignKey(handover => handover.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ProjectHandover>()
            .HasOne(handover => handover.HandedOverBy).WithMany()
            .HasForeignKey(handover => handover.HandedOverById).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EquipmentAssignment>()
            .HasOne(assignment => assignment.Task)
            .WithMany()
            .HasForeignKey(assignment => assignment.TaskId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<EquipmentAssignment>()
            .HasOne(assignment => assignment.AssignedTo)
            .WithMany()
            .HasForeignKey(assignment => assignment.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EquipmentAssignment>()
            .HasOne(assignment => assignment.HandedOverBy)
            .WithMany()
            .HasForeignKey(assignment => assignment.HandedOverById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EquipmentAssignment>()
            .HasOne(assignment => assignment.ReturnedBy)
            .WithMany()
            .HasForeignKey(assignment => assignment.ReturnedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MaterialTransaction>()
            .HasOne(transaction => transaction.MaterialRequest)
            .WithMany(request => request.Transactions)
            .HasForeignKey(transaction => transaction.MaterialRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<MaterialTransaction>()
            .HasOne(transaction => transaction.Project)
            .WithMany()
            .HasForeignKey(transaction => transaction.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MaterialTransaction>()
            .HasOne(transaction => transaction.Task)
            .WithMany()
            .HasForeignKey(transaction => transaction.TaskId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<MaterialTransaction>()
            .HasOne(transaction => transaction.PerformedBy)
            .WithMany()
            .HasForeignKey(transaction => transaction.PerformedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Contract>()
            .Property(contract => contract.ContractValue)
            .HasPrecision(18, 2);

        builder.Entity<Contract>()
            .HasIndex(contract => contract.ContractNumber)
            .IsUnique();

        builder.Entity<Contract>()
            .HasOne(contract => contract.Project)
            .WithMany()
            .HasForeignKey(contract => contract.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProjectDocument>()
            .HasOne(document => document.Project)
            .WithMany()
            .HasForeignKey(document => document.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProjectDocument>()
            .HasOne(document => document.Contract)
            .WithMany(contract => contract.Documents)
            .HasForeignKey(document => document.ContractId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<ProjectDocument>()
            .HasOne(document => document.UploadedBy)
            .WithMany()
            .HasForeignKey(document => document.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
