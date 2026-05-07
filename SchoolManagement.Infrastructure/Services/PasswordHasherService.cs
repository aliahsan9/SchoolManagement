using Microsoft.AspNetCore.Identity;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Services
{
    public sealed class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<User> _hasher = new();

        public string HashPassword(User user, string password) =>
            _hasher.HashPassword(user, password);

        public bool VerifyPassword(User user, string password, string passwordHash) =>
            _hasher.VerifyHashedPassword(user, passwordHash, password)
            is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}