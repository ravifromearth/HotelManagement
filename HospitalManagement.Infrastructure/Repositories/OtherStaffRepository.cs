using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

/// <summary>
/// Implementation of OtherStaff repository
/// </summary>
public class OtherStaffRepository : Repository<OtherStaff>, IOtherStaffRepository
{
    public OtherStaffRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OtherStaff>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Where(s => s.Name.Contains(name))
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OtherStaff>> GetByDesignationAsync(string designation)
    {
        return await _dbSet
            .Where(s => s.Designation == designation)
            .ToListAsync();
    }
}