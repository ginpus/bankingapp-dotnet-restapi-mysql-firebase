using AutoFixture.Xunit2;
using Contracts.Enums;
using Contracts.ResponseModels;
using Domain.Models.RequestModels;
using Domain.Services;
using FluentAssertions;
using Moq;
using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHelpers.Attributes;
using Xunit;

namespace Domain.UnitTests.Services
{
    public class AccountService_Should
    {
        [Theory, AutoMoqData]
        public async Task CheckAccountAsync_ShouldReturnBoolean_WhenAccountExists(
            string accountId,
            Guid userId,
            [Frozen] Mock<IAccountRepository> accountRepositoryMock,
            bool accountExists,
            AccountService sut)
        {
            //Arrange
            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByUserAsync(accountId, userId))
                .ReturnsAsync(accountExists);

            //Act
            var result = await sut.CheckAccountAsync(accountId, userId);

            //Assert
            accountRepositoryMock
                .Verify(mock => mock.
                CheckAccountByUserAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            result.Should().Be(accountExists);
        }

        [Theory, AutoMoqData]
        public async Task InsertAccountAsync_ShouldReturnRowsAffected_WhenAccountIsInserted(
            AccountCreateResponse newAccount,
            [Frozen] Mock<IAccountRepository> accountRepositoryMock,
            AccountService sut)
        {
            //Act
            await sut.InsertAccountAsync(newAccount);

            //Assert
            accountRepositoryMock
                .Verify(mock => mock.
                SaveOrUpdateAsync(It.Is<AccountWriteModel>(model => model.UserId == newAccount.UserId &&
                model.Iban == newAccount.Iban &&
                model.Balance == newAccount.Balance)), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task GetUserBalanceAsync_ShouldReturnCurrentBalance_WhenUserIdProvided(
            Guid userId,
            [Frozen] Mock<IAccountRepository> accountRepositoryMock,
            AccountService sut)
        {
            //Act
            await sut.GetUserBalanceAsync(userId);

            //Assert
            accountRepositoryMock
                .Verify(mock => mock.
                GetUserBalanceAsync(It.Is<Guid>(model => model.Equals(userId))), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task GetIbanBalanceAsync_ShouReturnException_WhenAccountNotExist(
            AccountBalanceRequestModel request,
            [Frozen] Mock<IAccountRepository> accountRepositoryMock,
            AccountService sut)
        {
            //Arange
            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByUserAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            //Act & Assert
            var result = await sut
                .Invoking(sut => sut.GetIbanBalanceAsync(request))
                .Should().ThrowAsync<Exception>()
                .WithMessage($"Account {request.Iban} not found for your user");

            accountRepositoryMock.Verify(accountRepository => accountRepository.CheckAccountByUserAsync(request.Iban, request.UserId), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task GetIbanBalanceAsync_ShouldReturnCurrentBalance_WhenAccountExists(AccountBalanceRequestModel request,
            bool accountExists,
            decimal currentBalance,
            [Frozen] Mock<IAccountRepository> accountRepositoryMock,
            AccountService sut)
        {
            //Arange
            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByUserAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(accountExists);

            accountRepositoryMock
                .Setup(mock => mock.GetAccountBalanceAsync(It.IsAny<string>()))
                .ReturnsAsync(currentBalance);

            //Act
            var result = await sut.GetIbanBalanceAsync(request);

            //Assert
            result.Should().Be(currentBalance);

            accountRepositoryMock
                .Verify(mock => mock.CheckAccountByUserAsync(request.Iban, request.UserId), Times.Once);

            accountRepositoryMock
                .Verify(mock => mock.GetAccountBalanceAsync(request.Iban), Times.Once);

        }

        [Theory, AutoMoqData]
        public async Task GetAllUserTransactionsAsync_ShouldReturnTransactions_WhenGetTransactionsAsync_IsCalledWithExistingUserId(
            Guid userId,
            IEnumerable<TransactionReadModel> transactions,
            [Frozen] Mock<ITransactionRepository> transactionRepositoryMock,
            AccountService sut)
        {
            //Arrange
            transactionRepositoryMock
                .Setup(mock => mock.GetTransactionsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(transactions);

            //Act
            var result = await sut.GetAllUserTransactionsAsync(userId);

            //Assert
            result.Should().BeEquivalentTo(transactions);

            transactionRepositoryMock
                .Verify(mock => mock.GetTransactionsAsync(userId), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task TopUpAccountAsync_ShouldReturnAccountNotFoundException_WhenAccountDoesNotExist(
            TopUpRequestModel request,
            [Frozen] Mock<IAccountRepository> accountRepositoryMock,
            AccountService sut)
        {
            //Arange
            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByUserAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            //Act & Assert
            var result = await sut
                .Invoking(sut => sut.TopUpAccountAsync(request))
                .Should().ThrowAsync<Exception>()
                .WithMessage(($"Account `{request.Iban}` not found for your user"));

            accountRepositoryMock.Verify(accountRepository => accountRepository.CheckAccountByUserAsync(request.Iban, request.UserId), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task TopUpAccountAsync_ShouldReturnRowsAffected_WhenAllChecksPass(
            TopUpRequestModel request,
            decimal currentBalance,
            AccountWriteModel accountWriteModel,
            [Frozen] Mock<IAccountRepository> accountRepositoryMock,
            AccountService sut)
        {
            //Arange
            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByUserAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                 .ReturnsAsync(true);

            accountRepositoryMock
                .Setup(mock => mock.GetAccountBalanceAsync(request.Iban))
                .ReturnsAsync(currentBalance);

            accountWriteModel.Iban = request.Iban;
            accountWriteModel.UserId = request.UserId;
            accountWriteModel.Balance = currentBalance + request.Sum;

            accountRepositoryMock
                .Setup(mock => mock.SaveOrUpdateAsync(accountWriteModel))
                .ReturnsAsync(It.IsAny<int>());

            //Act
            var result = await sut.TopUpAccountAsync(request);

            //Assert
            accountRepositoryMock.Verify(accountRepository => accountRepository.GetAccountBalanceAsync(It.IsAny<string>()), Times.Once);

            accountRepositoryMock.Verify(accountRepository => accountRepository.SaveOrUpdateAsync(It.IsAny<AccountWriteModel>()), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task TopUpAccountAsync_ShouldSaveTransaction_WhenAllChecksPass(
            TopUpRequestModel request,
            TransactionWriteModel transactionWriteModel,
            [Frozen] Mock<IAccountRepository> accountRepositoryMock,
            [Frozen] Mock<ITransactionRepository> transactionRepositoryMock,
            AccountService sut)
        {
            //Arange
            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByUserAsync(request.Iban, request.UserId))
                 .ReturnsAsync(true);

            accountRepositoryMock
                .Setup(mock => mock.SaveOrUpdateAsync(It.IsAny<AccountWriteModel>()))
                .ReturnsAsync(1);

            transactionWriteModel.Iban = request.Iban;
            transactionWriteModel.Sum = request.Sum;

            transactionRepositoryMock
                  .Setup(mock => mock.SaveTransactionAsync(transactionWriteModel))
                   .ReturnsAsync(It.IsAny<int>());

            //Act
            var result = await sut.TopUpAccountAsync(request);

            //Assert
            accountRepositoryMock.Verify(accountRepository => accountRepository.SaveOrUpdateAsync(It.IsAny<AccountWriteModel>()), Times.Once);

            transactionRepositoryMock.Verify(transactionRepository => transactionRepository.SaveTransactionAsync(It.Is<TransactionWriteModel>(value =>
                value.Iban.Equals(request.Iban) &&
                value.Sum.Equals(request.Sum) &&
                value.Type.Equals(TransactionType.TopUp) &&
                value.Description.Equals($"{TransactionType.TopUp} by {request.Sum}"))));
        }

        [Theory, AutoMoqData]
        public async Task SendMoneyAsync_ShouldReturnSenderAccountNotFoundException_WhenAccountDoesNotExist(
            SendMoneyRequestModel request,
            [Frozen] Mock<IAccountRepository> accountRepositoryMock,
            AccountService sut)
        {
            //Arange
            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByUserAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            //Act & Assert
            var result = await sut
                .Invoking(sut => sut.SendMoneyAsync(request))
                .Should().ThrowAsync<Exception>()
                .WithMessage($"Account {request.SenderIban} not found for your user");

            //Assert
            accountRepositoryMock.Verify(accountRepository => accountRepository.CheckAccountByUserAsync(request.SenderIban, request.UserId), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task SendMoneyAsync_ShouldReturnReceiverAccountNotFoundException_WhenAccountDoesNotExist(
            SendMoneyRequestModel request,
            [Frozen] Mock<IAccountRepository> accountRepositoryMock,
            AccountService sut)
        {
            //Arange
            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByUserAsync(request.SenderIban, request.UserId))
                .ReturnsAsync(true);

            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByIbanAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            //Act & Assert
            var result = await sut
                .Invoking(sut => sut.SendMoneyAsync(request))
                .Should().ThrowAsync<Exception>()
                .WithMessage($"Receiver account {request.ReceiverIban} not found");

            //Assert
            accountRepositoryMock.Verify(accountRepository => accountRepository.CheckAccountByUserAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            accountRepositoryMock.Verify(accountRepository => accountRepository.CheckAccountByIbanAsync(request.ReceiverIban), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task SendMoneyAsync_ShouldReturnInsufficientBalanceException_WhenAccountLacksBalance(
                SendMoneyRequestModel request,
                decimal currentSenderBalance,
                [Frozen] Mock<IAccountRepository> accountRepositoryMock,
                AccountService sut)
        {
            //Arange
            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByUserAsync(request.SenderIban, request.UserId))
                .ReturnsAsync(true);

            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByIbanAsync(request.ReceiverIban))
                .ReturnsAsync(true);

            accountRepositoryMock
                .Setup(mock => mock.GetAccountBalanceAsync(It.IsAny<string>()))
                .ReturnsAsync(currentSenderBalance);

            request.Sum = currentSenderBalance + 1;

            //Act & Assert
            var result = await sut
                .Invoking(sut => sut.SendMoneyAsync(request))
                .Should().ThrowAsync<Exception>()
                .WithMessage($"Insufficient balance. Desired send amount: {request.Sum}. Current balance: {currentSenderBalance}");

            //Assert
            accountRepositoryMock.Verify(accountRepository => accountRepository.CheckAccountByUserAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            accountRepositoryMock.Verify(accountRepository => accountRepository.CheckAccountByIbanAsync(It.IsAny<string>()), Times.Once);

            accountRepositoryMock.Verify(accountRepository => accountRepository.GetAccountBalanceAsync(request.SenderIban), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task SendMoneyAsync_ShouldDebitSenderAccount_WhenAccountBalanceIsSufficient(
                SendMoneyRequestModel request,
                decimal currentSenderBalance,
                AccountSendWriteModel accountSendWriteModel,
                [Frozen] Mock<IAccountRepository> accountRepositoryMock,
                AccountService sut)
        {
            //Arange
            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByUserAsync(request.SenderIban, request.UserId))
                .ReturnsAsync(true);

            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByIbanAsync(request.ReceiverIban))
                .ReturnsAsync(true);

            accountRepositoryMock
                .Setup(mock => mock.GetAccountBalanceAsync(request.SenderIban))
                .ReturnsAsync(currentSenderBalance);

            request.Sum = currentSenderBalance - 1;

            accountSendWriteModel.Iban = request.SenderIban;
            accountSendWriteModel.Balance = 1;

            accountRepositoryMock
                .Setup(mock => mock.SaveOrUpdateAsync(accountSendWriteModel))
                .ReturnsAsync(It.IsAny<int>());

            //Act
            var result = await sut.SendMoneyAsync(request);

            //Assert
            accountRepositoryMock.Verify(accountRepository => accountRepository.CheckAccountByUserAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            accountRepositoryMock.Verify(accountRepository => accountRepository.CheckAccountByIbanAsync(It.IsAny<string>()), Times.Once);

            accountRepositoryMock.Verify(accountRepository => accountRepository.GetAccountBalanceAsync(It.IsAny<string>()), Times.Exactly(2));

            accountRepositoryMock.Verify(accountRepository => accountRepository.SaveOrUpdateAsync(It.IsAny<AccountSendWriteModel>()), Times.Exactly(2));
        }

        [Theory, AutoMoqData]
        public async Task SendMoneyAsync_ShouldRecordSendTransaction_WhenAllChecksPass(
                SendMoneyRequestModel request,
                decimal currentSenderBalance,
                decimal currentReceiverBalance,
                AccountSendWriteModel accountSendWriteModel,
                AccountSendWriteModel accountReceiveWriteModel,
                TransactionWriteModel transactionSendWriteModel,
                TransactionWriteModel transactionReceiveWriteModel,
                [Frozen] Mock<IAccountRepository> accountRepositoryMock,
                [Frozen] Mock<ITransactionRepository> transactionRepositoryMock,
                AccountService sut)
        {
            //Arange
            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByUserAsync(request.SenderIban, request.UserId))
                .ReturnsAsync(true);

            accountRepositoryMock
                .Setup(mock => mock.CheckAccountByIbanAsync(request.ReceiverIban))
                .ReturnsAsync(true);

            accountRepositoryMock
                .Setup(mock => mock.GetAccountBalanceAsync(request.SenderIban))
                .ReturnsAsync(currentSenderBalance);

            request.Sum = currentSenderBalance - 1;

            accountSendWriteModel.Iban = request.SenderIban;
            accountSendWriteModel.Balance = 1;

            accountRepositoryMock
                .Setup(mock => mock.SaveOrUpdateAsync(It.IsAny<AccountWriteModel>()))
                .ReturnsAsync(1);

            transactionSendWriteModel.Iban = request.SenderIban;
            transactionSendWriteModel.Sum = request.Sum * (-1);

            transactionRepositoryMock
                  .Setup(mock => mock.SaveTransactionAsync(transactionSendWriteModel))
                   .ReturnsAsync(It.IsAny<int>());

            accountRepositoryMock
                .Setup(mock => mock.GetAccountBalanceAsync(request.ReceiverIban))
                .ReturnsAsync(currentReceiverBalance);

            accountReceiveWriteModel.Iban = request.ReceiverIban;
            accountReceiveWriteModel.Balance = currentReceiverBalance + request.Sum;

            accountRepositoryMock
                .Setup(mock => mock.SaveOrUpdateAsync(It.IsAny<AccountWriteModel>()))
                .ReturnsAsync(1);

            transactionReceiveWriteModel.Iban = accountReceiveWriteModel.Iban;

            transactionRepositoryMock
                  .Setup(mock => mock.SaveTransactionAsync(transactionReceiveWriteModel))
                   .ReturnsAsync(It.IsAny<int>());

            //Act
            var result = await sut.SendMoneyAsync(request);

            //Assert
            accountRepositoryMock.Verify(accountRepository => accountRepository.CheckAccountByUserAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            accountRepositoryMock.Verify(accountRepository => accountRepository.CheckAccountByIbanAsync(It.IsAny<string>()), Times.Once);

            accountRepositoryMock.Verify(accountRepository => accountRepository.GetAccountBalanceAsync(It.IsAny<string>()), Times.Exactly(2));

            accountRepositoryMock.Verify(accountRepository => accountRepository.SaveOrUpdateAsync(It.IsAny<AccountSendWriteModel>()), Times.Exactly(2));

            transactionRepositoryMock.Verify(transactionRepository => transactionRepository.SaveTransactionAsync(It.Is<TransactionWriteModel>(value =>
                    value.Iban.Equals(request.SenderIban) &&
                    value.Sum.Equals(request.Sum * (-1)))));

            transactionRepositoryMock.Verify(transactionRepository => transactionRepository.SaveTransactionAsync(It.Is<TransactionWriteModel>(value =>
                value.Iban.Equals(request.ReceiverIban))));

        }
    }
}
