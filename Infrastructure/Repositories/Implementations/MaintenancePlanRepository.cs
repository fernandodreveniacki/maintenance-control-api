using Microsoft.EntityFrameworkCore;
using MaintenanceControlSystem.Domain.Entities;
using MaintenanceControlSystem.Infrastructure.Contexts;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Implementations;

public class MaintenancePlanRepository(AppDbContext context)
    : GenericRepository<MaintenancePlan>(context), IMaintenancePlanRepository
{
    public async Task<IEnumerable<MaintenancePlan>> GetActivePlansAsync()
    {
        return await _dbSet.Where(p => p.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<MaintenancePlan>> GetPlansByCreatorAsync(int userId)
    {
        return await _dbSet
            .Where(p => p.CreatedByUserId == userId)
            .ToListAsync();
    }

}
