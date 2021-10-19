using Contracts.ResponseModels;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<int> InsertAccountAsync(AccountCreateResponse newAccount)
        {
            var rowsAffected = await _accountRepository.SaveOrUpdateAsync(newAccount.AsDto());

            return rowsAffected;
        }
    }
}
