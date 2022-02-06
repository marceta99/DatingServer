using API.Data;
using API.DTOS;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext dataContext,ITokenService tokenService)
        {
            this._dataContext = dataContext;
            this._tokenService = tokenService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO regsterDTO)
        {
            if (await UserExist(regsterDTO.UserName))
            {
                return BadRequest("Username alredy exist");
            }
            using var hmac = new HMACSHA512();

            var user = new AppUser()
            {
                UserName=  regsterDTO.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regsterDTO.Password)),
                PasswordSalt = hmac.Key
            };
            _dataContext.Add(user);
            await _dataContext.SaveChangesAsync();
            return new UserDTO
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }

        private async Task<bool> UserExist(string userName)
        {
            return await _dataContext.Users.AnyAsync(user => user.UserName == userName.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user =await _dataContext.Users.Include(p =>p.Photos)
                .SingleOrDefaultAsync(user => user.UserName == loginDTO.UserName);

            if(user == null)
            {
                return Unauthorized("inavlid username");
            }

            using var hmac =new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for(int i = 0; i<computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            }
            return new UserDTO
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                MainPhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain == true)?.Url
            };


        }


    }
}
