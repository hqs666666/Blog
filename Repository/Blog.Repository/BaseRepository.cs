using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Blog.IRepository;
using Dapper;

namespace Blog.Repository
{
    public class BaseRepository<T, TKey> : IBaseRepository<T, TKey> where T : class
    {
        private readonly IDbConnection _dbConnection;

        public BaseRepository(ConnectionFactory factory)
        {
            _dbConnection = factory.CreateConnection();
        }


        #region Get

        public T Get(string sql, object param)
        {
            return _dbConnection.QueryFirstOrDefault<T>(sql, param);
        }

        public async Task<T> GetAsync(string sql, object param)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        public List<T> GetList(string sql, object param)
        {
            return _dbConnection.Query<T>(sql, param).ToList();
        }

        public async Task<List<T>> GetListAsync(string sql, object param)
        {
            return (await _dbConnection.QueryAsync<T>(sql, param)).ToList();
        }


        #endregion

        #region Update

        public int Update(string sql, object param)
        {
            return _dbConnection.Execute(sql, param);
        }

        public async Task<int> UpdateAsync(string sql, object param)
        {
            return await _dbConnection.ExecuteAsync(sql, param);
        }

        #endregion

        #region Delete

        public int Delete(string sql, object param)
        {
            return _dbConnection.Execute(sql, param);
        }

        public async Task<int> DeleteAsync(string sql, object param)
        {
            return await _dbConnection.ExecuteAsync(sql, param);
        }

        #endregion

        #region Insert

        public int Insert(string sql, object param)
        {
            return _dbConnection.Execute(sql, param);
        }

        public async Task<int> InsertAsync(string sql, object param)
        {
            return await _dbConnection.ExecuteAsync(sql, param);
        }

        #endregion

    }
}
