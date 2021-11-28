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
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/nationalParks")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of national parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NationalParkDto>))]
        public IActionResult GetNationalParks()
        {
            var objList = _npRepo.GetNationalParks();
            var objDto = new List<NationalParkDto>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<NationalParkDto>(obj));
            }

            return Ok(objDto);
        }

        /// <summary>
        /// Get individual national park
        /// </summary>
        /// <param name="nationalParkId">The ID of the national park</param>
        /// <returns></returns>
        [HttpGet("{nationalParkId:int}", Name = "GetNationalPark")]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = _npRepo.GetNationalPark(nationalParkId);
            if (obj == null)
            {
                return NotFound();
            }
            var objDto = _mapper.Map<NationalParkDto>(obj);
            return Ok(objDto);
        }

        [HttpPost]

        public IActionResult CreateNationalPark([FromBody]NationalParkDto nationalParkDto)
        {
            if(nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_npRepo.NationalParkExist(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }
            var obj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepo.CreateNationalPark(obj))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetNationalPark", new { version=HttpContext.GetRequestedApiVersion().ToString(), nationalParkId = obj.Id }, obj);
        }

        [HttpPatch("{id:int}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int id, [FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null || id != nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }
            if (_npRepo.NationalParkExist(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }
            var obj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepo.UpdateNationalPark(obj))
            {
                ModelState.AddModelError("", "Something went wrong while updating!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteNationalPark")]
        public IActionResult DeleteNationalPark(int id)
        {
            if (!_npRepo.NationalParkExist(id))
            {
                return NotFound();
            }
            var obj = _npRepo.GetNationalPark(id);
            if (!_npRepo.DeleteNationalPark(obj))
            {
                ModelState.AddModelError("", "Something went wrong while deleting!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
