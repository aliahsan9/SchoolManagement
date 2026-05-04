using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence;

public class SchoolDbContext : DbContext
{
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
    {
    }

    #region AUTH

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    #endregion

    #region SCHOOL STRUCTURE

    public DbSet<School> Schools => Set<School>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Section> Sections => Set<Section>();

    #endregion

    #region STUDENT

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<StudentParent> StudentParents => Set<StudentParent>();
    public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();

    #endregion

    #region TEACHER

    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<TeacherAssignment> TeacherAssignments => Set<TeacherAssignment>();

    #endregion

    #region SUBJECT

    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();

    #endregion

    #region ATTENDANCE

    public DbSet<StudentAttendance> StudentAttendances => Set<StudentAttendance>();
    public DbSet<TeacherAttendance> TeacherAttendances => Set<TeacherAttendance>();

    #endregion

    #region EXAMS

    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamSubject> ExamSubjects => Set<ExamSubject>();
    public DbSet<Result> Results => Set<Result>();

    #endregion

    #region FEES

    public DbSet<FeeType> FeeTypes => Set<FeeType>();
    public DbSet<FeeStructure> FeeStructures => Set<FeeStructure>();
    public DbSet<StudentFee> StudentFees => Set<StudentFee>();
    public DbSet<Payment> Payments => Set<Payment>();

    #endregion

    #region TIMETABLE

    public DbSet<Timetable> Timetables => Set<Timetable>();

    #endregion

    #region NOTIFICATIONS

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRecipient> NotificationRecipients => Set<NotificationRecipient>();

    #endregion

    #region SYSTEM

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Setting> Settings => Set<Setting>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations automatically
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolDbContext).Assembly);

        // Global query filter for soft delete (VERY IMPORTANT)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(SchoolDbContext)
                    .GetMethod(nameof(SetGlobalQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, new object[] { modelBuilder });
            }
        }
    }

    private static void SetGlobalQueryFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => !e.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}