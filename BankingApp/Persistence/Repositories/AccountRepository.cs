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
        private const string AccountTable = "accounts";
        private readonly ISqlClient _sqlClient;

        public AccountRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public async Task<bool> CheckAccountByUserAsync(string accountId, Guid userId)
        {
            var sql = @$"SELECT EXISTS(SELECT * FROM {AccountTable} WHERE iban = @iban AND userid = @userid)";

            var exists = await _sqlClient.QueryFirstOrDefaultAsync<bool>(sql, new
            {
                iban = accountId,
                userid = userId
            });

            return exists;
        }

        public async Task<int> SaveOrUpdateAsync(AccountWriteModel model)
        {
            var sqlInsert = @$"INSERT INTO {AccountTable} (iban, userid, balance) VALUES(@iban, @userid, @balance) ON DUPLICATE KEY UPDATE balance = @balance";

            var rowsAffected = _sqlClient.ExecuteAsync(sqlInsert, new
            {
                iban = model.Iban,
                userid = model.UserId,
                balance = model.Balance
            });

            return await rowsAffected;
        }

        public async Task<decimal> GetAccountBalanceAsync(string accountId)
        {
            var sql = $@"SELECT balance FROM {AccountTable} where iban = @iban";

            var currentBalance = await _sqlClient.QueryFirstOrDefaultAsync<decimal>(sql, new
            {
                iban = accountId
            });

            return currentBalance;
        }

        public async Task<decimal> GetUserBalanceAsync(Guid userId)
        {
            var sql = $@"SELECT sum(balance) FROM {AccountTable} where userid = @userid";

            var currentBalance = await _sqlClient.QueryFirstOrDefaultAsync<decimal>(sql, new
            {
                userid = userId
            });

            return currentBalance;
        }
    }
}
