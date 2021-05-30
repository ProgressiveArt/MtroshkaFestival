using System.Threading.Tasks;
using MetroshkaFestival.Application.Commands.Records.Account;
using MetroshkaFestival.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MetroshkaFestival.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return User.Identity.IsAuthenticated ? RedirectToAction("Index", "AdminHome") : View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([FromForm] SignInCommandRecord command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            var (username, password) = command;
            var signInErrorCode = await _userService.SignInAsync(username, password, true);
            var result = SignInCommandResult.BuildResult(signInErrorCode);

            if (result.Error != null)
            {
                ModelState.AddModelError("SignIn", result.Error);
                Log.Debug($"Unsuccessful login for a reason:[{result.Error}]");
                return View(command);
            }

            return RedirectToRoute(new { area="Admin", controller="AdminHome", action="Index"});
        }

        [HttpGet]
        [Authorize]
        public new async Task<IActionResult> SignOut()
        {
            await _userService.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}