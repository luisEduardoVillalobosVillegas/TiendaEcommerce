using Microsoft.EntityFrameworkCore;
using MyStore.Context;
using System.Linq.Expressions;

namespace MyStore.Repositories
{
    public class GenericRepository<TEntity>(AppDbContext _dbContext) where TEntity : class
    {
        //este se replica para que muestre los productos con su categoria
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();

        }
        //esta expresion lamda:Expression<Func<TEntity, object>>[] includes, es un delegate,trabajando con condicionales para
        //la busqueda en el home
        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>[]? conditions = null,
            Expression<Func<TEntity, object>>[]? includes = null
            )
        {
            //este es un select * from categoria
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (conditions is not null)                
                foreach (var condition in conditions) query = query.Where(condition);

            if (includes is not null)
                //esto seria inner join category c on c.categoryId = p.categoryId asi devuelve el producto + categoria
                foreach (var include in includes) query = query.Include(include);

            return await query.ToListAsync();

        }

        ////agregar entidad, le paamos una entidad x
        //public async Task AddAsync(TEntity entity)
        //{
        //    await _dbContext.Set<TEntity>().AddAsync(entity);
        //    await _dbContext.SaveChangesAsync();
        //}

        //agregar entidad, le paamos una entidad x la palabra virtual es para pode ser sobre escrito por la herencia
        //de OrderRepository
        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<TEntity?> GetByIdAsync(int entityId)
        {
            return await _dbContext.Set<TEntity>().FindAsync(entityId);

        }

        public async Task EditAsync(TEntity entity)
        {
            //no es asincrono
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            //no es asincrono
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<TEntity?> GetByFilter(
            Expression<Func<TEntity, bool>>[] conditions
            
            )
        {
            //este es un select * from categoria
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (conditions is not null)
                foreach (var condition in conditions) query = query.Where(condition);



            return await query.FirstOrDefaultAsync();

        }



    }
}
