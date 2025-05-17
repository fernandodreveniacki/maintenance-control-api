using MaintenanceControlSystem.Domain.Entities;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

public interface IMachineRepository : IRepository<Machine>
{
    Task<Machine?> GetByCodeAsync(string code);

    Task<IEnumerable<Machine>> GetByStatusAsync();
    Task<IEnumerable<Machine>> GetMachinesByCreatorAsync(int userId);

}
