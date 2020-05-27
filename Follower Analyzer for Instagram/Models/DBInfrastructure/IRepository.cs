using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models.DBInfrastructure
{
    public interface IRepository
    {
        Task<TEntity> GetAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        Task<IQueryable<TEntity>> GetListAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        Task<IQueryable<TEntity>> GetListAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy) where TEntity : class;
        Task<IQueryable<TEntity>> GetListAsync<TEntity, TKey>(Expression<Func<TEntity, TKey>> orderBy) where TEntity : class;
        Task<IQueryable<TEntity>> GetListAsync<TEntity>() where TEntity : class;
        Task<bool> CreateAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<bool> UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<bool> DeleteAsync<TEntity>(TEntity entity) where TEntity : class;
    }
}