using API.DTOS;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context , IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberAsync(string userName)
        {
            return await _context.Users.Where(x => x.UserName == userName)
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public  async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x =>x.UserName == username); 
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(p=>p.Photos).ToListAsync(); //ovo Include koristimo kada imamo neke relacije kao u
        }                                                                   //ovom slucaju gde imamo relaciju izmedju tabela user
                                                                            //i photos i hocemo da se prikaze photo od nekog usera
                                                                            //i to include me podseca na JOIN , kao kada koristimo
                                                                            //JOIN u SQL upitima 
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0; //ova metoda save, ako se nesto sacuvalo, vraca broj promena pa ako smo  
        }                                                 //nesto promenili u bazi, ova metoda ce da vrati broj promena

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified; //ovde ne menjamo nista konkretno sa bazom , vec samo oznavamo neki
        }                                                       //entry da mu je state modified
    }
}
