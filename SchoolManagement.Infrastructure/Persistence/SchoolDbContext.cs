using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.MultiTenancy;

namespace SchoolManagement.Infrastructure.Persistence
{
    public class SchoolDbContext(DbContextOptions<SchoolDbContext> options) : DbContext(options), IApplicationDbContext
    {

        #region AUTH

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        #endregion

        #region SCHOOL STRUCTURE

        public DbSet<School> Schools => Set<School>();
        public DbSet<SchoolSubscription> SchoolSubscriptions => Set<SchoolSubscription>();
        public DbSet<SubscriptionPayment> SubscriptionPayments => Set<SubscriptionPayment>();
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
        public DbSet<Classes> Classes => Set<Classes>();
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
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations automatically
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolDbContext).Assembly);

            // SQL Server disallows multiple cascade paths; use NO ACTION by default.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var fk in entityType.GetForeignKeys())
                    fk.DeleteBehavior = DeleteBehavior.NoAction;
            }

            modelBuilder.Entity<RefreshToken>()
                .HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clr = entityType.ClrType;
                if (!typeof(BaseEntity).IsAssignableFrom(clr))
                    continue;

                if (clr == typeof(User))
                {
                    modelBuilder.Entity<User>().HasQueryFilter(u =>
                        !u.IsDeleted &&
                        (TenantScope.SchoolId == null || u.SchoolId == TenantScope.SchoolId));
                    continue;
                }

                if (typeof(ISchoolScoped).IsAssignableFrom(clr))
                {
                    typeof(SchoolDbContext)
                        .GetMethod(nameof(SetSchoolScopedQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                        .MakeGenericMethod(clr)
                        .Invoke(null, [modelBuilder]);
                }
                else
                {
                    typeof(SchoolDbContext)
                        .GetMethod(nameof(SetSoftDeleteQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                        .MakeGenericMethod(clr)
                        .Invoke(null, [modelBuilder]);
                }
            }
        }

        private static void SetSchoolScopedQueryFilter<TEntity>(ModelBuilder modelBuilder)
            where TEntity : BaseEntity, ISchoolScoped
        {
            modelBuilder.Entity<TEntity>()
                .HasQueryFilter(e =>
                    !e.IsDeleted &&
                    (TenantScope.SchoolId == null || e.SchoolId == TenantScope.SchoolId));
        }

        private static void SetSoftDeleteQueryFilter<TEntity>(ModelBuilder modelBuilder)
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
}