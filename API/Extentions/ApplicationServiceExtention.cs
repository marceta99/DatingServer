using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extentions
{
    public static class ApplicationServiceExtention
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection services, IConfiguration _config )
        {
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly); //ovo assembly govori u kom projektu ce da nadje taj nas 
                                                                        //automapper profile, posto mozemo da imamo vise projekata
                                                                        //u nasem programo. Ovde imamo samo jedan projekat
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(options => {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });

            services.Configure<CloudinarySettings>(_config.GetSection("CloudinarySettings"));//ovde pristupamo onim podesavanjima
                                                                                             //koje smo napisali u appsettings.json fajlu
            services.AddScoped<IPhotoService, PhotoService>();
            
            return services;
        }


    }
}
