using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Villa_VillaAPI.Model;
using Villa_VillaAPI.Repository.IRepository;

namespace Villa_VillaAPI.Controllers.v2;

[Route("api/v{version:apiVersion}/VillaNumberAPI")]
[ApiController]
[ApiVersion("2.0")]
public class VillaNumberAPIController : ControllerBase
{
    // _response → this is an object that will hold the API response we send back to the client. (status, message, data, etc.).
    protected APIResponse _response;
    private readonly IVillaNumberRepository _dbVillaNumber;
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;
    public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper, IVillaRepository dbVilla)
    {
        _dbVillaNumber = dbVillaNumber;
        _mapper = mapper;
        // Create a new empty APIResponse object (ready to be filled and returned).
        _response = new();
        _dbVilla = dbVilla;
    }

    /*
     =====================================================
                           Get All
     =====================================================
    */

    //[MapToApiVersion("2.0")]
    [HttpGet("GetString")]
    public IEnumerable<string> Get()
    {
        return new string[] { "Nawon", "Perera" };
    }

    /*
     =====================================================
                           Get One
     =====================================================
    */

    /*
     =====================================================
                           Create One
     =====================================================
    */

    /*
     =====================================================
                           Delete One
     =====================================================
    */

    /*
     =====================================================
                        Update One (Put)
     =====================================================
    */


}
