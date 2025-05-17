using MaintenanceControlSystem.Domain.Entities;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
