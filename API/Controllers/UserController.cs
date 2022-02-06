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
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    //[Authorize] //kad stavimo authorize iznad controllera onda je to isto kao da smo na sve metode u controleru stavili authorize
    public class UserController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UserController(IUserRepository userRepository,IMapper mapper, IPhotoService photoService)
        {
            this._userRepository = userRepository;
            this._mapper = mapper;
            this._photoService = photoService;
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

        [HttpPut("{userName}")]
        public async Task<ActionResult> UpdateUser(string userName,[FromBody] MemberUpdateDTO memberUpdateDTO)
        {
            //var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //ovim uzimamo userName iz jwt tokena koji se salje,
                                                                             //i to je user kojeg cemo da updatejutjemo 

            var user = await _userRepository.GetUserByUserNameAsync(userName);

            _mapper.Map(memberUpdateDTO, user);
            //_userRepository.Update(user); 

            if(await _userRepository.SaveAllAsync()) //ako se sve normalno sacuvalo u bazi onda za put request ne vracamo nista posebno nazad klijentskoj strani
            {
                return NoContent();
            }
            return BadRequest("Failed to update user"); 

        }

        [HttpPost("add-photo/{userName}")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(string userName,IFormFile file)
        {
            var user = await _userRepository.GetUserByUserNameAsync(userName);

            var result =  await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if(user.Photos.Count == 0)
            {
                photo.IsMain = true;   //ako user nije imao nijednu sliku do sada, onda mu ovo postaje glavna profilna slika
            }

            user.Photos.Add(photo); 

            if(await _userRepository.SaveAllAsync())
            {
                return _mapper.Map<PhotoDTO>(photo);
               
            }

            return BadRequest("Problem adding photos");
        }

        [HttpPut("set-main-photo/{userName}/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(string userName,int photoId)
        {
            //u ovoj metodi samo postavljamo neku fotografju da je glavna odnosno isMain = true, a ona fotografija koja je do
            //sada bila glavna , e njoj sada stavljamo isMain na false . 

            var user = await _userRepository.GetUserByUserNameAsync(userName);

            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);

            if (photo.IsMain) return BadRequest("This is alredy your main photo"); 

            var currentMainPhoto  = user.Photos.FirstOrDefault(photo => photo.IsMain == true);

            if(currentMainPhoto!= null)
            {
                currentMainPhoto.IsMain = false; 
            }
            photo.IsMain = true; 

            if(await _userRepository.SaveAllAsync())
            {
                return NoContent(); 
            }
            return BadRequest("Something went from with setting main photo");
        }

        [HttpDelete("delete-photo/{userName}/{photoId}")]
        public async Task<ActionResult> DeletePhoto(string userName,int photoId)
        {
            var user = await _userRepository.GetUserByUserNameAsync(userName);

            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);

            if (photo == null) return NotFound();
            
            if (photo.IsMain == true) return BadRequest("You can't delete your main photo");

            if(photo.PublicId != null) //samo fotografije sa cloudinaryiija imaju publicId dok ostale nemaju taj publicID
            {
                var result =  await _photoService.DeletePhotoAsync(photo.PublicId);//ovde brisemo photografiju sa cloudinaryija
                if(result.Error != null)
                {
                    return BadRequest(result.Error.Message); 
                }
            }
            //a u svakom slucaju i ako je photografija sa clodinaryija i ako je obicna, hocemo da obrisemo njen url iz baze :
            user.Photos.Remove(photo);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem with deleting photo"); 

        }

    }
}
