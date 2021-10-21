using AutoFixture.Xunit2;
using Contracts.ResponseModels;
using Domain.Services;
using FluentAssertions;
using Moq;
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
        public async Task CheckAccountAsync_Return_Boolean(
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
        public async Task InsertAccountAsync_Return_RowsAffected(
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
    }
}
