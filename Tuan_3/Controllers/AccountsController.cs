using HaiChanBank.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaiChanBank.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountsController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("{id}/create")]
        public IActionResult Create(string id, [FromQuery] string owner)
        {
            _accountService.CreateAccount(id, owner);
            return Ok();
        }

        [HttpPost("{id}/deposit")]
        public IActionResult Deposit(string id, [FromQuery] double amount)
        {
            _accountService.Deposit(id, amount);
            return Ok();
        }

        [HttpPost("transfer")]
        public IActionResult Transfer([FromQuery] string fromId, [FromQuery] string toId, [FromQuery] double amount)
        {
            _accountService.Transfer(fromId, toId, amount);
            return Ok();
        }
    }
}