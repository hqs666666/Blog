using Blog.Common;
using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Options;

namespace Blog.Repository
{
    public class ConnectionFactory
    {
        private readonly DbOption _dbOption;
        private IDbConnection _connection = null;

        public ConnectionFactory(IOptions<DbOption> dbOption)
        {
            _dbOption = dbOption.Value;
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns>数据库连接</returns>
        public IDbConnection CreateConnection()
        {
            if (string.IsNullOrWhiteSpace(_dbOption.ConnectionString))
                throw new ArgumentNullException(nameof(_dbOption.ConnectionString));

            switch (_dbOption.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    _connection = new SqlConnection(_dbOption.ConnectionString);
                    break;
                case DatabaseType.MySql:
                    _connection = new MySqlConnection(_dbOption.ConnectionString);
                    break;
                default:
                    throw new ArgumentNullException($"don't support {_dbOption.DatabaseType.ToString()}");
            }

            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            return _connection;
        }
    }
}
