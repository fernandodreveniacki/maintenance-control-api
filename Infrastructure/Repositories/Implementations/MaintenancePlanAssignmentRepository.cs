using Microsoft.EntityFrameworkCore;
using MaintenanceControlSystem.Domain.Entities;
using MaintenanceControlSystem.Infrastructure.Contexts;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Implementations;

public class MaintenancePlanAssignmentRepository(AppDbContext context)
    : GenericRepository<MaintenancePlanAssignment>(context), IMaintenancePlanAssignmentRepository
{
    public async Task<IEnumerable<MaintenancePlanAssignment>> GetByMachineIdAsync(int machineId)
    {
        return await _dbSet
            .Include(p => p.MaintenancePlan)
            .Where(a => a.MachineId == machineId)
            .ToListAsync();
    }
    public async Task<IEnumerable<MaintenancePlanAssignment>> GetAssignmentsByCreatorAsync(int userId)
    {
        return await _dbSet
            .Include(a => a.Machine)
            .Include(a => a.MaintenancePlan)
            .Where(a => a.CreatedByUserId == userId)
            .ToListAsync();
    }

}
