using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MUSM_api_MVCwebapp.Data;
using MUSM_api_MVCwebapp.Dtos;
using MUSM_api_MVCwebapp.Models;

namespace MUSM_api_MVCwebapp.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserCreationDto, AppUser>();
            CreateMap<RequestDto, RequestModel>();
            CreateMap<TaskDto, TaskModel>();
            CreateMap<AppUser, UserDto>();

        }
    }
}
