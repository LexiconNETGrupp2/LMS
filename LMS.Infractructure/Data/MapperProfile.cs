using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.CourseDtos;

namespace LMS.Infractructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserRegistrationDto, ApplicationUser>();
        CreateMap<UserDto, ApplicationUser>().ReverseMap();
        CreateMap<Course, CourseDto>().ForMember(dest => dest.NumberOfStudents, opt => opt.MapFrom(c => c.Students.Count)).ReverseMap();
        CreateMap<CreateCourseDto, Course>();
        CreateMap<CourseModuleDto, Module>().ReverseMap();
        CreateMap<ActivityDto, Activity>().ReverseMap();
    }
}
