using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnumsNET;
using MetroshkaFestival.Application.Commands.Records.AgeCatehories;
using MetroshkaFestival.Application.Queries.Models.AgeGroups;
using MetroshkaFestival.Core.Exceptions.Common;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MetroshkaFestival.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    public class AgeCategoryController : Controller
    {
        private readonly DataContext _dataContext;

        public AgeCategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Index([FromQuery] GetAgeCategoryListQueryModel query, CancellationToken ct)
        {
            GetAgeCategoryListQueryResult result;

            try
            {
                IQueryable<AgeCategory> ageCategoryQuery = _dataContext.AgeCategories;

                if (query.Sort != null)
                {
                    ageCategoryQuery = query.Sort.Apply(ageCategoryQuery);
                }

                var ageCategories = ageCategoryQuery.ToArray();

                var resultModel = new AgeCategoryListModel
                {
                    Query = query,
                    AgeCategories = ageCategories
                };

                result = GetAgeCategoryListQueryResult.BuildResult(resultModel);
            }
            catch (Exception e)
            {
                Log.Error("An error occured while getting ageCategory list", e);
                result = GetAgeCategoryListQueryResult.BuildResult(error: e.Message);
            }

            if (result.Error != null)
            {
                throw new Exception(result.Error);
            }

            FillSelected();
            return View(result.Result);
        }

        [HttpGet]
        public ActionResult GetAddCategoryPage(string returnUrl)
        {
            var command = new AddAgeCategoryCommandRecord(returnUrl);
            FillSelected();
            return View("Add", command);
        }

        [HttpPost]
        public async Task<ActionResult> AddAgeCategory(
            [FromForm] AddAgeCategoryCommandRecord command, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                FillSelected();
                return View("Add", command);
            }

            if (command.MinBirthYear > command.MaxBirthYear)
            {
                ModelState.AddModelError("AddAgeCategory", "Минимальный год рождения не может быть больше максимального");
                FillSelected();
                return View("Add", command);
            }

            var newAgeCategory = new AgeCategory
            {
                AgeGroup = command.AgeGroup.Value,
                MinBirthDate = new DateTime(command.MinBirthYear.Value, 1, 1),
                MaxBirthDate = new DateTime(command.MaxBirthYear.Value, 12, 31)
            };

            var isContains = await _dataContext.AgeCategories.AnyAsync(x =>
                x.AgeGroup == newAgeCategory.AgeGroup &&
                x.MinBirthDate.Equals(newAgeCategory.MinBirthDate) &&
                x.MaxBirthDate.Equals(newAgeCategory.MaxBirthDate), ct);

            if (!isContains)
            {
                await _dataContext.AgeCategories.AddAsync(newAgeCategory, ct);
                await _dataContext.SaveChangesAsync(ct);
            }
            else
            {
                ModelState.AddModelError("AddAgeCategory", "Такая категория уже существует");
                FillSelected();
                return View("Add", command);
            }
            return RedirectPermanent(command.ReturnUrl);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteAgeCategory(int ageCategoryId)
        {
            var ageCategory = await _dataContext.AgeCategories.FirstOrDefaultAsync(x => x.Id == ageCategoryId);

            if (ageCategory == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound);
            }

            if (!ageCategory.CanBeRemoved)
            {
                throw new Exception("Категорию нельзя удалить");
            }

            _dataContext.AgeCategories.Remove(ageCategory);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index", "AgeCategory");
        }

        private void FillSelected()
        {
            var years = new List<int>();
            for (var i = DateTime.Now.Year - 20; i < DateTime.Now.Year; i++)
            {
                years.Add(i);
            }

            var yearSelectListItems = years.Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = x.ToString()
            });
            ViewBag.Years = yearSelectListItems;

            var ageGroupNames = Enum.GetValues(typeof(AgeGroup))
                .Cast<int>()
                .ToArray();

            var ageGroups = ageGroupNames.Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = $"{((AgeGroup) x).AsString(EnumFormat.Description)}"
            });

            ViewBag.AgeGroups = ageGroups;
        }
    }
}