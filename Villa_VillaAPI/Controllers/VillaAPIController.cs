using Microsoft.AspNetCore.JsonPatch;
using Villa_VillaAPI.Logging;
using Microsoft.AspNetCore.Mvc;
using Villa_VillaAPI.Data;
using Villa_VillaAPI.Model.Dto;

namespace Villa_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {

        private readonly ILogging _logger;

        public VillaAPIController(ILogging logger)
        { 
            _logger = logger;
        }



        /*
         =====================================================
                               Get All
         =====================================================
        */

        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            _logger.Log("Getting All Villas","");
            return Ok(VillaStore.villaList);
        }


        /*
         =====================================================
                               Get One
         =====================================================
        */

        [HttpGet("{id:int}", Name = "GetVilla")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(400)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if(id == 0)
            {
                _logger.Log("Get Villa Error with Id" + id,"error");
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }

            return Ok(villa);
        }


        /*
         =====================================================
                               Create One
         =====================================================
        */


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            if(VillaStore.villaList.FirstOrDefault(u=>u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomerError", "Villa already Exists!");
                return BadRequest(ModelState);

            }
            if(villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            if(villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);

            return CreatedAtRoute("Getvilla",new {id = villaDTO.Id },villaDTO);
            // "CreatedAtRoute" is used when we create a new resource (like a new Villa).
            // It returns HTTP status 201 (Created) + the location (URL) of the new resource.
            //
            // Parameters:
            // 1. "Getvilla" → This is the name of the route we already defined in [HttpGet("{id:int}", Name="Getvilla")].
            //    It tells ASP.NET Core: "use this route to show where the new item can be found."
            //
            // 2. new { id = villaDTO.Id } → This gives the value for the {id} placeholder in the route.
            //    Example: if villaDTO.Id = 5, then the URL will be ".../api/villa/5".
            //
            // 3. villaDTO → This is the actual data (the newly created villa object) that we send back in the response body.
            //
            // So, in short: 
            // It creates a new villa, returns status 201 Created, 
            // and includes both the URL to fetch that villa and the villa details itself.

        }

        /*
         =====================================================
                               Delete One
         =====================================================
        */

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);
            return NoContent();
        }

        /*
         =====================================================
                            Update One (Put)
         =====================================================
        */

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if(villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            villa.Name = villaDTO.Name;
            villa.Sqft = villaDTO.Sqft;
            villa.Occupancy = villaDTO.Occupancy;

            return NoContent();
        }

        /*
         =====================================================
                            Update One (Patch)
         =====================================================
        */

        [HttpPatch("{id:int}", Name="UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if(patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villa, ModelState); // Applies the patch changes from patchDTO to the villa object and records any errors in ModelState.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();

        }
    }
}
