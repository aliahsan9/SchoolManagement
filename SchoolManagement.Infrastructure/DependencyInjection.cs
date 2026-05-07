using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Application.Common.Configuration;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Infrastructure.Payment;
using SchoolManagement.Infrastructure.Persistence;
using SchoolManagement.Infrastructure.Persistence.Interceptors;
using SchoolManagement.Infrastructure.Services;

namespace SchoolManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<AuditSaveChangesInterceptor>();
            services.AddDbContext<SchoolDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString)
                    .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
            });

            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<SchoolDbContext>());

            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IAuthenticationTokenService, AuthenticationTokenService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICurrentTenantContext, CurrentTenantContext>();
            services.AddScoped<IPaymentGateway, FakePaymentGateway>();

            return services;
        }
    }
}