using System.Linq;
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
        private readonly LoggerService _loggerService;
        private readonly UserService _userService;

        public AccountController(LoggerService loggerService, UserService userService)
        {
            _loggerService = loggerService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return User.Identity.IsAuthenticated ? RedirectToAction("Index", "AdminHome") : LogAndView();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([FromForm] SignInCommandRecord commandRecord)
        {
            if (!ModelState.IsValid)
            {
                return LogAndView(commandRecord);
            }

            var (username, password) = commandRecord;
            var signInErrorCode = await _userService.SignInAsync(username, password, true);
            var result = SignInCommandResult.BuildResult(signInErrorCode);

            if (result.Error != null)
            {
                ModelState.AddModelError("SignIn", result.Error);
                return LogAndView(commandRecord);
            }

            if (!ModelState.IsValid)
            {
                ErrorService.Error(ViewData, UnprocessableEntity);
            }

            Log.Debug("The \"Sign In\" command finished successfully");
            return RedirectToRoute(new { area="Admin", controller="AdminHome", action="Index"});
        }

        [HttpGet]
        [Authorize]
        public new async Task<IActionResult> SignOut()
        {
            await _userService.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private IActionResult LogAndView()
        {
            _loggerService.Information($"The user followed the link [{Request.Host}{Request.Path}]", HttpContext);
            return View();
        }

        private IActionResult LogAndView<TModel>(TModel model)
        {
            var errorMessage = CollectErrors();
            _loggerService.Information($"Unsuccessful login for a reason:[{errorMessage}]", HttpContext);
            return View(model);
        }

        private string CollectErrors()
        {
            var errors = ModelState.Values.SelectMany(valueModelState => valueModelState.Errors);
            var errorMessage = string.Join(", ", errors);
            return errorMessage;
        }
    }
}