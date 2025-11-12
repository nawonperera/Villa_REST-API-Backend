using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Villa_Web.Model.Dto;

namespace Villa_Web.Models.ViewModel;

public class VillaNumberUpdateVM
{
    public VillaNumberUpdateDTO VillaNumber { get; set; }
    public VillaNumberUpdateVM()
    {
        VillaNumber = new VillaNumberUpdateDTO();
    }

    [ValidateNever]
    public IEnumerable<SelectListItem> VillaList { get; set; }
}


