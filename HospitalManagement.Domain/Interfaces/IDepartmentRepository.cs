using HospitalManagement.Domain.Models;

namespace HospitalManagement.Domain.Interfaces;

/// <summary>
/// Department repository interface
/// </summary>
public interface IDepartmentRepository : IRepository<Department>
{
    /// <summary>
    /// Get department by name
    /// </summary>
    /// <param name="name">Department name</param>
    /// <returns>Department entity</returns>
    Task<Department?> GetByNameAsync(string name);

    /// <summary>
    /// Get all departments with doctors
    /// </summary>
    /// <returns>List of departments with doctors</returns>
    Task<IEnumerable<Department>> GetAllWithDoctorsAsync();
}