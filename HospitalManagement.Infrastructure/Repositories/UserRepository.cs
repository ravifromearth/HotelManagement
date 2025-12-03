using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

/// <summary>
/// Implementation of User repository
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <inheritdoc />
    public async Task<User?> ValidateLoginAsync(string email, string password)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    }
}