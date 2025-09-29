using Villa_VillaAPI.Model.Dto;

namespace Villa_VillaAPI.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> villaList = new List<VillaDTO>
        {
            new VillaDTO { Id = 1, Name = "Ocean View Villa", Sqft = 100, Occupancy = 4 },
            new VillaDTO { Id = 2, Name = "Mountain Retreat Villa", Sqft = 200, Occupancy = 27 },
            new VillaDTO { Id = 3, Name = "City Lights Villa", Sqft = 522, Occupancy = 44 },
            new VillaDTO { Id = 4, Name = "Countryside Villa", Sqft = 785, Occupancy = 9 },

        };
    }
}
