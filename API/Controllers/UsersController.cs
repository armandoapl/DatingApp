using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _usersRepository;

        private readonly IMapper _mapper;

        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService )
        {
            _usersRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]   
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var user = await _usersRepository.GetMemberAsync(User.GetUserName());
            userParams.CurrentUserName = user.UserName;


            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }

            var users = await _usersRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }


        [HttpGet("{userName}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string userName)
        {
            return Ok(await _usersRepository.GetMemberAsync(userName));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _usersRepository.GetUserByUserNameAsync(User.GetUserName());

            _mapper.Map(memberUpdateDto, user);

            _usersRepository.Update(user);

            if (await _usersRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
            
        }


        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _usersRepository.GetUserByUserNameAsync(User.GetUserName());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
 
            user.Photos.Add(photo);

            if(await _usersRepository.SaveAllAsync())
            {
                //return CreatedAtRoute("GetUser", _mapper.Map<Photo, PhotoDto>(photo));

                return CreatedAtRoute("GetUser", new { userName = User.GetUserName() }, _mapper.Map<Photo, PhotoDto>(photo));
             
            }


                return BadRequest("Problem adding Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _usersRepository.GetUserByUserNameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x=> x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if (currentMain != null) currentMain.IsMain = false;

            photo.IsMain = true;

            if (await _usersRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");  

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DetelePhoto(int photoId)
        {
            var user = await _usersRepository.GetUserByUserNameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _usersRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the photos");

        }


    }
}
