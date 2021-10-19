using Contracts.ResponseModels;
using Domain.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAccountService
    {
        Task<int> InsertAccountAsync(AccountCreateResponse newAccount);

        Task<bool> CheckAccountAsync(string accountId, Guid userId);

        Task<bool> TopUpAccount(TopUpRequestModel request);

        Task<string> RandomIbanGenerator();

        Task<decimal> GetIbanBalanceAsync(AccountBalanceRequestModel request);

        Task<decimal> GetUserBalance(Guid userId);
    }
}
