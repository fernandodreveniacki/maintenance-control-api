using MaintenanceControlSystem.Domain.Entities;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

public interface IMaintenancePlanAssignmentRepository : IRepository<MaintenancePlanAssignment>
{
    Task<IEnumerable<MaintenancePlanAssignment>> GetByMachineIdAsync(int machineId);
    Task<IEnumerable<MaintenancePlanAssignment>> GetAssignmentsByCreatorAsync(int userId);

}
