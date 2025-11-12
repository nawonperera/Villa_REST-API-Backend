using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Villa_Web.Model.Dto;

namespace Villa_Web.Models.ViewModel;

public class VillaNumberCreateVM
{
    // Constructor — automatically creates a new VillaNumberCreateDTO object
    // so it's not null when the form is displayed
    public VillaNumberCreateVM()
    {
        VillaNumber = new VillaNumberCreateDTO();
    }

    // Holds the data for the villa number being created (form input)
    public VillaNumberCreateDTO VillaNumber { get; set; }

    // Contains a list of villas to display in a dropdown menu (for selection)
    // [ValidateNever] tells ASP.NET not to validate this property
    // since it’s only used for displaying data, not user input
    [ValidateNever]
    public IEnumerable<SelectListItem> VillaList { get; set; }
}


