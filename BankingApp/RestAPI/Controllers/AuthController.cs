using Domain.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Domain.Client.Models.ResponseModels;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Contracts.RequestModels;
using Contracts.ResponseModels;
using Domain.Models.ResponseModels;
using Domain.Services;
using System;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserResolverService _userResolverService;

        public AuthController(IUserService userService, IUserResolverService userResolverService)
        {
            _userService = userService;
            _userResolverService = userResolverService;
        }

        [HttpPost]
        [Route("signUp")]

        public async Task<ActionResult<UserResponseModel>> SignUp(SignUpRequest request)
        {
            var newUser = await _userService.SignUpAsync(request);

            return Ok(newUser);
        }

        [HttpPost]
        [Route("signIn")]
        public async Task<ActionResult<SignInResponse>> SignIn(SignInRequest request)
        {
            var returnedUser = await _userService.SignInUserAsync(request);

            return Ok(returnedUser);
        }

        [HttpPost]
        [Route("changePassword")]
        [Authorize]

        public async Task<ActionResult<EditUserResponse>> ChangePassword(ChangePasswordRequest request)
        {
            var idToken = _userResolverService.IdToken;

            if (string.IsNullOrEmpty(idToken))
            {
                return NotFound();
            }

            var idTokenValue = idToken.Remove(0, 7);

            var response = await _userService.ChangePasswordAsync(new ChangePasswordRequestModel
            {
                IdToken = idTokenValue,
                NewPassword = request.NewPassword,
                ReturnSecureToken = true
            });

            return Ok(response);
        }

        [HttpPost]
        [Route("changeEmail")]
        [Authorize]

        public async Task<ActionResult<EditUserResponse>> ChangeEmail(ChangeEmailRequest request)
        {
            var userId = _userResolverService.UserId;

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId);

            var idToken = _userResolverService.IdToken;

            if (string.IsNullOrEmpty(idToken))
            {
                return NotFound();
            }

            var idTokenValue = idToken.Remove(0, 7);

            var response = await _userService.ChangeEmailAsync(user.UserId, new ChangeEmailRequestModel
            {
                IdToken = idTokenValue,
                NewEmail = request.NewEmail,
                ReturnSecureToken = true
            });

            return Ok(response);
        }
    }
}