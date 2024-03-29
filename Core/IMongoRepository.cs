﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Collplex.Core
{
    public interface IMongoRepository<T> where T : class
    {
        /// <summary>
        /// 从指定的库与表中获取指定条件的数据
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string dbName, string tableName = "", bool logRolling = false);

        /// <summary>
        /// 对指定的库与表中新增数据
        /// </summary>
        /// <returns></returns>
        Task Add(List<T> list, string dbName, string tableName = "", bool logRolling = false);
        Task Add(T document, string dbName, string tableName = "", bool logRolling = false);
    }
}
