using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LoggingService.Models;

namespace LoggingService.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task Create(TEntity entity);
        Task CreateAll(IEnumerable<TEntity> entity);

        PagedList<TEntity> FilterBy(Expression<Func<TEntity, bool>> filterExpression, int pageNumber = 1, int pageSize = 15);

        Task DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression);
    }
}
