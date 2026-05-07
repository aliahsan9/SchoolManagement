using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(User user, string password);
        bool VerifyPassword(User user, string password, string passwordHash);
    }
}