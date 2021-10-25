using AutoFixture.Xunit2;
using Contracts.RequestModels;
using Contracts.ResponseModels;
using Domain.Client.Models.ResponseModels;
using Domain.Models.RequestModels;
using Domain.Models.ResponseModels;
using Domain.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RestAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Controller.UnitTests.Controllers
{
    public class AccountController_Should
    {
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<IAccountService> _accountService = new Mock<IAccountService>();
        private readonly Mock<IUserResolverService> _userResolverServiceMock = new Mock<IUserResolverService>();

        private readonly AccountController _sut;

        public AccountController_Should()
        {
            _sut = new AccountController(_accountService.Object, _userServiceMock.Object, _userResolverServiceMock.Object);
        }

        [Theory, AutoData]
        public async Task CreateNewAccount_ShouldReturnNotFoundException_WhenUserIdIsNull()
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(() => null);

            // Act
            var result = await _sut.CreateAccount();

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoData]
        public async Task CreateNewAccount_ShouldCreateNewAccount_WhenUserIdIsNotNull(
            string newIban,
            UserResponseModel user,
            AccountCreateResponse accountCreateResponse)
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(user.LocalId);

            _userServiceMock
                .Setup(mock => mock.GetUserAsync(user.LocalId))
                .ReturnsAsync(user);

            _accountService
                .Setup(mock => mock.RandomIbanGenerator())
                .ReturnsAsync(newIban);

            accountCreateResponse.Iban = newIban;
            accountCreateResponse.UserId = user.UserId;
            accountCreateResponse.Balance = 0;

            _accountService
                .Setup(mock => mock.InsertAccountAsync(accountCreateResponse))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateAccount();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(accountCreateResponse);

            _userServiceMock.Verify(mock => mock.GetUserAsync(It.IsAny<string>()), Times.Once);

            _accountService.Verify(mock => mock.RandomIbanGenerator(), Times.Once);

            _accountService.Verify(mock => mock.InsertAccountAsync(It.IsAny<AccountCreateResponse>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task GetSingleIbanBalance_ShouldReturnNotFoundException_WhenUserIdIsNull(
            string iban)
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(() => null);

            // Act
            var result = await _sut.GetSingleIbanBalance(iban);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoData]
        public async Task GetSingleIbanBalance_ShouldReturnBalance_WhenUserIdIsNotNull(
            string iban,
            UserResponseModel user,
            AccountBalanceRequestModel account)
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(user.LocalId);

            _userServiceMock
                .Setup(mock => mock.GetUserAsync(user.LocalId))
                .ReturnsAsync(user);

            account.Iban = iban;
            account.UserId = user.UserId;

            _accountService
                .Setup(mock => mock.GetIbanBalanceAsync(account))
                .ReturnsAsync(It.IsAny<decimal>());

            // Act
            var result = await _sut.GetSingleIbanBalance(iban);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            _userServiceMock.Verify(mock => mock.GetUserAsync(It.IsAny<string>()), Times.Once);

            _accountService.Verify(mock => mock.GetIbanBalanceAsync(It.IsAny<AccountBalanceRequestModel>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task GetTotalBalance_ShouldReturnNotFoundException_WhenUserIdIsNull()
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(() => null);

            // Act
            var result = await _sut.GetTotalBalance();

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoData]
        public async Task GetTotalBalance_ShouldReturnBalance_WhenUserIdIsNotNull(
            UserResponseModel user)
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(user.LocalId);

            _userServiceMock
                .Setup(mock => mock.GetUserAsync(user.LocalId))
                .ReturnsAsync(user);

            _accountService
                .Setup(mock => mock.GetUserBalanceAsync(user.UserId))
                .ReturnsAsync(It.IsAny<decimal>());

            // Act
            var result = await _sut.GetTotalBalance();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            _userServiceMock.Verify(mock => mock.GetUserAsync(It.IsAny<string>()), Times.Once);

            _accountService.Verify(mock => mock.GetUserBalanceAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
