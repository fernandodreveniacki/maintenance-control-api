using MaintenanceControlSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

public interface IAlertRepository : IRepository<Alert>
{
    Task<IEnumerable<Alert>> GetUnreadAsync();
    Task<IEnumerable<Alert>> GetByMachineAsync(int machineId);
    Task<IEnumerable<Alert>> GetAlertsByCreatorAsync(int userId);
    Task<IEnumerable<Alert>> GetUnreadByUserAsync(int userId);
}

