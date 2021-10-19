using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public interface IAccountRepository
    {
        Task<int> SaveOrUpdateAsync(AccountWriteModel model);

        Task<bool> CheckAccountByUserAsync(string accountId, Guid userId);

        Task<decimal> GetAccountBalanceAsync(string accountId);

        Task<decimal> GetUserBalanceAsync(Guid userId); 
    }
}
