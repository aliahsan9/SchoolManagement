using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Infrastructure.Persistence
{
    public static class DatabaseInitializer
    {
        public static Task SeedAsync(
            SchoolDbContext db,
            IPasswordHasherService passwordHasher,
            CancellationToken cancellationToken = default) =>
            ApplicationDbSeeder.SeedAsync(db, passwordHasher, cancellationToken);
    }
}