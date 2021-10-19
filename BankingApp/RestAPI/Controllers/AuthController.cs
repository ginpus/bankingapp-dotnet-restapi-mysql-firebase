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

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("signUp")]

        public async Task<ActionResult<CreateUserResponse>> SignUp(SignUpRequest request)
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
            Request.Headers.TryGetValue("Authorization", out var idToken);

            //var idToken = this.GetHeaderData("Authorization");

            var idTokenValue = idToken.First().Remove(0, 7); // removes 'Bearer ' from the header

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
            var userId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id");

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId.Value);

            Request.Headers.TryGetValue("Authorization", out var idToken);

            var idTokenValue = idToken.ToString().Remove(0, 7); // removes 'Bearer ' from the header

            var response = await _userService.ChangeEmailAsync(user.UserId, new ChangeEmailRequestModel
            {
                IdToken = idTokenValue,
                NewEmail = request.NewEmail,
                ReturnSecureToken = true
            });

            return Ok(response);
        }

        /*        [HttpGet("GetHeaderData")]
                public ActionResult<string> GetHeaderData(string headerKey)
                {
                    Request.Headers.TryGetValue(headerKey, out var headerValue);
                    return Ok(headerValue);
                }
        //----------------------------------get User ID value----------------------------------
                     var userId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id");

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId.Value);
        
        //----------------------------------get User email value from request----------------------------------
        
            var firebaseIdentity = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "firebase").Value;

            FirebaseIdentity firebaseIdentityJson = (FirebaseIdentity)JsonSerializer.Deserialize(firebaseIdentity, typeof(FirebaseIdentity));

            var userEmail = firebaseIdentityJson.Identities.Email.First();
         
         */
    }
}