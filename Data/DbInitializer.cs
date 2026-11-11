using ConstructionProjectManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Task = System.Threading.Tasks.Task;

namespace ConstructionProjectManagement.Data;

public static class DbInitializer
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager,
        CancellationToken cancellationToken = default)
    {
        foreach (var roleName in RoleNames.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Không thể tạo vai trò '{roleName}': {errors}");
            }
        }
    }

    public static async Task SeedAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var email = configuration["CPMS_ADMIN_EMAIL"];
        var password = configuration["CPMS_ADMIN_PASSWORD"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "Bỏ qua seed Admin: cần cấu hình CPMS_ADMIN_EMAIL và CPMS_ADMIN_PASSWORD.");
            return;
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = "Quản trị hệ thống",
                IsActive = true
            };
            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Không thể tạo tài khoản Admin: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, RoleNames.Admin))
        {
            var roleResult = await userManager.AddToRoleAsync(user, RoleNames.Admin);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Không thể gán vai trò Admin: {errors}");
            }
        }
    }

    public static async Task ApplyMigrationsAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public static async Task SeedSampleDataAsync(
        ApplicationDbContext dbContext,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        if (!bool.TryParse(configuration["CPMS_SEED_SAMPLE_DATA"], out var enabled) || !enabled)
        {
            return;
        }

        const string sampleCode = "DEMO-001";
        if (await dbContext.Projects.AnyAsync(project => project.ProjectCode == sampleCode, cancellationToken))
        {
            return;
        }

        var adminEmail = configuration["CPMS_ADMIN_EMAIL"];
        var admin = string.IsNullOrWhiteSpace(adminEmail)
            ? null
            : await dbContext.Users.SingleOrDefaultAsync(
                user => user.Email == adminEmail,
                cancellationToken);

        var project = new Project
        {
            ProjectCode = sampleCode,
            Name = "Dự án mẫu kiểm thử",
            Location = "Hà Nội",
            ClientName = "Khách hàng mẫu",
            StartDate = DateTime.UtcNow.Date,
            ExpectedEndDate = DateTime.UtcNow.Date.AddMonths(6),
            Budget = 1_000_000_000m,
            ProjectManagerId = admin?.Id
        };
        project.Phases.Add(new ProjectPhase
        {
            Name = "Chuẩn bị mặt bằng",
            SequenceOrder = 1
        });

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
