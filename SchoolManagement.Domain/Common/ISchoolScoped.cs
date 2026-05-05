namespace SchoolManagement.Domain.Common;

public interface ISchoolScoped
{
    Guid SchoolId { get; }
}
