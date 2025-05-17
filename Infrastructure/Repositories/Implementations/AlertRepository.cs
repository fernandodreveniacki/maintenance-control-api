using Microsoft.EntityFrameworkCore;
using MaintenanceControlSystem.Domain.Entities;
using MaintenanceControlSystem.Infrastructure.Contexts;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Implementations;

public class AlertRepository(AppDbContext context)
    : GenericRepository<Alert>(context), IAlertRepository
{
    public async Task<IEnumerable<Alert>> GetUnreadAsync()
    {
        return await _dbSet
            .Include(a => a.Assignment)
            .ThenInclude(p => p.Machine)
            .Where(a => !a.IsRead)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetByMachineAsync(int machineId)
    {
        return await _dbSet
            .Include(a => a.Assignment)
            .Where(a => a.MachineId == machineId)
            .ToListAsync();
    }


    public async Task<IEnumerable<Alert>> GetAlertsByCreatorAsync(int userId)
    {
        return await _dbSet
            .Include(a => a.Assignment)
            .ThenInclude(p => p.Machine)
            .Where(a => a.CreatedByUserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetUnreadByUserAsync(int userId)
    {
        return await _dbSet
            .Include(a => a.Assignment)
            .ThenInclude(p => p.Machine)
            .Where(a => !a.IsRead && a.CreatedByUserId == userId)
            .ToListAsync();
    }


}
