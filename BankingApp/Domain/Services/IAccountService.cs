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

        Task<bool> TopUpAccountAsync(TopUpRequestModel request);

        Task<bool> SendMoneyAsync(SendMoneyRequestModel sendMoneyDetails);

        Task<string> RandomIbanGenerator();

        Task<decimal> GetIbanBalanceAsync(AccountBalanceRequestModel request);

        Task<decimal> GetUserBalanceAsync(Guid userId);

        Task<IEnumerable<TransactionResponse>> GetAllUserTransactionsAsync(Guid userId);
    }
}
