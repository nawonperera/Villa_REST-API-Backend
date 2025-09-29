using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IMapper _mapper;
        public VillaAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }



        /*
         =====================================================
                               Get All
         =====================================================
        */

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villaList)); // Maps the list of Villa entities to a list of VillaDTOs and returns it with an HTTP 200 OK response.
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

        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDTO>(villa));
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
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO createDTO)
        {
            if(await _db.Villas.FirstOrDefaultAsync(u=>u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomerError", "Villa already Exists!");
                return BadRequest(ModelState);
                // ModelState stores validation errors; we use it here to return a BadRequest if input is invalid.

            }
            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            Villa model = _mapper.Map<Villa>(createDTO);

            //Villa model = new()
            //{
            //    // We don’t set Id here. Entity Framework will auto-generate it 
            //    // (like 1, 2, 3, ...) when saving to the database.
            //    Name = createDTO.Name,
            //    Rate = createDTO.Rate,
            //    Sqft = createDTO.Sqft,
            //    Occupancy = createDTO.Occupancy,
            //    Amenity = createDTO.Amenity,
            //    ImageUrl = createDTO.ImageUrl,
            //    Details = createDTO.Details
            //};

            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("Getvilla", new { id = model.Id }, model);
            // CreatedAtRoute → used after adding a new item (like a villa).
            // It returns:
            //   - Status 201 (Created)
            //   - Location (URL) of the new item
            //   - The item itself in the response body
            //
            // Parameters:
            // 1. "Getvilla" → the route name we gave in [HttpGet("{id:int}", Name = "Getvilla")]
            //                 This tells ASP.NET Core which route to use for the new item's URL.
            //
            // 2. new { id = model.Id } → fills in the {id} placeholder in the route.
            //                            Example: if model.Id = 5 → URL becomes ".../api/villa/5".
            //
            // 3. model → the actual data of the newly created villa that will be sent back.
            //
            // Simple meaning: 
            // When you create a new villa, this line returns 201 Created,
            // gives the link to get that villa, and also sends back the villa details.

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
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();
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
        
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
        {
            if(updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            //villa.Name = villaDTO.Name;
            //villa.Sqft = villaDTO.Sqft;
            //villa.Occupancy = villaDTO.Occupancy;

            Villa model = _mapper.Map<Villa>(updateDTO);

            //Villa model = new()
            //{
            //    Id = updateDTO.Id,
            //    Name = updateDTO.Name,
            //    Rate = updateDTO.Rate,
            //    Sqft = updateDTO.Sqft,
            //    Occupancy = updateDTO.Occupancy,
            //    Amenity = updateDTO.Amenity,
            //    ImageUrl = updateDTO.ImageUrl,
            //    Details = updateDTO.Details
            //};

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

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

        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if(patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id); // Fetches the first Villa with the given Id from the database without tracking changes (read-only)

            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            //VillaUpdateDTO villaDTO = new()
            //{
            //    Id = villa.Id,
            //    Name = villa.Name,
            //    Rate = villa.Rate,
            //    Sqft = villa.Sqft,
            //    Occupancy = villa.Occupancy,
            //    Amenity = villa.Amenity,
            //    ImageUrl = villa.ImageUrl,
            //    Details = villa.Details
            //};

            if(villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villaDTO, ModelState); // Applies the patch changes from patchDTO to the villa object and records any errors in ModelState.


            Villa model = _mapper.Map<Villa>(villaDTO);

            //Villa model = new()
            //{
            //    Id = villaDTO.Id,
            //    Name = villaDTO.Name,
            //    Rate = villaDTO.Rate,
            //    Sqft = villaDTO.Sqft,
            //    Occupancy = villaDTO.Occupancy,
            //    Amenity = villaDTO.Amenity,
            //    ImageUrl = villaDTO.ImageUrl,
            //    Details = villaDTO.Details
            //};

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();

        }
    }
}
