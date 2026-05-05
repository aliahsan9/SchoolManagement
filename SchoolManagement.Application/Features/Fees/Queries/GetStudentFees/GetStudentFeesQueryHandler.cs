using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Fees.DTOs;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Fees.Queries.GetStudentFees;

public sealed class GetStudentFeesQueryHandler(
    IApplicationDbContext context,
    ICurrentTenantContext tenant)
    : IRequestHandler<GetStudentFeesQuery, List<StudentFeeDto>>
{
    public async Task<List<StudentFeeDto>> Handle(GetStudentFeesQuery request, CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant || tenant.SchoolId is null)
            return [];

        var schoolId = tenant.SchoolId.Value;

        var studentOk = await context.Students.AsNoTracking()
            .AnyAsync(s => s.Id == request.StudentId && s.SchoolId == schoolId, cancellationToken);
        if (!studentOk)
            return [];

        var fees = await context.StudentFees.AsNoTracking()
            .Include(f => f.FeeStructure)
            .Where(f => f.StudentId == request.StudentId)
            .ToListAsync(cancellationToken);

        if (fees.Count == 0)
            return [];

        var ids = fees.Select(f => f.Id).ToList();
        var paidLookup = await context.Payments.AsNoTracking()
            .Where(p => ids.Contains(p.StudentFeeId))
            .GroupBy(p => p.StudentFeeId)
            .Select(g => new { g.Key, Sum = g.Sum(x => x.AmountPaid) })
            .ToDictionaryAsync(x => x.Key, x => x.Sum, cancellationToken);

        return fees.ConvertAll(f => new StudentFeeDto
        {
            Id = f.Id,
            StudentId = f.StudentId,
            Amount = f.FeeStructure.Amount,
            DueDate = f.DueDate,
            Status = f.Status,
            TotalPaid = paidLookup.GetValueOrDefault(f.Id)
        });
    }
}
