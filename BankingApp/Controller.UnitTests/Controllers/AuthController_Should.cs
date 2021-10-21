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
    public class AuthController_Should
    {
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<HttpContext> _httpContextMock = new Mock<HttpContext>();

        private readonly AuthController _sut;

        public AuthController_Should()
        {
            _sut = new AuthController(_userServiceMock.Object)
            {
                ControllerContext =
                {
                    HttpContext = _httpContextMock.Object
                }
            };
        }

        [Theory, AutoData]
        public async Task ReturnUserReadModel_When_SignUp_Is_Called(
            SignUpRequest request,
            UserResponseModel userResponse)
        {
            // Arrange
            _userServiceMock
                .Setup(mock => mock.SignUpAsync(request))
                .ReturnsAsync(userResponse);

            // Act
            var result = await _sut.SignUp(request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(userResponse);

            _userServiceMock.Verify(mock => mock.SignUpAsync(It.IsAny<SignUpRequest>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task ReturnSignInResponse_When_SignIn_Is_Called(
            SignInRequest request,
            SignInResponse userResponse)
        {
            // Arrange
            _userServiceMock
                .Setup(mock => mock.SignInUserAsync(request))
                .ReturnsAsync(userResponse);

            // Act
            var result = await _sut.SignIn(request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(userResponse);

            _userServiceMock.Verify(mock => mock.SignInUserAsync(It.IsAny<SignInRequest>()), Times.Once);
        }

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
