using Persistence.Client;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private const string UsersTable = "accounts";
        private readonly ISqlClient _sqlClient;

        public AccountRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }
        public async Task<int> SaveOrUpdateAsync(AccountWriteModel model)
        {
            var sqlInsert = @$"INSERT INTO {UsersTable} (iban, userid, balance) VALUES(@iban, @userid, @balance)";

            var rowsAffected = _sqlClient.ExecuteAsync(sqlInsert, new
            {
                iban = model.Iban,
                userid = model.UserId,
                balance = model.Balance
            });

            return await rowsAffected;
        }
    }
}
