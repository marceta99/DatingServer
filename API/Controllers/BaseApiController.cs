using API.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))] //ovo je ono za update kada je user poslednji put bio aktivan, i dodao sam ovo 
    //ovde da bi svi ostali kontroleri imali ovaj atribut, tj da ne moram svaki put to da dodajem. To stavljamo na kontrolere
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController: ControllerBase
    {



    }
}
