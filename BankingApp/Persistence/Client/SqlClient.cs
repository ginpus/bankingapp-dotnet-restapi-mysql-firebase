using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Client
{
    public class SqlClient : ISqlClient
    {
        private readonly string _connectionString;

        public SqlClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> ExecuteAsync(string sql, object param = null)
        {
            using var connection = new MySqlConnection(_connectionString);

            var rowsAffected = await connection.ExecuteAsync(sql, param);
            if (rowsAffected < 1)
            {
                throw new Exception("No rows affected");
            }

            return rowsAffected;
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            return connection.QueryAsync<T>(sql, param);
        }

        public IEnumerable<T> Query<T>(string sql, object param = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            return connection.Query<T>(sql, param);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            return connection.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            return connection.QuerySingleOrDefaultAsync<T>(sql, param);
        }
    }
}
