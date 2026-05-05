using AutoMapper;
using SchoolManagement.Application.Features.Students.DTOs;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappings;

/// <summary>
/// Extend with CreateMap rules as you adopt AutoMapper in handlers (use ProjectTo with Includes for navigations).
/// </summary>
public sealed class StudentMappingProfile : Profile
{
    public StudentMappingProfile()
    {
        CreateMap<Student, StudentDto>()
            .ForMember(d => d.FullName, o => o.Ignore())
            .ForMember(d => d.Email, o => o.Ignore())
            .ForMember(d => d.PhoneNumber, o => o.Ignore());
    }
}
