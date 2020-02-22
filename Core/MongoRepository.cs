using MongoDB.Bson;
using MongoDB.Driver;
using System;
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
        public MongoRepository() {}

        /// <summary>
        /// 异步获取表（集合）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="datetime"></param>
        /// <returns></returns>
        private static async Task<IMongoCollection<TEntity>> GetCollectionAsync<TEntity>(string dbName, string tableName = "", bool logRolling = false) where TEntity : class
        {
            var DbContext = Constants.MongoDB.GetDatabase(dbName);
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            // 获取集合名称，如果启用日志滚动切割，则在实体类型名后添加日期
            string collectionName = logRolling ? (tableName + "_" + date) : tableName;

            // 如果集合不存在，那么创建集合
            if (await CollectionExistsAsync<TEntity>(dbName, collectionName) == false)
            {
                await DbContext.CreateCollectionAsync(collectionName);
            }
            return DbContext.GetCollection<TEntity>(collectionName);
        }


        /// <summary>
        /// 集合是否存在
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        private static async Task<bool> CollectionExistsAsync<TEntity>(string dbName, string collectionName)
        {
            var DbContext = Constants.MongoDB.GetDatabase(dbName);
            var filter = new BsonDocument("name", collectionName);
            // 通过集合名称过滤
            var collections = await DbContext.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            // 检查是否存在
            return await collections.AnyAsync();
        }

        /// <summary>
        /// 从指定的库与表中获取指定条件的数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string dbName, string tableName, bool logRolling = false)
        {
            var collection = await GetCollectionAsync<T>(dbName, tableName, logRolling);
            return collection.AsQueryable().Where(predicate).ToList();
        }


        /// <summary>
        /// 对指定的库与表中新增多条数据
        /// </summary>
        /// <returns></returns>
        public async Task Add(List<T> list, string dbName, string tableName = "", bool logRolling = false)
        {
            var collection = await GetCollectionAsync<T>(dbName, tableName, logRolling);
            await collection.InsertManyAsync(list);
        }

        /// <summary>
        /// 对指定的库与表中新增单条数据
        /// </summary>
        /// <returns></returns>
        public async Task Add(T document, string dbName, string tableName = "", bool logRolling = false)
        {
            var collection = await GetCollectionAsync<T>(dbName, tableName, logRolling);
            await collection.InsertOneAsync(document);
        }
    }
}
