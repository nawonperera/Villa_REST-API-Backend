using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Villa_VillaAPI.Data;
using Villa_VillaAPI.Model.Dto;
using Villa_VillaAPI.Model.Entity;

namespace Villa_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        public VillaAPIController(ApplicationDbContext db)
        {
            _db = db;
        }



        /*
         =====================================================
                               Get All
         =====================================================
        */

        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(_db.Villas.ToList());
        }


        /*
         =====================================================
                               Get One
         =====================================================
        */

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
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
            if(_db.Villas.FirstOrDefault(u=>u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
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

            Villa model = new()
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
                Occupancy = villaDTO.Occupancy,
                Amenity = villaDTO.Amenity,
                ImageUrl = villaDTO.ImageUrl,
                Details = villaDTO.Details
            };
            _db.Villas.Add(model);
            _db.SaveChanges();

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
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            _db.SaveChanges();
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
            //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            //villa.Name = villaDTO.Name;
            //villa.Sqft = villaDTO.Sqft;
            //villa.Occupancy = villaDTO.Occupancy;

            Villa model = new()
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
                Occupancy = villaDTO.Occupancy,
                Amenity = villaDTO.Amenity,
                ImageUrl = villaDTO.ImageUrl,
                Details = villaDTO.Details
            };

            _db.Villas.Update(model);
            _db.SaveChanges();

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
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id); // Fetches the first Villa with the given Id from the database without tracking changes (read-only)

            VillaDTO villaDTO = new()
            {
                Id = villa.Id,
                Name = villa.Name,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
                Occupancy = villa.Occupancy,
                Amenity = villa.Amenity,
                ImageUrl = villa.ImageUrl,
                Details = villa.Details
            };

            if(villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villaDTO, ModelState); // Applies the patch changes from patchDTO to the villa object and records any errors in ModelState.

            Villa model = new()
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
                Occupancy = villaDTO.Occupancy,
                Amenity = villaDTO.Amenity,
                ImageUrl = villaDTO.ImageUrl,
                Details = villaDTO.Details
            };

            _db.Villas.Update(model);
            _db.SaveChanges();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();

        }
    }
}
