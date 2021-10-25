using Contracts.RequestModels;
using Contracts.ResponseModels;
using Domain.Models.RequestModels;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private readonly IUserResolverService _userResolverService;

        public AccountController(
            IAccountService accountService,
            IUserService userService,
            IUserResolverService userResolverService)
        {
            _accountService = accountService;
            _userService = userService;
            _userResolverService = userResolverService;
        }

        [HttpPost]
        [Authorize]
        [Route("newAccount")]
        public async Task<ActionResult<AccountCreateResponse>> CreateAccount()
        {
            var userId = _userResolverService.UserId;

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId);

            var newIban = await _accountService.RandomIbanGenerator();

            var newAccount = new AccountCreateResponse
            {
                Iban = newIban,
                Balance = 0,
                UserId = user.UserId
            };

            await _accountService.InsertAccountAsync(newAccount);

            return Ok(newAccount);
        }

        [HttpGet]
        [Authorize]
        [Route("balance/{iban}")]
        public async Task<ActionResult<decimal>> GetSingleIbanBalance(string iban)
        {
            var userId = _userResolverService.UserId;

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId);

            var currentBalance = await _accountService.GetIbanBalanceAsync(new AccountBalanceRequestModel
            {
                Iban = iban,
                UserId = user.UserId
            });

            return Ok(currentBalance);
        }

        [HttpGet]
        [Authorize]
        [Route("balance")]

        public async Task<ActionResult<decimal>> GetTotalBalance()
        {
            var userId = _userResolverService.UserId;

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId);

            var currentUserBalance = await _accountService.GetUserBalanceAsync(user.UserId);

            return Ok(currentUserBalance);
        }
    }
}
