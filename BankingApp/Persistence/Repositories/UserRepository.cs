using Persistence.Client;
using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string UsersTable = "users";
        private readonly ISqlClient _sqlClient;

        public UserRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public async Task<int> CreateUserAysnc(UserWriteModel user)
        {
            var sqlInsert = @$"INSERT INTO {UsersTable} (userid, email, localid, datecreated) VALUES(@userid, @email, @localid, @datecreated)";

            var rowsAffected = _sqlClient.ExecuteAsync(sqlInsert, new
            {
                userid = user.UserId,
                email = user.Email,
                localid = user.LocalId,
                datecreated = user.DateCreated
            });

            return await rowsAffected;
        }

        public async Task<IEnumerable<UserReadModel>> GetAllUsersAsync()
        {
            var sqlSelect = $"SELECT userid, email, localid, datecreated FROM {UsersTable}";

            var users = await _sqlClient.QueryAsync<UserReadModel>(sqlSelect);

            return users;
        }

        public async Task<UserReadModel> GetUserAsync(string localId)
        {
            var sqlSelect = $"SELECT userid, email, localid, datecreated FROM {UsersTable} where localid = @localid";

            var user = await _sqlClient.QuerySingleOrDefaultAsync<UserReadModel>(sqlSelect, new
            {
                localid = localId
            });

            return user;
        }

        public async Task<int> EditEmailAsync(Guid userId, string newEmail)
        {
            var sql = @$"UPDATE {UsersTable} SET email = @email WHERE userid = @userid";

            var rowsAffected = _sqlClient.ExecuteAsync(sql, new
            {
                email = newEmail,
                userid = userId
            });

            return await rowsAffected;
        }
    }
}
