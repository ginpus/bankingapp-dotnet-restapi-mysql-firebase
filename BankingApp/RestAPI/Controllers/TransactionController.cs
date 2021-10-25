using Contracts.RequestModels;
using Contracts.ResponseModels;
using Domain.Models.RequestModels;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IUserResolverService _userResolverService;

        public TransactionController(
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
        [Route("topUp")]
        public async Task<ActionResult<int>> TopUpAccount(TopUpRequest topUpRequest)
        {
            var userId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id");

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId.Value);

            var topUpDetails = new TopUpRequestModel
            {
                Iban = topUpRequest.Iban,
                Sum = topUpRequest.Sum,
                UserId = user.UserId
            };

            var result = await _accountService.TopUpAccountAsync(topUpDetails);

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [Route("send")]
        public async Task<ActionResult<bool>> SendMoney(SendMoneyRequest sendMoneyRequest)
        {
            var userId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id");

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId.Value);

            var sendMoneyDetails = new SendMoneyRequestModel
            {
                SenderIban = sendMoneyRequest.SenderIban,
                Sum = sendMoneyRequest.Sum,
                ReceiverIban = sendMoneyRequest.ReceiverIban,
                UserId = user.UserId
            };

            var result = await _accountService.SendMoneyAsync(sendMoneyDetails);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [Route("history")]

        public async Task<ActionResult<TransactionResponse>> GetAllTransactions()
        {
            var userId = _userResolverService.UserId;

            if (userId is null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(userId);

            var transactions = await _accountService.GetAllUserTransactionsAsync(user.UserId);

            return Ok(transactions);
        }
    }
}
