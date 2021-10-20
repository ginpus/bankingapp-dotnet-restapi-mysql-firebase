using Persistence.Client;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ISqlClient _sqlClient;
        private const string TransactionTable = "transactions";

        public TransactionRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }
        public async Task<int> SaveTransactionAsync(TransactionWriteModel model)
        {
            var sqlInsert = @$"INSERT INTO {TransactionTable} (transactionid, iban, type, sum, timestamp, description) VALUES(@transactionid, @iban, @type, @sum, @timestamp, @description)";

            var rowsAffected = _sqlClient.ExecuteAsync(sqlInsert, new
            {
                transactionid = model.TransactionId,
                iban = model.Iban,
                type = model.Type.ToString(),
                sum = model.Sum,
                timestamp = model.Timestamp,
                description = model.Description
            });

            return await rowsAffected;
        }
    }
}
