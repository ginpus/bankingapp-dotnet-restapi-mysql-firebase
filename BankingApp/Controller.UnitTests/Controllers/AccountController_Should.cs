using AutoFixture.Xunit2;
using Contracts.RequestModels;
using Contracts.ResponseModels;
using Domain.Client.Models.ResponseModels;
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
        private readonly Mock<HttpContext> _httpContextMock = new Mock<HttpContext>();

        private readonly AccountController _sut;

        public AccountController_Should()
        {
            _sut = new AccountController(_accountService.Object, _userServiceMock.Object)
            {
                ControllerContext =
                {
                    HttpContext = _httpContextMock.Object
                }
            };
        }

        /*       [Theory, AutoData]
               public async Task CreateNewAccount_When_CreateAccount_All_Checks_Pass(
                   SignUpRequest request,
                   UserResponseModel userResponse)
               {
                   // Arrange
                   var userId = SetupHttpContext();

                   _userServiceMock
                       .Setup(mock => mock.SignUpAsync(request))
                       .ReturnsAsync(userResponse);

                   // Act
                   var result = await _sut.SignUp(request);

                   // Assert
                   result.Result.Should().BeOfType<OkObjectResult>()
                       .Which.Value.Should().BeEquivalentTo(userResponse);

                   _userServiceMock.Verify(mock => mock.SignUpAsync(It.IsAny<SignUpRequest>()), Times.Once);
               }*/

        /*[Theory, AutoData]
        public async Task CreateNewAccount_Returns_NotFound_When_UserId_Is_Null(
            SignInRequest request,
            SignInResponse userResponse)
        {
            // Arrange
            var userId = SetupHttpContext();

            _userServiceMock
                .Setup(mock => mock.SignInUserAsync(request))
                .ReturnsAsync(userResponse);

            // Act
            var result = await _sut.SignIn(request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(userResponse);

            _userServiceMock.Verify(mock => mock.SignInUserAsync(It.IsAny<SignInRequest>()), Times.Once);
        }*/

        private Guid SetupHttpContext()
        {
            var userId = Guid.NewGuid();

            _httpContextMock
                .SetupGet(mock => mock.Items["userId"])
                .Returns(userId);

            return userId;
        }
    }
}
