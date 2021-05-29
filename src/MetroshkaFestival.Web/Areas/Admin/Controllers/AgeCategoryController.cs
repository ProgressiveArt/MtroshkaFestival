// using System.Threading;
// using System.Threading.Tasks;
// using MetroshkaFestival.Application.Queries.Models.AgeGroups;
// using MetroshkaFestival.Application.Queries.Models.Tournaments;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace MetroshkaFestival.Web.Areas.Admin.Controllers
// {
//     [Authorize(Roles = "Admin")]
//     [Area("Admin")]
//     [Route("[area]/[controller]/[action]")]
//     public class AgeCategoryController : Controller
//     {
//         [HttpGet]
//         public async Task<IActionResult> Index([FromQuery] GetAgeCategoryListQueryModel query, CancellationToken ct)
//         {
//             var result = await handler.Handle(query, ct);
//             if (result.Error != null)
//             {
//                 result.Result = new TournamentListModel
//                 {
//                     Query = query
//                 };
//             }
//
//             FillSelected();
//             return View(result.Result);
//         }
//     }
// }