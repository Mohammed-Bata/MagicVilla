using AutoMapper;
using MagicVillaApi.Models;
using MagicVillaApi.Models.Dtos;

namespace MagicVillaApi
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa,VillaDto>().ReverseMap();
            CreateMap<UserDto,ApplicationUser>().ReverseMap();
        }
    }
}
