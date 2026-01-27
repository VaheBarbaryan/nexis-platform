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

    public async Task<bool> EmailExistsAsync(Email email, CancellationToken ct = default)
    {
        return await _context.Users.AnyAsync(x => x.Email.Value == email.Value, ct);
    }

    public async Task<bool> UsernameExistsAsync(Username username, CancellationToken ct = default)
    {
        return await _context.Users.AnyAsync(x => x.Username.Value == username.Value, ct);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
    }
}
