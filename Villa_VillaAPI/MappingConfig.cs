using AutoMapper;
using Villa_VillaAPI.Model;
using Villa_VillaAPI.Model.Dto;
using Villa_VillaAPI.Model.Entity;

namespace Villa_VillaAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Villa, VillaDTO>(); // Source -> Target
        CreateMap<VillaDTO, Villa>();

        CreateMap<Villa, VillaCreateDTO>().ReverseMap(); //reverseMap() allows mapping in both directions
        CreateMap<Villa, VillaUpdateDTO>().ReverseMap();

        CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
        CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
        CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
        CreateMap<ApplicationUser, UserDto>().ReverseMap();
    }
}
