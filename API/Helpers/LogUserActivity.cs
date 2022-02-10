using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection; 

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        //ovde hocemo da updajtujemo lastActive property kod svakog usera, znaci da vidimo kad je zadnji put bio aktivan 
        //odnosno kad je zadnji put slao neki http request,i to lastActive cemo da updejtujemo svaki put kad posalje http request

        //ovo ce da se izvrsava pre nego sto http request stigne do kontrolera ili nakon sto se zavrsi obrada requsesta
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //ovaj context daje nam pristup kontroleru, httprequestu i jos dosta nekih stvari 
            //ovo context nam daje context pre nego sto je action metoda u controlleru izvrsena 
            //ovo next npr mozemo da korsitimo da odradimo nesto nakon sto je action metoda u controleru izvrsena
            //i mi hocemo da pristupimo contextu nakon sto je action metoda izvrsena

            var resultContext = await next();//ovde imamo pristup contextu nakon sto je akcija u controleru izvrsena nad htpp req

            //ali cemo prvo da proverimo da li je ulogovan koristnik jer nas zanimaju samo ulogovani korisnici koji salju http
            //requestove jer samo njih imamo u bazi, i samo njima mozemo da menjamo lastActive property

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated)//ovo je true ako korisnik ima jwt token 
            {
                return; //ako nije ulogovan onda nista ne radimo 
            }
            var userId = resultContext.HttpContext.User.GetUserId();

            //sad nam treba user repository, i za to korisimo "service locator pattern" 
            var repostory = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            
            var user = await repostory.GetUserByIdAsync(userId);

            user.LastActive = DateTime.Now;  //updatejuemo last active property od usera, tj kad je zadnji put bio aktivan 

            await repostory.SaveAllAsync();

        }
    }
}
