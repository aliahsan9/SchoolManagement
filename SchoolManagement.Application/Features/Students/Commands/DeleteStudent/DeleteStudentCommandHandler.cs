using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Students.Commands.DeleteStudent
{


    public class DeleteStudentCommandHandler
        : IRequestHandler<DeleteStudentCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantContext _tenant;

        public DeleteStudentCommandHandler(IApplicationDbContext context, ICurrentTenantContext tenant)
        {
            _context = context;
            _tenant = tenant;
        }

        public async Task<bool> Handle(
            DeleteStudentCommand request,
            CancellationToken cancellationToken)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (student == null)
                return false;

            if (_tenant.HasTenant && _tenant.SchoolId is Guid tid && student.SchoolId != tid)
                return false;

            student.IsDeleted = true;
            student.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
