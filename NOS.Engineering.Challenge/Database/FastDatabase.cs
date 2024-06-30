using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.Context;

namespace NOS.Engineering.Challenge.Database;

public class FastDatabase<TOut, TIn> : IDatabase<TOut, TIn> where TOut : class
{
    private readonly EFSQLServerContext _db;
    private readonly IMapper<TOut?, TIn> _mapper;

    public FastDatabase(EFSQLServerContext db, IMapper<TOut?, TIn> mapper)
    {
        _mapper = mapper;
        _db = db;

        _db.Database.EnsureCreated();
    }

    public async Task<TOut?> Create(TIn item)
    {
        var id = Guid.NewGuid();
        var entity = _mapper.Map(id, item);

        if (entity is not null)
        {
            await _db.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        return entity;
    }

    public async Task<Guid> Delete(Guid id)
    {
        var entity = await Read(id);

        if (entity is null)
            return Guid.Empty;

        _db.Remove(entity);
        await _db.SaveChangesAsync();

        return id;
    }

    public async Task<TOut?> Read(Guid id)
    {
        return await _db.FindAsync<TOut>(id);
    }

    public async Task<IEnumerable<TOut?>> ReadAll()
    {
        return await _db.Set<TOut>().ToListAsync();
    }

    public async Task<TOut?> Update(Guid id, TIn item)
    {
        var entity = await Read(id);

        if (entity is null)
            return default;

        var entityUpdated = _mapper.Patch(entity, item);

        if (entityUpdated is not null)
        {
            _db.Entry(entity).CurrentValues.SetValues(entityUpdated);

            await _db.SaveChangesAsync();
        }

        return entityUpdated;

    }
}
