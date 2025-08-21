﻿using Minishop.Core.Entities;

namespace Minishop.Application.Auth.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}