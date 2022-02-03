using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOS
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        [StringLength(15,MinimumLength = 2)]
        [Required]
        public string Password { get; set; }
    }
}
