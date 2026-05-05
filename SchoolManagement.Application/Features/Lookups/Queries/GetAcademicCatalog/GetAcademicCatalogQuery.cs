using MediatR;
using SchoolManagement.Application.Features.Lookups.DTOs;

namespace SchoolManagement.Application.Features.Lookups.Queries.GetAcademicCatalog;

public sealed record GetAcademicCatalogQuery : IRequest<AcademicCatalogDto>;
