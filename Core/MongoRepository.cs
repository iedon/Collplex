using Collplex.Models;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Collplex.Core
{
    /// <summary>
    /// 封装向业务层提供操作MongoDB的操作方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        private readonly ConcurrentDictionary<string, MongoDbContext> contextDictionary;

        public MongoRepository()
        {
            contextDictionary = new ConcurrentDictionary<string, MongoDbContext>();
        }

        private MongoDbContext GetDbContext(string dbName)
        {
            return contextDictionary.GetOrAdd(dbName, key => new MongoDbContext(key));
        }

        /// <summary>
        /// 从指定的库与表中获取指定条件的数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string dbName, string tableName)
        {
            var dbContext = GetDbContext(dbName);
            var collection = await dbContext.GetCollectionAsync<T>(tableName);
            return collection.AsQueryable().Where(predicate).ToList();
        }


        /// <summary>
        /// 对指定的库与表中新增多条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Add(List<T> list, string dbName, string tableName = "")
        {
            var dbContext = GetDbContext(dbName);
            var collection = await dbContext.GetCollectionAsync<T>(tableName);
            await collection.InsertManyAsync(list);
            return true;
        }

        /// <summary>
        /// 对指定的库与表中新增单条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Add(T document, string dbName, string tableName = "")
        {
            var dbContext = GetDbContext(dbName);
            var collection = await dbContext.GetCollectionAsync<T>(tableName);
            await collection.InsertOneAsync(document);
            return true;
        }
    }
}
