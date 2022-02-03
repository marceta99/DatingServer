using API.Data;
using API.DTOS;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    //[Authorize] //kad stavimo authorize iznad controllera onda je to isto kao da smo na sve metode u controleru stavili authorize
    public class UserController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository,IMapper mapper)
        {
            this._userRepository = userRepository;
            this._mapper = mapper;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            /*var users = await _userRepository.GetUsersAsync();
            var usersToReturn = _mapper.Map<IEnumerable<MemberDTO>>(users); //mapiramo usere tipa AppUser u MemberDTO

            return Ok(usersToReturn); */

            //i ovde je moglo ovo gore ali ovo dole je malo bolja praksa da se mappovanje radi direktno u repository a ne ovde 
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
        }
        
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string userName)
        {
            /*var user = await _userRepository.GetUserByUserNameAsync(userName);
            var userToReturn = _mapper.Map<MemberDTO>(user);
            return userToReturn; */

            //moglo je i ovo gore ali je ovo dole bolje praksa da se mappuje odmah u repositoryiju a ne u controlleru
            return await _userRepository.GetMemberAsync(userName);

        }


    }
}
