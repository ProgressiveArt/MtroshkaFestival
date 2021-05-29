using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnumsNET;
using MetroshkaFestival.Application.Commands.Records.Groups;
using MetroshkaFestival.Core.Exceptions.Common;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MetroshkaFestival.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    public class GroupController : Controller
    {
        private readonly DataContext _dataContext;

        public GroupController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public ActionResult GetAddGroupPage(string returnUrl, int tournamentId)
        {
            var command = new AddGroupCommandRecord(returnUrl, tournamentId);
            FillSelected();
            return View("Add", command);
        }

        [HttpPost]
        public async Task<ActionResult> AddGroup(AddGroupCommandRecord command, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                FillSelected();
                return View("Add", command);
            }

            var tournament = await _dataContext.Tournaments
                .Include(x => x.Groups)
                .ThenInclude(x => x.AgeCategory)
                .FirstOrDefaultAsync(x => x.Id == command.TournamentId, ct);

            if (tournament == null)
            {
                ModelState.AddModelError("AddGroup", "Невозможно добавить группу к несуществующему турниру");
                FillSelected();
                return View("Add", command);
            }

            if (tournament.Groups.Any(x => x.Name == command.Name && x.AgeCategory.Id == command.AgeCategoryId))
            {
                ModelState.AddModelError("AddGroup", "Такая группа уже существует");
                FillSelected();
                return View("Add", command);
            }

            var ageCategory = _dataContext.AgeCategories.First(x => x.Id == command.AgeCategoryId);
            var newGroup = new Group
            {
                Name = command.Name.Value,
                AgeCategory = ageCategory
            };

            ageCategory.CanBeRemoved = false;
            tournament.Groups.Add(newGroup);
            tournament.CanBeRemoved = false;
            await _dataContext.SaveChangesAsync(ct);

            return RedirectPermanent(command.ReturnUrl);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteGroup(string returnUrl, int groupId, int tournamentId)
        {
            var group = await _dataContext.Groups
                .Include(x => x.AgeCategory)
                .FirstOrDefaultAsync(x => x.Id == groupId);

            if (group == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound);
            }

            var ageCategoryId = group.AgeCategory.Id;
            _dataContext.Groups.Remove(group);
            await _dataContext.SaveChangesAsync();

            var tournament = await _dataContext.Tournaments
                .Include(x => x.Groups)
                .FirstAsync(x => x.Id == tournamentId);

            var ageCategoryCanBeRemoved = await _dataContext.Groups
                .Include(x => x.AgeCategory)
                .AnyAsync(x => x.AgeCategory.Id == ageCategoryId);

            if (tournament.Groups.Count == 0)
            {
                tournament.CanBeRemoved = true;
                await _dataContext.SaveChangesAsync();
            }

            if (ageCategoryCanBeRemoved)
            {
                var ageCategory = await _dataContext.AgeCategories.FirstOrDefaultAsync(x => x.Id == ageCategoryId);
                ageCategory.CanBeRemoved = true;
                await _dataContext.SaveChangesAsync();
            }

            return RedirectPermanent(returnUrl);
        }

        private void FillSelected()
        {
            var groupNames = Enum.GetValues(typeof(GroupNames))
                .Cast<int>()
                .ToArray();

            var groups = groupNames.Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = $"{((GroupNames) x).AsString(EnumFormat.Description)}"
            });

            ViewBag.GroupNames = groups;

            var ageCategoryDictionary = _dataContext.AgeCategories.ToDictionary(key => key.Id, value => value.Name);
            var ageCategories = ageCategoryDictionary.Select(x => new SelectListItem
            {
                Value = x.Key.ToString(),
                Text = $"{x.Value}"
            });

            ViewBag.AgeCategories = ageCategories;
        }
    }
}