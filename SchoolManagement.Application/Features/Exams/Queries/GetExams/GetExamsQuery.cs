using MediatR;
using SchoolManagement.Application.Features.Exams.DTOs;

namespace SchoolManagement.Application.Features.Exams.Queries.GetExams;

public sealed record GetExamsQuery : IRequest<List<ExamListDto>>;
