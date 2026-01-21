using Microsoft.EntityFrameworkCore;
using Modules.Users.Application.Contracts;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.ValueObjects;
using Modules.Users.Persistence.Contexts;

namespace Modules.Users.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;

    public UserRepository(UsersDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<User?> GetByIdAsync(UserId id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}