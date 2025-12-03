using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

/// <summary>
/// Implementation of Department repository
/// </summary>
public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Department?> GetByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.DeptName == name);
    }

    /// <summary>
    /// Get all departments with doctors
    /// </summary>
    /// <returns>List of departments with doctors</returns>
    public async Task<IEnumerable<Department>> GetAllWithDoctorsAsync()
    {
        return await _dbSet
            .Include(d => d.Doctors)
            .ToListAsync();
    }
}