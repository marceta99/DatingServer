using API.DTOS;
using API.Entities;
using API.Helpers;
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

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            //entitiy framework automatski dodaje neki tracking(praćenje) svaki put, medjutim to nam treba samo kada nesto
            //menjamo u bazi, a posto mi ovde hocemo samo da citamo podatke iz baze, necemo nista da menjamo sa bazom
            //onda mozemo da iskljucimo taj tracking sa ovim AsNoTracking(). 
            var query = _context.Users.AsQueryable(); 

            query = query.Where(u=> u.UserName != userParams.CurrentUserName);//necemo da se medju matchevima trenutnog usera
                                                                               //prikazuje i njegov profil 
            query = query.Where(u => u.Gender == userParams.Gender); //u userControlleru smo stavili suprotan pol od trenutnog 
                                                                     //korisnik tako da ako je musko hocemo da prikazemo zene
            
            var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1); //ovo ce nam koristiti da filtriramo korisnike
            var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge); //po min i max godinama koje imaju

            query = query.Where(u => u.DateOfBirth >= minDateOfBirth && u.DateOfBirth <= maxDateOfBirth);

            switch (userParams.OrderBy) //sortianje 
            {
                case "created":
                    query = query.OrderByDescending(u => u.Created); 
                    break;
                default:
                    query = query.OrderByDescending(u => u.LastActive); 
                    break;       
            }

            var filteredQuery = query.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).AsNoTracking();


            return await PagedList<MemberDTO>.CreateAsync(filteredQuery, userParams.PageNumber, userParams.PageSize);
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
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; //ova metoda save, ako se nesto sacuvalo, vraca broj promena pa ako smo  
        }                                                 //nesto promenili u bazi, ova metoda ce da vrati broj promena

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified; //ovde ne menjamo nista konkretno sa bazom , vec samo oznavamo neki
        }                                                       //entry da mu je state modified
    }
}
