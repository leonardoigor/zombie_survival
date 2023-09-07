using Server.Entities.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Server.Repositories.Base
{
    public class RepositoryBase<TEntity, TId> : IRepositoryBase<TEntity, TId>
        where TEntity : EntityBase
        where TId : struct
    {
        private readonly DbContext _context;

        public RepositoryBase(DbContext context)
        {
            _context = context;
        }
        public TEntity Add(TEntity entity)
        {
            return _context.Set<TEntity>().Add(entity);
        }

        public IEnumerable<TEntity> AddList(IEnumerable<TEntity> entities)
        {
            return _context.Set<TEntity>().AddRange(entities);
        }

        public TEntity Edit(TEntity entity)
        {
            _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;

            return entity;
        }

        public bool Exist(Func<TEntity, bool> where)
        {
            return _context.Set<TEntity>().Any(where);
        }

        public TEntity GetBy(Func<TEntity, bool> where, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return List(includeProperties).FirstOrDefault(where);
        }

        public TEntity GetById(TId id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            if (includeProperties.Any())
            {
                return List(includeProperties).FirstOrDefault(x => x.Id.ToString() == id.ToString());
            }

            return _context.Set<TEntity>().Find(id);
        }

        public IQueryable<TEntity> List(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

            if (includeProperties.Any())
            {
                return Include(_context.Set<TEntity>(), includeProperties);
            }

            return query;
        }

        public IQueryable<TEntity> ListAndOrderBy<TKey>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> order, bool ascending = true, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return ascending ? ListBy(where, includeProperties).OrderBy(order) : ListBy(where, includeProperties).OrderByDescending(order);
        }

        public IQueryable<TEntity> ListBy(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return List(includeProperties).Where(where);
        }

        public IQueryable<TEntity> ListOrderBy<TKey>(Expression<Func<TEntity, TKey>> order, bool ascending = true, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return ascending ? List(includeProperties).OrderBy(order) : List(includeProperties).OrderByDescending(order);
        }

        public bool Remove(TEntity entity)
        {
            var result = _context.Set<TEntity>().Remove(entity);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public Task<int> SaveChanges()
        {
            return _context.SaveChangesAsync();
        }

        private IQueryable<TEntity> Include(IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }

            return query;
        }

    }
}
