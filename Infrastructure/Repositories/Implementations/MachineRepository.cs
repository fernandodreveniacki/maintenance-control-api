using Microsoft.EntityFrameworkCore;
using MaintenanceControlSystem.Domain.Entities;
using MaintenanceControlSystem.Domain.Enums;
using MaintenanceControlSystem.Infrastructure.Contexts;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Implementations;

public class MachineRepository : GenericRepository<Machine>, IMachineRepository
{
    public MachineRepository(AppDbContext context) : base(context) { }

    public async Task<Machine?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(m => m.Code == code);
    }

    public async Task<IEnumerable<Machine>> GetByStatusAsync()
    {
        return await _dbSet
            .Where(m => m.Status == MachineStatus.Active)
            .ToListAsync();
    }

    public async Task<IEnumerable<Machine>> GetMachinesByCreatorAsync(int creatorId)
    {
        return await _dbSet
            .Where(m => m.CreatedByUserId == creatorId)
            .ToListAsync();
    }

}
