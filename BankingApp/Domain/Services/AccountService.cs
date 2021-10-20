using Contracts.ResponseModels;
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

        public async Task<bool> TopUpAccountAsync(TopUpRequestModel request)
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

        public async Task<decimal> GetIbanBalanceAsync(AccountBalanceRequestModel request)
        {
            var accountExists = await _accountRepository.CheckAccountByUserAsync(request.Iban, request.UserId);

            if (!accountExists)
            {
                throw new Exception($"Account {request.Iban} not found for your user");
            }

            var currentBalance = await _accountRepository.GetAccountBalanceAsync(request.Iban);

            return currentBalance;
        }

        public async Task<decimal> GetUserBalanceAsync(Guid userId)
        {
            var currentBalance = await _accountRepository.GetUserBalanceAsync(userId);

            return currentBalance;
        }

        public async Task<string> RandomIbanGenerator()
        {
            var startWith = "LT";
            var generator = new Random();
            var firstIbanPart = generator.Next(0, 999999999).ToString("D9");
            var secondIbanPart = generator.Next(0, 999999999).ToString("D9");
            return startWith + firstIbanPart + secondIbanPart;
        }

        public async Task<bool> SendMoneyAsync(SendMoneyRequestModel request)
        {
            var senderAccountExists = await _accountRepository.CheckAccountByUserAsync(request.SenderIban, request.UserId);

            if (!senderAccountExists)
            {
                throw new Exception($"Account {request.SenderIban} not found for your user");
            }

            var receiverAccountExists = await _accountRepository.CheckAccountByIbanAsync(request.ReceiverIban);

            if (!receiverAccountExists)
            {
                throw new Exception($"Receiver account {request.ReceiverIban} not found");
            }

            var currentSenderBalance = await _accountRepository.GetAccountBalanceAsync(request.SenderIban);

            if (currentSenderBalance < request.Sum)
            {
                throw new Exception($"Insufficient balance. Desired send amount: {request.Sum}. Current balance: {currentSenderBalance}");
            }

            var newSenderBalance = currentSenderBalance - request.Sum;

            var rowsAffetedSend = await _accountRepository.SaveOrUpdateAsync(new AccountSendWriteModel
            {
                Iban = request.SenderIban,
                Balance = newSenderBalance
            });

            //bool resultSend = Convert.ToBoolean(rowsAffetedSend);

            var currentReceiverBalance = await _accountRepository.GetAccountBalanceAsync(request.ReceiverIban);

            var newReceiverBalance = currentReceiverBalance + request.Sum;

            var rowsAffetedReceive = await _accountRepository.SaveOrUpdateAsync(new AccountSendWriteModel
            {
                Iban = request.ReceiverIban,
                Balance = newReceiverBalance
            });

            bool resultReceive = Convert.ToBoolean(rowsAffetedReceive);

            return resultReceive;
        }
    }
}
