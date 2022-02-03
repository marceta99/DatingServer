using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")] //ovaj atribut Table kaze entity frameworku da kad kreira tabelu za fotografjiu da je nazove Photos 
                      //a ne kao po defaulutu isto kao i ime klase Photo
    public class Photo
    {

        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }


    }
}