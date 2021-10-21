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

        public AccountController(
            IAccountService accountService,
            IUserService userService)
        {
            _accountService = accountService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        [Route("newAccount")]
        public async Task<ActionResult<AccountCreateResponse>> CreateAccount()
        {
            var userId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id");

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId.Value);

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

        [HttpPost]
        [Authorize]
        [Route("balance/single")]
        public async Task<ActionResult<decimal>> GetSingleIbanBalance(SingleIbanBalanceRequest request)
        {
            var userId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id");

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId.Value);

            var currentBalance = await _accountService.GetIbanBalanceAsync(new AccountBalanceRequestModel
            {
                Iban = request.Iban,
                UserId = user.UserId
            });

            return Ok(currentBalance);
        }

        [HttpGet]
        [Authorize]
        [Route("balance")]

        public async Task<ActionResult<decimal>> GetTotalBalance()
        {
            var userId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id");

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId.Value);

            var currentUserBalance = await _accountService.GetUserBalanceAsync(user.UserId);

            return Ok(currentUserBalance);
        }


        [HttpGet]
        [Authorize]
        [Route("transactionHistory")]

        public async Task<ActionResult<TransactionResponse>> GetAllTransactions()
        {
            var userId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id");

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId.Value);

            var transactions = await _accountService.GetAllUserTransactionsAsync(user.UserId);

            return Ok(transactions);
        }
    }
}
