using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            // without using auto mapper
            // return await _context.Users
            //     .Where(x => x.UserName == username)
            //     .Select(user => new MemberDto{
            //         Id = user.Id,
            //         UserName = user.UserName,
            //         KnownAs = user.KnownAs
            //     }).SingleOrDefaultAsync();

            return await _context.Users
                .Where(x => x.UserName == username)
                // pass in ConfigurationProvider so it knows to find out mapping profiles
                // which it gets from service that we added to the application service extensions
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsNoTracking(); // EF is not going to keep track of what we return from this method

            return await PagedList<MemberDto>.CreateAsync(query, userParams.pageNumber, userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(user => user.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            // tells the Entity Framework tracker that something has changed with the entity
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}