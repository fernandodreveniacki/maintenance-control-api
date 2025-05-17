using Microsoft.EntityFrameworkCore;
using MaintenanceControlSystem.Domain.Entities;
using MaintenanceControlSystem.Infrastructure.Contexts;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Implementations;

public class MaintenanceRepository(AppDbContext context)
    : GenericRepository<Maintenance>(context), IMaintenanceRepository
{
    public async Task<IEnumerable<Maintenance>> GetByMachineIdAsync(int machineId)
    {
        return await _dbSet
            .Include(m => m.Machine)
            .Where(m => m.MachineId == machineId)
            .ToListAsync();
    }
    public async Task<IEnumerable<Maintenance>> GetMaintenancesByCreatorAsync(int userId)
    {
        return await _dbSet
            .Include(m => m.Machine)
            .Where(m => m.CreatedByUserId == userId)
            .ToListAsync();
    }

}
