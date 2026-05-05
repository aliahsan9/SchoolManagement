using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Constants;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Infrastructure.MultiTenancy;

namespace SchoolManagement.Infrastructure.Persistence;

public static class ApplicationDbSeeder
{
    public static async Task SeedAsync(
        SchoolDbContext db,
        IPasswordHasherService passwordHasher,
        CancellationToken cancellationToken = default)
    {
        var prev = TenantScope.SchoolId;
        TenantScope.SchoolId = null;
        try
        {
            if (!await db.Roles.AnyAsync(cancellationToken))
            {
                await db.Roles.AddRangeAsync(
                [
                    new Role { Id = Guid.NewGuid(), Name = RoleNames.Admin, Description = "School administrator" },
                    new Role { Id = Guid.NewGuid(), Name = RoleNames.Teacher, Description = "Teacher" },
                    new Role { Id = Guid.NewGuid(), Name = RoleNames.Student, Description = "Student" },
                    new Role { Id = Guid.NewGuid(), Name = RoleNames.Parent, Description = "Parent / guardian" }
                ], cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            var school = await db.Schools.FirstOrDefaultAsync(s => s.Subdomain == "demo", cancellationToken);
            if (school is null)
            {
                school = new School
                {
                    Id = Guid.NewGuid(),
                    Name = "Demo High School",
                    Subdomain = "demo",
                    IsActive = true,
                    Address = "1 Education Way",
                    City = "Demo City",
                    Country = "Demo Country",
                    Phone = "+1-555-0100",
                    Email = "office@demoschool.edu"
                };
                await db.Schools.AddAsync(school, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            if (!await db.SchoolSubscriptions.AnyAsync(s => s.SchoolId == school.Id, cancellationToken))
            {
                await db.SchoolSubscriptions.AddAsync(new SchoolSubscription
                {
                    Id = Guid.NewGuid(),
                    SchoolId = school.Id,
                    Plan = SubscriptionPlanType.Trial,
                    Status = SubscriptionStatus.Trialing,
                    TrialEndsAtUtc = DateTime.UtcNow.AddDays(SubscriptionPricing.TrialDays),
                    CurrentPeriodEndUtc = null,
                    MonthlyPriceSnapshot = SubscriptionPricing.MonthlyStandardAmount,
                    YearlyPriceSnapshot = SubscriptionPricing.YearlyPremiumAmount
                }, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            var year = await db.AcademicYears.FirstOrDefaultAsync(
                y => y.SchoolId == school.Id && y.IsActive, cancellationToken);
            if (year is null)
            {
                year = new AcademicYear
                {
                    Id = Guid.NewGuid(),
                    SchoolId = school.Id,
                    Name = $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Year + 1}",
                    StartDate = new DateTime(DateTime.UtcNow.Year, 7, 1),
                    EndDate = new DateTime(DateTime.UtcNow.Year + 1, 6, 30),
                    IsActive = true
                };
                await db.AcademicYears.AddAsync(year, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            var cls = await db.Classes.FirstOrDefaultAsync(c => c.SchoolId == school.Id, cancellationToken);
            if (cls is null)
            {
                cls = new Classes
                {
                    Id = Guid.NewGuid(),
                    SchoolId = school.Id,
                    Name = "Grade 10",
                    Description = "Demo class"
                };
                await db.Classes.AddAsync(cls, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            if (!await db.Sections.AnyAsync(s => s.ClassId == cls.Id, cancellationToken))
            {
                await db.Sections.AddAsync(new Section
                {
                    Id = Guid.NewGuid(),
                    ClassId = cls.Id,
                    Name = "A",
                    Capacity = 40
                }, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            if (!await db.Users.AnyAsync(u => u.SchoolId == school.Id && u.Email == "admin@demo.school", cancellationToken))
            {
                var adminRole = await db.Roles.FirstAsync(r => r.Name == RoleNames.Admin, cancellationToken);
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    SchoolId = school.Id,
                    FirstName = "Demo",
                    LastName = "Admin",
                    Email = "admin@demo.school",
                    PhoneNumber = "+10000000000",
                    PasswordHash = string.Empty,
                    IsActive = true
                };
                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");
                await db.Users.AddAsync(adminUser, cancellationToken);
                await db.UserRoles.AddAsync(new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                }, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }
        }
        finally
        {
            TenantScope.SchoolId = prev;
        }
    }
}
