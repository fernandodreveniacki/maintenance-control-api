using Microsoft.EntityFrameworkCore;
using MaintenanceControlSystem.Domain.Entities;
using MaintenanceControlSystem.Infrastructure.Contexts;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Implementations;

public class UserRepository(AppDbContext context)
    : GenericRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}
