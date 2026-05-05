using MediatR;
using SchoolManagement.Application.Features.Fees.DTOs;

namespace SchoolManagement.Application.Features.Fees.Queries.GetStudentFees;

public sealed record GetStudentFeesQuery(Guid StudentId) : IRequest<List<StudentFeeDto>>;
