using API.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUser
    {

        public int Id { get; set; }
        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Intorduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo>  Photos { get; set; }

        public ICollection<UserLike> LikedByUsers { get; set; } //lista usera koji su lajkovali ovog usera
        public ICollection<UserLike> LikedUsers { get; set; } //lista usera koje je ovaj user lajkovao 

    }
}
