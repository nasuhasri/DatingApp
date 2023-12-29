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
            CreateMap<AppUser, MemberDto>()
                /*
                    - specify ForMember and tells the destionation, which member that we are interested in populating here which is PhotoUrl property
                    - specify options: what we want to do? MapFrom something
                    - specify source: where we want to map from so src.Photos
                    - specify photos: look for firstOrDefault which matches the IsMain property
                    - then get the Url
                */
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<Photo, PhotoDto>();
        }
    }
}