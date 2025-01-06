using AutoMapper;
using MagicVillaWeb.Models;
using MagicVillaWeb.Models.Dtos;

namespace MagicVillaWeb
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDto>().ReverseMap();
        }
    }
}
