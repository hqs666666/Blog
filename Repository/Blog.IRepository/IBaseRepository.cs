
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.IRepository
{
    public interface IBaseRepository<T, TKey> where T : class
    {
        #region Get

        T Get(string sql, object param);
        Task<T> GetAsync(string sql, object param);
        List<T> GetList(string sql, object param);
        Task<List<T>> GetListAsync(string sql, object param);

        #endregion

        #region Update

        int Update(string sql, object param);
        Task<int> UpdateAsync(string sql, object param);

        #endregion

        #region Delete

        int Delete(string sql, object param);
        Task<int> DeleteAsync(string sql, object param);

        #endregion

        #region Insert

        int Insert(string sql, object param);
        Task<int> InsertAsync(string sql, object param);

        #endregion
    }
}
