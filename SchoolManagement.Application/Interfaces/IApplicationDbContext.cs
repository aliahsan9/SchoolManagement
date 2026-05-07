using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<UserRole> UserRoles { get; }
        DbSet<School> Schools { get; }
        DbSet<SchoolSubscription> SchoolSubscriptions { get; }
        DbSet<SubscriptionPayment> SubscriptionPayments { get; }
        DbSet<Student> Students { get; }
        DbSet<Parent> Parents { get; }
        DbSet<StudentParent> StudentParents { get; }
        DbSet<StudentEnrollment> StudentEnrollments { get; }
        DbSet<Classes> Classes { get; }
        DbSet<Section> Sections { get; }
        DbSet<AcademicYear> AcademicYears { get; }
        DbSet<Teacher> Teachers { get; }
        DbSet<StudentFee> StudentFees { get; }
        DbSet<Payment> Payments { get; }
        DbSet<FeeStructure> FeeStructures { get; }
        DbSet<FeeType> FeeTypes { get; }
        DbSet<Exam> Exams { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<AuditLog> AuditLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}