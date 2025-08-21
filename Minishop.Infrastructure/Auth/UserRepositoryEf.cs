using Microsoft.EntityFrameworkCore;
using Minishop.Application.Auth.Interfaces;
using Minishop.Core.Entities;
using Minishop.Infrastructure.Persistence;

namespace Minishop.Infrastructure.Auth;

public sealed class UserRepositoryEf : IUserRepository
{
    private readonly ApplicationDbContext _db;
    public UserRepositoryEf(ApplicationDbContext db) => _db = db;

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => await _db.Users.AnyAsync(u => u.Email.ToUpper() == email.ToUpper(), ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _db.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == email.ToUpper(), ct);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct)!;

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _db.Users.AddAsync(user, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}