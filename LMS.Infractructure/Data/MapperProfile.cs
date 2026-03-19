using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.ModuleDtos;

namespace LMS.Infractructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserRegistrationDto, ApplicationUser>();

        CreateMap<Module, ModuleDto>()
            .ForCtorParam(nameof(ModuleDto.StartDate), opt => opt.MapFrom(src => src.StartDate.ToDateTime(TimeOnly.MinValue)))
            .ForCtorParam(nameof(ModuleDto.EndDate), opt => opt.MapFrom(src => src.EndDate.ToDateTime(TimeOnly.MinValue)))
            .ForCtorParam(nameof(ModuleDto.CourseId), opt => opt.MapFrom(src => src.Course.Id))
            .ForCtorParam(nameof(ModuleDto.CourseName), opt => opt.MapFrom(src => src.Course.Name));
    }
}
