using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; } //svaka poruka ce da ima svoj jedinstveni Id i on je generisan automatski od MSql

        public int SenderId { get; set; }
        public string SenderUserName { get; set; }
        public AppUser Sender { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUserName { get; set; }
        public AppUser Recipient { get; set; }

        public string Content { get; set; }
        public DateTime? DateRead { get; set; } //hocemo da ovaj prop ne bude obavezan,tj da bude null ako poruka nije procitana 
                                                //i zbog ovog ? ce entitiy framework da u bazi kod ovog stiklira nullable
        public DateTime MessageSent { get; set; } = DateTime.Now; //cim se kreira objekat poruka, stavlja se trenutni datum ovde
        
        public bool SenderDeleted { get; set; }//ici cemo logikom da ako user koji je poslao poruku odluci da je obrise onda ce
                                             //ta poruka nece vise da se prikazuje na UI tog usera,ali nece da se obrise i u bazi
        public bool RecipientDeleted { get; set; } //jedini slucaj kada ce ta poruka da se obrise i u bazi je kada su user koji
                                                   //je poslao tu poruku i user koji je primio tu poruku , obrisali tu poruku
    }
}
