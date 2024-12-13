using APW2.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APW2.Data.Repositorio
{
    public abstract class RepositoryBase<TEntity> where TEntity : class
    {
        protected readonly ProcessdbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected RepositoryBase(ProcessdbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual TEntity? GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Add(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public virtual void Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null) throw new KeyNotFoundException($"Entity with ID {id} not found.");
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }
    }
}