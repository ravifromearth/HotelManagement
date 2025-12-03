using HospitalManagement.Domain.Models;

namespace HospitalManagement.Domain.Interfaces;

/// <summary>
/// OtherStaff repository interface
/// </summary>
public interface IOtherStaffRepository : IRepository<OtherStaff>
{
    /// <summary>
    /// Search staff by name
    /// </summary>
    /// <param name="name">Search query for staff name</param>
    /// <returns>List of matching staff members</returns>
    Task<IEnumerable<OtherStaff>> SearchByNameAsync(string name);

    /// <summary>
    /// Get staff members by designation
    /// </summary>
    /// <param name="designation">Staff designation</param>
    /// <returns>List of staff members with the specified designation</returns>
    Task<IEnumerable<OtherStaff>> GetByDesignationAsync(string designation);
}