using AutoMapper;
using HRMS.Domain.Entities;

namespace HRMS.Application.Mappings;

/// <summary>
/// AutoMapper profile for Entity to DTO mappings.
/// Keeps presentation layer separate from domain model.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Employee mappings
        CreateMap<Employee, DTOs.EmployeeDto>()
            .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.EmployeeDepartment));

        CreateMap<DTOs.CreateEmployeeDto, Employee>()
            .ForMember(dest => dest.EmployeeDepartment, opt => opt.MapFrom(src => src.Department))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore());

        CreateMap<DTOs.UpdateEmployeeDto, Employee>()
            .ForMember(dest => dest.EmployeeDepartment, opt => opt.MapFrom(src => src.Department))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
