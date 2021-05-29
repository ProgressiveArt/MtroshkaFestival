using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroshkaFestival.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("[area]/main")]
    public class AdminHomeController : Controller
    {
        [HttpGet, Route("")]
        public ActionResult Index()
        {
            return View();
        }
    }
}