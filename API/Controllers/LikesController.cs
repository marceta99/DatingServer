using API.DTOS;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    //[Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserRepository userRepository ,ILikesRepository likesRepository)
        {
            this._userRepository = userRepository;
            this._likesRepository = likesRepository;
        }

        [HttpPost("{sourceUserName}/{likedUserName}")]
        public async Task<IActionResult> AddLike(string sourceUserName,string likedUserName)
        {
            var sourceUser = await _userRepository.GetUserByUserNameAsync(sourceUserName);
            var likedUser =  await _userRepository.GetUserByUserNameAsync(likedUserName);

            var sourceUserWithLikes = await _likesRepository.GetUserWithLikes(sourceUser.Id); //isit source user samo join sa lajkovima

            if (likedUser == null) return NotFound();
            if (sourceUserName == likedUserName) return BadRequest("Can't like yourself"); //ne moze user da lajkuje sam sebe 

            var userLike = await _likesRepository.GetUserLike(sourceUserWithLikes.Id, likedUser.Id);

            if (userLike != null) return BadRequest("You alredy liked this person");//ako ga nije lajkovo onda je null

            userLike = new UserLike
            {
                SourceUserId = sourceUserWithLikes.Id,
                LikedUserId = likedUser.Id
            };

            sourceUserWithLikes.LikedUsers.Add(userLike);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");
        }
    
        [HttpGet("{sourceUserName}")]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes(string sourceUserName,
            [FromQuery] LikesParams likesParams)
        {
            var sourceUser = await _userRepository.GetUserByUserNameAsync(sourceUserName);
            if (sourceUser == null) return NotFound();

            likesParams.UserId = sourceUser.Id; 

            var users =  await _likesRepository.GetUserLikes(likesParams);

            //ovde dodajemo u response header dodatne informacije na kojoj smo trenutno strani, koliko ima el po strani itd...
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users); 
        } 
    
    
    }
}
