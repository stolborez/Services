using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LoggingService.Models;

namespace LoggingService.Repositories
{
    public class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IMongoCollection<TEntity> _collection;

        public MongoRepository(string collectionName)
        {
            var mongodbHostname = Environment.GetEnvironmentVariable("MONGODB_HOSTNAME");
            if (string.IsNullOrEmpty(mongodbHostname)) throw new ArgumentNullException(nameof(mongodbHostname));

            var mongodbPort = Environment.GetEnvironmentVariable("MONGODB_PORT");
            if (string.IsNullOrEmpty(mongodbPort)) throw new ArgumentNullException(nameof(mongodbPort));

            var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASENAME");
            if (string.IsNullOrEmpty(databaseName)) throw new ArgumentNullException(nameof(databaseName));

            var username = Environment.GetEnvironmentVariable("MONGODB_USERNAME");
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

            var password = Environment.GetEnvironmentVariable("MONGODB_PASSWORD");
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            var database = new MongoClient($"mongodb://{username}:{password}@{mongodbHostname}:{mongodbPort}").GetDatabase(databaseName);
            //_collection = database.GetCollection<TEntity>(typeof(TEntity).Name);
            _collection = database.GetCollection<TEntity>(collectionName);
        }

        public virtual IQueryable<TEntity> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public virtual Task Create(TEntity entity)
        {
            return Task.Run(() => _collection.InsertOneAsync(entity));
        }

        public virtual Task CreateAll(IEnumerable<TEntity> entities)
        {
            return Task.Run(() => _collection.InsertManyAsync(entities));
        }

        public virtual PagedList<TEntity> FilterBy(Expression<Func<TEntity, bool>> filterExpression, int pageNumber = 1, int pageSize = 15)
        {
            var count = _collection
                .Find(filterExpression)
                .CountDocuments();
            var list = _collection
                .Find(filterExpression)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            return new PagedList<TEntity>(list, unchecked((int)count), pageNumber, pageSize);
        }

        public Task DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return Task.Run(() => _collection.DeleteManyAsync(filterExpression));
        }
    }
}
