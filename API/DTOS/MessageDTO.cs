using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOS
{
    public class MessageDTO
    {
        //DTO koji se salje od servera ka klijentu 
        public int Id { get; set; } //svaka poruka ce da ima svoj jedinstveni Id i on je generisan automatski od MSql

        public int SenderId { get; set; }
        public string SenderUserName { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUserName { get; set; }
        public string RecipientPhotoUrl { get; set; }

        public string Content { get; set; }
        public DateTime? DateRead { get; set; } 
        public DateTime MessageSent { get; set; }
    }
}
