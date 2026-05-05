namespace SchoolManagement.Application.Interfaces;

public interface ICurrentTenantContext
{
    Guid? SchoolId { get; }
    bool HasTenant { get; }
}
