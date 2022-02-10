using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class UserLike
    {
        //ovo je asocijativna klasa za many to many relationship posto jedan user moze da lajkuje vise usera, 
        //a takodje jedan user moze da bude lajkovan od vise usera
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }
}
