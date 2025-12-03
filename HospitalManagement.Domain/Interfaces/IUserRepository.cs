using HospitalManagement.Domain.Models;

namespace HospitalManagement.Domain.Interfaces;

/// <summary>
/// User repository interface
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>User entity or null</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Validate user login
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <returns>User entity or null if credentials are invalid</returns>
    Task<User?> ValidateLoginAsync(string email, string password);
}