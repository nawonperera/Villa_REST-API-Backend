using AutoMapper;
using Villa_VillaAPI.Model.Dto;
using Villa_VillaAPI.Model.Entity;

namespace Villa_VillaAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDTO>(); // Source -> Target
            CreateMap<VillaDTO, Villa>();

            CreateMap<Villa, VillaCreateDTO>().ReverseMap(); //reverseMap() allows mapping in both directions
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
        }
    }
}
