using Contracts.ResponseModels;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        public AccountController(
            IAccountService accountService,
            IUserService userService)
        {
            _accountService = accountService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AccountCreateResponse>> CreateAccount()
        {
            var userId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id");

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId.Value);

            var newAccount = new AccountCreateResponse
            {
                Iban = "some_iban",
                Balance = 0,
                UserId = user.UserId
            };

            await _accountService.InsertAccountAsync(newAccount);

            return Ok(newAccount);
        }
    }
}
