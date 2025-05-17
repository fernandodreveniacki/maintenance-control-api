using MaintenanceControlSystem.Domain.Entities;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

public interface IMaintenancePlanRepository : IRepository<MaintenancePlan>
{
    Task<IEnumerable<MaintenancePlan>> GetActivePlansAsync();
    Task<IEnumerable<MaintenancePlan>> GetPlansByCreatorAsync(int userId);

}
