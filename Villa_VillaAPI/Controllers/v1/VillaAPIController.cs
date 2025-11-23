using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using Villa_VillaAPI.Model;
using Villa_VillaAPI.Model.Dto;
using Villa_VillaAPI.Model.Entity;
using Villa_VillaAPI.Repository.IRepository;

namespace Villa_VillaAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaAPIController : ControllerBase
    {
        // _response → this is an object that will hold the API response we send back to the client. (status, message, data, etc.).
        protected APIResponse _response;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
            // Create a new empty APIResponse object (ready to be filled and returned).
            _response = new();
        }

        /*
         =====================================================
                               Get All
         =====================================================
        */

        [HttpGet]
        //[ResponseCache(Duration = 30)]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "filterOccupancy")] int? occupancy,
            [FromQuery] string? search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Villa> villaList;

                if (occupancy > 0)
                {
                    villaList = await _dbVilla.GetAllAsync(u => u.Occupancy == occupancy, pageSize: pageSize,
                        pageNumber: pageNumber);
                }
                else
                {
                    villaList = await _dbVilla.GetAllAsync(pageSize: pageSize,
                        pageNumber: pageNumber);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    villaList = villaList.Where(u => u.Amenity.ToLower().Contains(search)
                    || u.Name.ToLower().Contains(search));
                }

                Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };
                Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagination));

                // Maps the list of Villa entities to a list of VillaDTOs and returns it with an HTTP 200 OK response.
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        /*
         =====================================================
                               Get One
         =====================================================
        */

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ResponseCache(Duration = 30)] // Caches the response for 30 seconds. During this time, repeated requests return the cached result instead of re-running the action.
        // [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)] // Disables caching completely. Every request gets a fresh response, and nothing is stored in any cache.
        // Location = ResponseCacheLocation.None => // Do not store the response in any cache (not in browser, server, or proxy).
        // NoStore = true => // Do not save the response at all — even temporarily. Ensures the response is always generated fresh.
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;

        }


        /*
         =====================================================
                               Create One
         =====================================================
        */


        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa already Exists!");
                    return BadRequest(ModelState);

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

                await _dbVilla.CreateAsync(model);
                _response.Result = _mapper.Map<VillaDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("Getvilla", new { id = model.Id }, _response);
                //CreatedAtRoute(...) A helper method from ControllerBase that returns 201 Created + route location
                // "Getvilla"  The name of the route to which this newly created resource belongs
                // new { id = model.Id }   The route values(used to generate the URL for that route)
                // _response The actual data object you send back to the client
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        /*
         =====================================================
                               Delete One
         =====================================================
        */

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }
                await _dbVilla.RemoveAsync(villa);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        /*
         =====================================================
                            Update One (Put)
         =====================================================
        */

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }

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

                await _dbVilla.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;

        }

        /*
         =====================================================
                            Update One (Patch)
         =====================================================
        */

        [Authorize(Roles = "admin")]
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false); // Fetches the first Villa with the given Id from the database without tracking changes (read-only)

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

            if (villa == null)
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

            await _dbVilla.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();

        }
    }
}
