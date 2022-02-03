using API.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        //kad pravimo neki nas custom Middleware onda pravimo ovako klasu i pravimo konstruktor. U tom konstrutoru saljemo prvo 
        //requestDelegate i to predstavlja ono sto je sledece u middleware pipeline-u . A ovde posto pravimo middleware za greske
        //onda ILogger<> i sa tim hocemo da ispisujemo
        //(logujemo) greske koje se dese. takodje hocemo da proverimo da li smo u developmentu ili u productionu pa koristimo  
        //IHostEnviroment .

        public ExceptionMiddleware(RequestDelegate next , ILogger<ExceptionMiddleware> logger , IHostEnvironment env )
        {
            this._next = next;
            this._logger = logger;
            this._env = env;
        }

        public async Task InvokeAsync(HttpContext context) //kad smo u middleweru mi mozemo da pristupamo http requestu i to radimo
        {                                                 //sa ovim httpContext
                                                          //i posto je ovaj nas custom middleware za handelovanje gresaka onda cemo da stavimo
            try                                           //jedan try catch blok i samo da u prosledjujemo request dalje na sledeci middleware
            {                                             //a ako se desi neka greska e onda je hvaramo u catch bloku 
                await _next(context);                     //i ako zamislimo middleware cevodod kao neko drvo, onda ce exception middleware uvek
            }                                             //da bude na vrhu i onda u bilo kom middlewaru ispod da pukne greska on ce tu gresku
            catch (Exception ex)                          //da salje ka gore sve dok tu gresku ne uhvati neki middleware, i onda ce da greska
            {                                             //dodje do ovog naseg middleware-a i on ce ovde da uhvati tu gresku. 
                _logger.LogError(ex, ex.Message);         //ovde logujemo gresku koja se desila
                context.Response.ContentType = "application/json"; //a ovde vracamo response gde cemo da pisemo da je doslo do greske
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment() ?
                     new ApiExcepton(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())  //ako smo u dev modu
                    :new ApiExcepton(context.Response.StatusCode, "Internal Server Error");             //ako smo u production modu 

                // hocemo da response koji vracamo bubde u json formatu, u camel case-u
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json); 
            }
            





        }











    }
}
