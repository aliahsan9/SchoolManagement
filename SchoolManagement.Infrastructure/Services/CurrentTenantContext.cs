using SchoolManagement.Application.Interfaces;
using SchoolManagement.Infrastructure.MultiTenancy;

namespace SchoolManagement.Infrastructure.Services
{
    public sealed class CurrentTenantContext : ICurrentTenantContext
    {
        public Guid? SchoolId => TenantScope.SchoolId;
        public bool HasTenant => TenantScope.SchoolId.HasValue;
    }
}