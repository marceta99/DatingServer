using API.DTOS;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId); 
        }

        //u predicate navodimo da li hocemo sve usere koje je
        //taj user lajkovao ili hocemo sve usere koji su lajkovali tog usera
        //znaci u jednom slucaju ce ovaj userId da bude sourceID u like tabeli,a u drugom slucaju ce da bude LikedUserID u like tabeli
        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var usersQuery = _context.Users.OrderBy(user => user.UserName).AsQueryable();

            var likesQuery = _context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked") //sve one koje je taj user lajkovao
            {
                likesQuery = likesQuery.Where(like => like.SourceUserId == likesParams.UserId);
                usersQuery = likesQuery.Select(like => like.LikedUser); //ovo nam vraca lajkovane usere iz like tabele , a zbog onog
                                                                        //where iznad to ce da budu samo oni useri koji su lajkovani
                                                                        //od strane usera sa id-jem userID
            }
            if(likesParams.Predicate == "likedBy") //sve one usere koji su lajkovali tog usera
            {
                likesQuery = likesQuery.Where(like => like.LikedUserId == likesParams.UserId);
                usersQuery = likesQuery.Select(like => like.SourceUser);
            }

            //ovde sad nismo koristili auto mappera vec smo rucno mapovali u LikeDTO , ali naravno moze i sa auto mapperom 
            var likedUsers = usersQuery.Select(user => new LikeDTO 
            {
               UserName = user.UserName , 
               KnownAs = user.KnownAs ,
               Age = user.DateOfBirth.CalculateAge(),
               PhotoUrl = user.Photos.FirstOrDefault(p =>p.IsMain).Url,
               Id = user.Id

            });

            return await PagedList<LikeDTO>.CreateAsync(likedUsers,likesParams.PageNumber, likesParams.PageSize);
        
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            //ovde vracamo usera sa tim id-jem i litu usera koje je ovaj user lajkovao 

            return await _context.Users.
                Include(x => x.LikedUsers).
                FirstOrDefaultAsync(user => user.Id == userId);

        }
    }
}
