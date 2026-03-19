using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.ModuleDtos;

namespace LMS.Infractructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserRegistrationDto, ApplicationUser>();
        CreateMap<UserDto, ApplicationUser>().ReverseMap();
        CreateMap<Course, CourseDto>().ForMember(dest => dest.NumberOfStudents, opt => opt.MapFrom(c => c.Students.Count)).ReverseMap();
        CreateMap<CourseModuleDto, Module>().ReverseMap();
        CreateMap<ActivityDto, Activity>().ReverseMap();
        CreateMap<Module, ModuleDto>()
            .ForCtorParam(nameof(ModuleDto.StartDate), opt => opt.MapFrom(src => src.StartDate.ToDateTime(TimeOnly.MinValue)))
            .ForCtorParam(nameof(ModuleDto.EndDate), opt => opt.MapFrom(src => src.EndDate.ToDateTime(TimeOnly.MinValue)))
            .ForCtorParam(nameof(ModuleDto.CourseId), opt => opt.MapFrom(src => src.Course.Id))
            .ForCtorParam(nameof(ModuleDto.CourseName), opt => opt.MapFrom(src => src.Course.Name));
    }
}
