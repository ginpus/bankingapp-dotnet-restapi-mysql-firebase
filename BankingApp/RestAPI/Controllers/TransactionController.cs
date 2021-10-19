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

        public TransactionController(
            IAccountService accountService,
            IUserService userService)
        {
            _accountService = accountService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        [Route("topUp")]
        public async Task<ActionResult<bool>> TopUpAccount(TopUpRequest topUpRequest)
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

            var result = await _accountService.TopUpAccount(topUpDetails);

            return Ok(result);
        }
    }
}
