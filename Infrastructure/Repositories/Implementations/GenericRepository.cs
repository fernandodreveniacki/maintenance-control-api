using Microsoft.EntityFrameworkCore;
using MaintenanceControlSystem.Infrastructure.Contexts;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;

namespace MaintenanceControlSystem.Infrastructure.Repositories.Implementations;

public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task<IEnumerable<T>> GetAllByCreatorAsync(int userId)
    {
        var parameter = typeof(T).GetProperty("GetAllByCreatorAsync");
        if (parameter == null)
            throw new InvalidOperationException($"A entidade {typeof(T).Name} não contém a propriedade GetAllByCreatorAsync.");

        return await _dbSet
            .Where(e => EF.Property<int>(e, "GetAllByCreatorAsync") == userId)
            .ToListAsync();
    }


}
