﻿using Contracts.ResponseModels;
using Domain.Models.RequestModels;
using Persistence.Models.WriteModels;
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

        public async Task<bool> CheckAccountAsync(string accountId, Guid userId)
        {
            var accountExists = await _accountRepository.CheckAccountByUserAsync(accountId, userId);

            return accountExists;
        }

        public async Task<int> InsertAccountAsync(AccountCreateResponse newAccount)
        {
            var rowsAffected = await _accountRepository.SaveOrUpdateAsync(newAccount.AsDto());

            return rowsAffected;
        }

        public async Task<bool> TopUpAccount(TopUpRequestModel request)
        {
            var accountExists = await _accountRepository.CheckAccountByUserAsync(request.Iban, request.UserId);

            if (!accountExists)
            {
                throw new Exception($"Account {request.Iban} not found for your user");
            }

            var currentBalance = await _accountRepository.GetAccountBalanceAsync(request.Iban);

            var newBalance = currentBalance + request.Sum;

            var rowsAffeted = await _accountRepository.SaveOrUpdateAsync(new AccountWriteModel
            {
                Iban = request.Iban,
                UserId = request.UserId,
                Balance = newBalance
            });

            bool result = Convert.ToBoolean(rowsAffeted);

            return result;
        }

        public async Task<string> RandomIbanGenerator()
        {
            var startWith = "LT";
            var generator = new Random();
            var firstIbanPart = generator.Next(0, 999999999).ToString("D9");
            var secondIbanPart = generator.Next(0, 999999999).ToString("D9");
            return startWith + firstIbanPart + secondIbanPart;
        }
    }
}
