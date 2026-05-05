using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Features.Students.DTOs;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Students.Queries.GetStudentsPaged;

public sealed class GetStudentsPagedQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetStudentsPagedQuery, PagedResult<StudentDto>>
{
    public async Task<PagedResult<StudentDto>> Handle(GetStudentsPagedQuery request, CancellationToken cancellationToken)
    {
        var q = context.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            q = q.Where(s =>
                s.AdmissionNumber.Contains(term) ||
                (s.User.FirstName + " " + s.User.LastName).Contains(term) ||
                s.User.Email.Contains(term));
        }

        if (request.ClassId is Guid classId)
            q = q.Where(s => s.Enrollments.Any(e => e.ClassId == classId));

        if (request.SectionId is Guid sectionId)
            q = q.Where(s => s.Enrollments.Any(e => e.SectionId == sectionId));

        q = request.SortBy switch
        {
            "Name" => request.SortDescending
                ? q.OrderByDescending(s => s.User.LastName).ThenByDescending(s => s.User.FirstName)
                : q.OrderBy(s => s.User.LastName).ThenBy(s => s.User.FirstName),
            "DOB" => request.SortDescending
                ? q.OrderByDescending(s => s.DOB)
                : q.OrderBy(s => s.DOB),
            "Email" => request.SortDescending
                ? q.OrderByDescending(s => s.User.Email)
                : q.OrderBy(s => s.User.Email),
            _ => request.SortDescending
                ? q.OrderByDescending(s => s.AdmissionNumber)
                : q.OrderBy(s => s.AdmissionNumber)
        };

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new StudentDto
            {
                Id = s.Id,
                UserId = s.UserId,
                SchoolId = s.SchoolId,
                AdmissionNumber = s.AdmissionNumber,
                FullName = s.User.FirstName + " " + s.User.LastName,
                Email = s.User.Email,
                PhoneNumber = s.User.PhoneNumber,
                DOB = s.DOB,
                Gender = s.Gender,
                BloodGroup = s.BloodGroup,
                Address = s.Address
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<StudentDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}
