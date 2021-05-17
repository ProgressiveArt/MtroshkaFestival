using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MetroshkaFestival.Application.Commands.Handlers;
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
        private readonly LoggerService _loggerService;

        public AccountController(UserService userService,
            LoggerService loggerService)
        {
            _userService = userService;
            _loggerService = loggerService;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return User.Identity.IsAuthenticated ? RedirectToAction("Index", "Home") : LogAndView();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([FromServices] SignInCommandHandler handler,
            [FromForm] SignInCommandRecord commandRecord,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return LogAndView(commandRecord);
            }

            var result = await handler.Handle(commandRecord, ct);

            if (result.ErrorCode != null)
            {
                ModelState.AddModelError("Login", result.ErrorCode);
                return LogAndView(commandRecord);
            }

            if (!ModelState.IsValid)
            {
                ErrorService.Error(ViewData, UnprocessableEntity);
            }

            Log.Debug("The \"Sign In\" command finished successfully");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SignOut([FromServices] SignOutCommandHandler handler,
            [FromQuery] SignOutCommandRecord commandRecord,
            CancellationToken ct)
        {
            await handler.Handle(commandRecord, ct);
            return RedirectToAction("SignIn", "Account");
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