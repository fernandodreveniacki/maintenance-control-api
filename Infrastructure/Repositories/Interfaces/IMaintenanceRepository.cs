using MaintenanceControlSystem.Domain.Entities;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

public interface IMaintenanceRepository : IRepository<Maintenance>
{
    Task<IEnumerable<Maintenance>> GetByMachineIdAsync(int machineId);
    Task<IEnumerable<Maintenance>> GetMaintenancesByCreatorAsync(int userId);

}
