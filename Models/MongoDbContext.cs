using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collplex.Core;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Collplex.Models
{
    /// <summary>
    /// MongoDB对象的上下文
    /// </summary>
    public class MongoDbContext
    {
        /// <summary>
        /// Mongo上下文 
        /// </summary>
        public IMongoDatabase DbContext { get; }

        /// <summary>
        /// 初始化MongoDb数据上下文
        /// 将数据库名传递进来
        /// </summary>
        public MongoDbContext(string dbName)
        {
            var mongoClient = new MongoClient(Constants.MongoDBConnectionString);
            // 数据库如果不存在，会自动创建
            DbContext = mongoClient.GetDatabase(dbName);
        }

        /// <summary>
        /// 异步获取表（集合）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public async Task<IMongoCollection<TEntity>> GetCollectionAsync<TEntity>(string tableName = "") where TEntity : class
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            // 获取集合名称，使用的标准是在实体类型名后添加日期
            string collectionName = tableName + "_" + date;

            // 如果集合不存在，那么创建集合
            if (await CollectionExistsAsync<TEntity>(collectionName) == false)
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
        public async Task<bool> CollectionExistsAsync<TEntity>(string name)
        {
            var filter = new BsonDocument("name", name);
            // 通过集合名称过滤
            var collections = await DbContext.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            // 检查是否存在
            return await collections.AnyAsync();
        }
    }
}