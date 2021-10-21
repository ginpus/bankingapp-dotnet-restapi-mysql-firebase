using AutoFixture.Xunit2;
using Contracts.RequestModels;
using Contracts.ResponseModels;
using Domain.Client;
using Domain.Client.Models.ResponseModels;
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
    public class AuthService_Should
    {
        [Theory, AutoMoqData]
        public async Task SignInAsync_WithSignInRequest_ReturnSignInResponse(
            SignInRequest signInRequest,
            ClientSignInUserResponse signInResponse,
            UserReadModel userReadModel,
            [Frozen] Mock<IAuthClient> authClientMock,
            [Frozen] Mock<IUserRepository> userRepositoryMock,
            UserService sut)
        {
            //Arange
            signInResponse.Email = signInRequest.Email;

            userReadModel.LocalId = signInResponse.LocalId;
            userReadModel.Email = signInResponse.Email;

            //Setup
            authClientMock
                 .Setup(authClient => authClient
                 .SignInUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                 .ReturnsAsync(signInResponse);

            userRepositoryMock
                .Setup(userRepository => userRepository
                .GetUserAsync(signInResponse.LocalId))
                .ReturnsAsync(userReadModel);

            var expectedResult = new SignInResponse
            {
                Email = userReadModel.Email,
                IdToken = signInResponse.IdToken
            };

            //Act

            var result = await sut.SignInUserAsync(signInRequest);

            //Assert

            Assert.Equal(expectedResult.Email, result.Email);
            Assert.Equal(expectedResult.IdToken, result.IdToken);

            authClientMock
                .Verify(authClient => authClient.SignInUserAsync(signInRequest.Email, signInRequest.Password), Times.Once);

            userRepositoryMock
                .Verify(userRepository => userRepository.GetUserAsync(signInResponse.LocalId), Times.Once);

        }

        [Theory]
        [AutoMoqData]
        public async Task SignUpAsync_WithSignUpRequest_ReturnSignUpResponse(
            SignUpRequest signUpRequest,
            CreateUserResponse signUpResponse,
            [Frozen] Mock<IAuthClient> authClientMock,
            [Frozen] Mock<IUserRepository> userRepositoryMock,
            UserService sut)
        {
            //Arange
            signUpResponse.Email = signUpRequest.Email;

            //Setup
            authClientMock
                 .Setup(authClient => authClient
                 .SignUpUserAsync(signUpResponse.Email, signUpRequest.Password))
                 .ReturnsAsync(signUpResponse);

            //Act
            var result = await sut.SignUpAsync(signUpRequest);

            //Assert
            authClientMock
                .Verify(authClient => authClient.SignUpUserAsync(It.Is<string>(value => value.Equals(signUpRequest.Email)),
                    It.Is<string>(value => value.Equals(signUpRequest.Password))), Times.Once);


            userRepositoryMock
                .Verify(userRepository => userRepository.CreateUserAysnc(It.Is<UserWriteModel>(user =>
                user.Email.Equals(signUpResponse.Email) &&
                user.LocalId.Equals(signUpResponse.LocalId))),
                Times.Once);

            /*            Assert.Equal(signUpResponse.Email, result.Email);
                        Assert.Equal(signUpResponse.LocalId, result.LocalId);
                        Assert.IsType<Guid>(result.UserId);*/

            result.Should().BeEquivalentTo(result, options => options.ComparingByMembers<UserReadModel>());

            result.Email.Should().BeEquivalentTo(signUpResponse.Email);
            result.LocalId.Should().BeEquivalentTo(signUpResponse.LocalId);
            result.DateCreated.GetType().Should().Be<DateTime>();

        }
    }
}