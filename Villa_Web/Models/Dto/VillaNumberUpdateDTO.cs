using System.ComponentModel.DataAnnotations;

namespace Villa_Web.Model.Dto
{
    public class VillaNumberUpdateDTO
    {
        [Required]
        public int VillaNo { get; set; }
        [Required]
        public int VillaId { get; set; }
        public string? SpecialDetails { get; set; }
    }
}
