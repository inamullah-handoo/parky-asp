using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dto;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/trails")]
    //[Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecT")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TrailsController : Controller
    {
        private readonly ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of trails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TrailDto>))]
        public IActionResult GetTrails()
        {
            var objList = _trailRepo.GetTrails();
            var objDto = new List<TrailDto>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }

            return Ok(objDto);
        }

        /// <summary>
        /// Get individual trail
        /// </summary>
        /// <param name="trailId">The ID of the trail</param>
        /// <returns></returns>
        [HttpGet("{trailId:int}", Name = "GetTrail")]
        public IActionResult GetTrail(int trailId)
        {
            var obj = _trailRepo.GetTrail(trailId);
            if (obj == null)
            {
                return NotFound();
            }
            var objDto = _mapper.Map<TrailDto>(obj);
            return Ok(objDto);
        }

        /// <summary>
        /// Get trails in national park
        /// </summary>
        /// <param name="npId">The ID of the national park</param>
        /// <returns></returns>
        [HttpGet("[action]/{npId:int}", Name = "GetTrailsInNP")]
        //[HttpGet("getTrailInNP/{npId:int}", Name = "GetTrailsInNP")]
        public IActionResult GetTrailsInNP(int npId)
        {
            var objList = _trailRepo.GetTrailsInNationalPark(npId);
            if (objList == null)
            {
                return NotFound();
            }
            var objDto = new List<TrailDto>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }
            return Ok(objDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type= typeof(TrailUpsertDto))]
        public IActionResult CreateTrail([FromBody]TrailUpsertDto trailDto)
        {
            if(trailDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_trailRepo.TrailExist(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(404, ModelState);
            }
            var obj = _mapper.Map<Trail>(trailDto);
            if (!_trailRepo.CreateTrail(obj))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetTrail", new { trailId = obj.Id }, obj);
        }

        [HttpPatch("{id:int}", Name = "UpdateTrail")]
        [ProducesResponseType(204)]
        public IActionResult UpdateTrail(int id, [FromBody] TrailUpsertDto trailDto)
        {
            if (trailDto == null || id != trailDto.Id)
            {
                return BadRequest(ModelState);
            }
            if (_trailRepo.TrailExist(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(404, ModelState);
            }
            var obj = _mapper.Map<Trail>(trailDto);
            if (!_trailRepo.UpdateTrail(obj))
            {
                ModelState.AddModelError("", "Something went wrong while updating!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteTrail")]
        public IActionResult DeleteTrail(int id)
        {
            if (!_trailRepo.TrailExist(id))
            {
                return NotFound();
            }
            var obj = _trailRepo.GetTrail(id);
            if (!_trailRepo.DeleteTrail(obj))
            {
                ModelState.AddModelError("", "Something went wrong while deleting!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
