using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroshkaFestival.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("/main")]
    public class AdminHomeController : Controller
    {
        [HttpGet, Route("")]
        public ActionResult Index()
        {
            return View();
        }
    }
}