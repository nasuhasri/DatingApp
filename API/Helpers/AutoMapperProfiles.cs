using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            /*
                - Idea behind AutoMapper: compare all the properties by their name and type
                - for Age(), since we have method GetAge(), AutoMapper auto knows that it will get int age for Age()
            */
            // create map from AppUser to MemberDto
            CreateMap<AppUser, MemberDto>();
            CreateMap<Photo, PhotoDto>();
        }
    }
}