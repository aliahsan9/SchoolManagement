using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Features.Students.DTOs;

namespace SchoolManagement.Application.Features.Students.Queries.GetStudentsPaged;

public sealed record GetStudentsPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? ClassId = null,
    Guid? SectionId = null,
    string SortBy = "AdmissionNumber",
    bool SortDescending = false) : IRequest<PagedResult<StudentDto>>;
