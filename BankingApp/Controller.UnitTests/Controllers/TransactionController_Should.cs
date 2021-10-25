using AutoFixture.Xunit2;
using Contracts.RequestModels;
using Contracts.ResponseModels;
using Domain.Models.RequestModels;
using Domain.Models.ResponseModels;
using Domain.Services;
using FluentAssertions;
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
    public class TransactionController_Should
    {
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<IAccountService> _accountServiceMock = new Mock<IAccountService>();
        private readonly Mock<IUserResolverService> _userResolverServiceMock = new Mock<IUserResolverService>();

        private readonly TransactionController _sut;

        public TransactionController_Should()
        {
            _sut = new TransactionController(
                _accountServiceMock.Object,
                _userServiceMock.Object,
                _userResolverServiceMock.Object);
        }

        [Theory, AutoData]
        public async Task TopUpAccount_ShouldReturnNotFoundException_WhenUserIdIsNull()
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(() => null);

            // Act
            var result = await _sut.TopUpAccount(It.IsAny<TopUpRequest>());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoData]
        public async Task TopUpAccount_ShouldReturnInteger_WhenTopUoRequestIsProvided(
            UserResponseModel user,
            TopUpRequestModel topUpRequestDetails,
            TopUpRequest topUpRequest)
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(user.LocalId);

            _userServiceMock
                .Setup(mock => mock.GetUserAsync(user.LocalId))
                .ReturnsAsync(user);

            topUpRequestDetails.Iban = topUpRequest.Iban;
            topUpRequestDetails.Sum = topUpRequest.Sum;
            topUpRequestDetails.UserId = user.UserId;

            _accountServiceMock
                .Setup(mock => mock.TopUpAccountAsync(topUpRequestDetails))
                .ReturnsAsync(It.IsAny<int>());

            //Act
            var result = await _sut.TopUpAccount(topUpRequest);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            _userServiceMock.Verify(mock => mock.GetUserAsync(It.IsAny<string>()), Times.Once);

            _accountServiceMock.Verify(mock => mock.TopUpAccountAsync(It.IsAny<TopUpRequestModel>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task SendMoney_ShouldReturnNotFoundException_WhenUserIdIsNull()
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(() => null);

            // Act
            var result = await _sut.SendMoney(It.IsAny<SendMoneyRequest>());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoData]
        public async Task SendMoney_ShouldReturnInteger_WhenSendMoneyRequestIsProvided(
            UserResponseModel user,
            SendMoneyRequest sendMoneyRequest,
            SendMoneyRequestModel sendMoneyDetails)
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(user.LocalId);

            _userServiceMock
                .Setup(mock => mock.GetUserAsync(user.LocalId))
                .ReturnsAsync(user);

            sendMoneyDetails.SenderIban = sendMoneyRequest.SenderIban;
            sendMoneyDetails.ReceiverIban = sendMoneyRequest.ReceiverIban;
            sendMoneyDetails.Sum = sendMoneyRequest.Sum;
            sendMoneyDetails.UserId = user.UserId;

            _accountServiceMock
                .Setup(mock => mock.SendMoneyAsync(sendMoneyDetails))
                .ReturnsAsync(It.IsAny<int>());

            //Act
            var result = await _sut.SendMoney(sendMoneyRequest);

            //Assert
            _userServiceMock.Verify(mock => mock.GetUserAsync(It.IsAny<string>()), Times.Once);

            _accountServiceMock.Verify(mock => mock.SendMoneyAsync(It.IsAny<SendMoneyRequestModel>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task GetAllTransactions_ShouldReturnNotFoundException_WhenUserIdIsNull()
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(() => null);

            // Act
            var result = await _sut.GetAllTransactions();

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoData]
        public async Task GetAllTransactions_ShouldReturnAllTransactions_WhenAllChecksPass(
            UserResponseModel user)
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(user.LocalId);

            _userServiceMock
                .Setup(mock => mock.GetUserAsync(user.LocalId))
                .ReturnsAsync(user);

            _accountServiceMock
                .Setup(mock => mock.GetAllUserTransactionsAsync(user.UserId))
                .ReturnsAsync(It.IsAny<IEnumerable<TransactionResponse>>());

            // Act
            var result = await _sut.GetAllTransactions();

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            _userServiceMock.Verify(mock => mock.GetUserAsync(It.IsAny<string>()), Times.Once);

            _accountServiceMock.Verify(mock => mock.GetAllUserTransactionsAsync(It.IsAny<Guid>()), Times.Once);

        }
    }
}
