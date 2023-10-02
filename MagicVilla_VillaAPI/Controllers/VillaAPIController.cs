using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repositories.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using System.Linq;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        public readonly ILogger<VillaAPIController> _logger;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        public VillaAPIController(ILogger<VillaAPIController> logger, IVillaRepository dbVilla, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _dbVilla = dbVilla;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            _logger.LogInformation("Getting all villas");
            IEnumerable<Villa> villaList=await _dbVilla.GetAllAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }

        [HttpGet("{id}", Name ="GetVilla")]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id==0)
            {
                _logger.LogError("Get Villa Error with Id" + id);
                return BadRequest();
            }
         var villa= await _dbVilla.GetAsync( u => u.Id == id);
            if (villa ==null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDTO>(villa));
        }


        [HttpPost]
        public async Task<ActionResult<VillaDTO>>CreateVilla([FromBody]VillaCreateDTO createDTO)
        {
            if (await _dbVilla.GetAsync(u=>u.Name.ToLower()==createDTO.Name.ToLower())!=null)
            {
                return BadRequest(createDTO);
            }
            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            Villa model = _mapper.Map<Villa>(createDTO);            
           await _dbVilla.CreateAsync(model);
            return CreatedAtRoute("GetVilla", new {id=model.Id} ,model);
        }
        [HttpDelete("{id}", Name ="DeleteVilla")]
        public async Task<ActionResult> DeleteVilla(int id)
        {

            if (id == 0)
            {
                return BadRequest();
            }
          var villa=await _dbVilla.GetAsync(u=>u.Id == id);
            if (villa==null)
            {
                return NotFound();
            }
          await _dbVilla.RemoveAsync(villa);
            return NoContent();
        }


        [HttpPut]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }
            Villa model = _mapper.Map<Villa>(updateDTO);

         await _dbVilla.UpdateAsync(model);         
            return NoContent();
        }
        [HttpPatch("{id}", Name = "UpdatePartialVilla")]
        //op:replace, add, remove
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked:false);

            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null)
            {
                return BadRequest();
            }
            try
            {
                patchDTO.ApplyTo(villaDTO);
                Villa model=_mapper.Map<Villa>(villaDTO);
               await _dbVilla.UpdateAsync(model);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
