using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;
        public bool IsActive { get; set; }

        public Guid? SchoolId { get; set; }
        public School? School { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}