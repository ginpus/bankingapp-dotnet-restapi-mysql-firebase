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
    public class AuthController_Should
    {
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<IUserResolverService> _userResolverServiceMock = new Mock<IUserResolverService>();

        private readonly AuthController _sut;

        public AuthController_Should()
        {
            _sut = new AuthController(_userServiceMock.Object, _userResolverServiceMock.Object);
        }

        [Theory, AutoData]
        public async Task SignUp_ReturnsUserReadModel_WhenAllChecksPass(
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
        public async Task SignIn_ReturnSignInResponse_WhenAllChecksPass(
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

        [Theory, AutoData]
        public async Task ChangeEmail_ShouldReturnNotFoundException_WhenUserIdIsNull()
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId).Returns(() => null);

            // Act
            var result = await _sut.ChangeEmail(It.IsAny<ChangeEmailRequest>());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Theory, AutoData]
        public async Task ChangeEmail_ShouldReturnNotFoundException_WhenIdTokenIsNull(UserResponseModel user)
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId)
                .Returns(() => user.LocalId);

            _userServiceMock
                .Setup(mock => mock.GetUserAsync(user.LocalId))
                .ReturnsAsync(user);

            _userResolverServiceMock
                .Setup(mock => mock.IdToken)
                .Returns(() => null);

            //Act
            var result = await _sut.ChangeEmail(It.IsAny<ChangeEmailRequest>());

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();

        }

        /*[Theory, AutoData]
        public async Task ChangeEmail_ShouldChangePassword_WhenAllChecksPass(
            UserResponseModel user,
            string idTokenValue,
            ChangeEmailRequest emailRequest,
            ChangeEmailRequestModel request,
            ChangePasswordOrEmailResponse response
            )
        {
            // Arrange
            _userResolverServiceMock
                .Setup(mock => mock.UserId)
                .Returns(() => user.LocalId);

            _userServiceMock
                .Setup(mock => mock.GetUserAsync(user.LocalId))
                .ReturnsAsync(user);

            _userResolverServiceMock
                .Setup(mock => mock.IdToken)
                .Returns(() => idTokenValue);

            request.IdToken = idTokenValue.Remove(0, 7);
            request.ReturnSecureToken = true;
            request.NewEmail = emailRequest.NewEmail;

            _userServiceMock
                .Setup(mock => mock.ChangeEmailAsync(user.UserId, request))
                .ReturnsAsync(response);
            *//*
                        It.Is<ChangePasswordOrEmailResponse>(value =>
                            value.Email.Equals(emailRequest.NewEmail) &&
                            value.LocalId.Equals(user.LocalId) &&
                            value.IdToken.Equals(request.IdToken))*//*

            //Act
            var result = await _sut.ChangeEmail(emailRequest);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);

        }*/
    }
}
