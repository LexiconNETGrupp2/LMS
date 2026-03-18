using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.AuthDtos;

namespace LMS.Infractructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserRegistrationDto, ApplicationUser>();
        CreateMap<UserDto, ApplicationUser>().ReverseMap();
        CreateMap<CourseDto, Course>().ReverseMap();
        CreateMap<ModuleDto, Module>().ReverseMap();
        CreateMap<ActivityDto, Activity>().ReverseMap();
    }
}
