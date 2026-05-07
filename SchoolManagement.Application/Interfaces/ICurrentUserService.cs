namespace SchoolManagement.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? SchoolId { get; }
        string? Email { get; }
        IReadOnlyList<string> Roles { get; }
    }
}