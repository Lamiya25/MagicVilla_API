using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        public readonly ILogger<VillaAPIController> _logger;
        private readonly ApplicationDbContext _db;
        public VillaAPIController(ILogger<VillaAPIController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;

        }
        [HttpGet]
        public ActionResult< IEnumerable<VillaDTO>> GetVillas()
        {
            _logger.LogInformation("Getting all villas");
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("{id}", Name ="GetVilla")]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id==0)
            {
                _logger.LogError("GEt Villa Error with Id" + id);
                return BadRequest();
            }
          var villa= _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa ==null)
            {
                return NotFound();
            }

            return Ok(villa);
        }


        [HttpPost]
        public ActionResult<VillaDTO>CreateVilla([FromBody]VillaCreateDTO villaDTO)
        {
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            //if (villaDTO.Id>0)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft
            };
            _db.Villas.Add(model);
            _db.SaveChanges();
            return CreatedAtRoute("GetVilla", new {id=model.Id} ,model);
        }
        [HttpDelete("{id}", Name ="DeleteVilla")]
        public ActionResult DeleteVilla(int id)
        {

            if (id == 0)
            {
                return BadRequest();
            }
          var villa=_db.Villas.FirstOrDefault(u=>u.Id == id);
            if (villa==null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }


        [HttpPut]
        public  IActionResult UpdateVilla(int id, [FromBody]VillaUpdateDTO villaDTO)
        {
            if (villaDTO==null || id !=villaDTO.Id)
            {
                return BadRequest();
            }
            /*  var villa= _db.Villas.FirstOrDefault(u=>u.Id==id);
              villa.Name=villaDTO.Name;
              villa.Occupancy=villaDTO.Occupancy;
              villa.Sqft=villaDTO.Sqft;*/

            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft
            };
            _db.Villas.Update(model);
            _db.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id}", Name = "UpdatePartialVilla")]
        //op:replace, add, remove
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id);


            VillaUpdateDTO villaDTO = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft
            };

            if (villa == null)
            {
                return BadRequest();
            }
            try
            {
                patchDTO.ApplyTo(villaDTO);
                Villa model = new Villa()
                {
                    Amenity = villa.Amenity,
                    Details = villa.Details,
                    Id = villa.Id,
                    ImageUrl = villa.ImageUrl,
                    Name = villa.Name,
                    Occupancy = villa.Occupancy,
                    Rate = villa.Rate,
                    Sqft = villa.Sqft
                };
                _db.Villas.Update(model);
                _db.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
