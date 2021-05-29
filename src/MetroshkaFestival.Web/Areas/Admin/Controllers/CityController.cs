using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MetroshkaFestival.Application.Commands.Records.AgeCatehories;
using MetroshkaFestival.Application.Commands.Records.Cities;
using MetroshkaFestival.Application.Queries.Models.Cities;
using MetroshkaFestival.Core.Exceptions.Common;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using GetAgeCategoryListQueryResult = MetroshkaFestival.Application.Queries.Models.AgeGroups.GetAgeCategoryListQueryResult;

namespace MetroshkaFestival.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    public class CityController : Controller
    {
        private readonly DataContext _dataContext;

        public CityController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Index([FromQuery] GetCityListQueryModel query)
        {
            GetCityCategoryListQueryResult result;
            try
            {
                var cities = _dataContext.Cities.OrderBy(x => x.Name).ToArray();

                var resultModel = new CityListModel
                {
                    Cities = cities
                };

                result = GetCityCategoryListQueryResult.BuildResult(resultModel);
            }
            catch (Exception e)
            {
                Log.Error("An error occured while getting ageCategory list", e);
                result = GetCityCategoryListQueryResult.BuildResult(error: e.Message);
            }

            if (result.Error != null)
            {
                throw new Exception(result.Error);
            }

            return View(result.Result);
        }

        [HttpGet]
        public ActionResult GetAddCityPage(string returnUrl)
        {
            var command = new AddCityCommandRecord(returnUrl);
            return View("Add", command);
        }

        [HttpPost]
        public async Task<ActionResult> AddCity(
            [FromForm] AddCityCommandRecord command, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View("Add", command);
            }

            var isContains = await _dataContext.Cities.AnyAsync(x => x.Name.ToUpper() == command.Name.ToUpper(), ct);

            var newCity = new City { Name = command.Name };

            if (!isContains)
            {
                await _dataContext.Cities.AddAsync(newCity, ct);
                await _dataContext.SaveChangesAsync(ct);
            }
            else
            {
                ModelState.AddModelError("AddCity", "Город с таким названием уже существует");
                return View("Add", command);
            }
            return RedirectPermanent(command.ReturnUrl);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteCity(int cityId)
        {
            var city = await _dataContext.Cities.FirstOrDefaultAsync(x => x.Id == cityId);

            if (city == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound);
            }

            if (!city.CanBeRemoved)
            {
                throw new Exception("Город нельзя удалить");
            }

            _dataContext.Cities.Remove(city);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index", "City");
        }
    }
}