using Microsoft.EntityFrameworkCore;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly GameDbContext _context;

        public UserRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user) // Přidáno
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

    
        public async Task<User?> GetAiUserByTypeAsync(string typeName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.IsAi && u.TypeName == typeName);
        }
    
    }
}