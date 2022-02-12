using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOS
{
    public class CreateMessageDTO
    {
        //DTO koji se salje od klijenta ka serveru 
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
    }
}
