using API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }

        //posto sa tabelom userLike uvodimo many to many realtionship onda za to moraju da imaju neka dodatna podesavanja u 
        //entity frameworku i zato overridujemo ovu onModelCrating metodu
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //ova tabela UserLike imace se slozeni primarni kljuc i to moramo da podesimo ovde ispod. Znci slozeni primarni kljuc
            //te tabele ce da se sastoji od id-ija usera koji je lajkovao (source), i usera koga je lajkovao
            builder.Entity<UserLike>().HasKey(key => new { key.SourceUserId, key.LikedUserId });

            //onda moramo da podesimo i relacije izmedju tabela
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)       //one user can like many users
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserLike>()
               .HasOne(s => s.LikedUser)       //one user can be liked by many users
               .WithMany(l => l.LikedByUsers)
               .HasForeignKey(s => s.LikedUserId)
               .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
