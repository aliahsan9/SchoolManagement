namespace SchoolManagement.Application.Features.Lookups.DTOs;

public sealed class AcademicCatalogDto
{
    public List<IdNameItemDto> AcademicYears { get; init; } = [];
    public List<IdNameItemDto> Classes { get; init; } = [];
    public List<SectionItemDto> Sections { get; init; } = [];
}

public sealed class IdNameItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
}

public sealed class SectionItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public Guid ClassId { get; init; }
}
