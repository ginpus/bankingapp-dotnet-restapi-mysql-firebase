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
            //AccountWriteModel accountWriteModel,
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
    }
}
