using API.DTOS;
using API.Entities;
using API.Extentions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>()
                .ForMember(destionation => destionation.PhotoUrl,
                options => options.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url)) //map from AppUSer to MemberDTO 
                .ForMember(destionation => destionation.Age,
                options => options.MapFrom(src => src.DateOfBirth.CalculateAge())); 

            CreateMap<Photo, PhotoDTO>(); //map from photo to photoDTO

            CreateMap<MemberUpdateDTO, AppUser>(); //from MemberUpdateDTO to AppUser
        }
    }
}
