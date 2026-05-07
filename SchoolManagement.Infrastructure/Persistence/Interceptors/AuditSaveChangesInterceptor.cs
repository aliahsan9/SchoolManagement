using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.MultiTenancy;

namespace SchoolManagement.Infrastructure.Persistence.Interceptors
{
    public sealed class AuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is SchoolDbContext db)
            {
                var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                _ = Guid.TryParse(userIdStr, out var userId);

                var entries = db.ChangeTracker.Entries<BaseEntity>()
                    .Where(e => e.Entity is not AuditLog &&
                                e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                    .ToList();

                foreach (var entry in entries)
                {
                    var action = entry.State switch
                    {
                        EntityState.Added => "Create",
                        EntityState.Modified => "Update",
                        EntityState.Deleted => "Delete",
                        _ => "Unknown"
                    };

                    await db.AuditLogs.AddAsync(new AuditLog
                    {
                        Id = Guid.NewGuid(),
                        SchoolId = TenantScope.SchoolId,
                        UserId = userId == Guid.Empty ? null : userId,
                        Action = action,
                        TableName = entry.Entity.GetType().Name,
                        RecordId = entry.Entity.Id,
                        Timestamp = DateTime.UtcNow
                    }, cancellationToken);
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}