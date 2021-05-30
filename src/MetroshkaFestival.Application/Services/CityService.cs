using System.Linq;
using Interfaces.Application;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetroshkaFestival.Application.Services
{
    public class CityService : IService
    {
        private readonly DataContext _dataContext;

        public CityService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public bool CanBeRemoved(City city)
        {
            var cityCanNotBeRemovedDiscriminatorTournament = _dataContext.Tournaments.Include(x => x.City).Any(x => x.City == city);
            var cityCanNotBeRemovedDiscriminatorTeam = _dataContext.Teams.Include(x => x.TeamCity).Any(x => x.TeamCity == city);

            return !cityCanNotBeRemovedDiscriminatorTournament && !cityCanNotBeRemovedDiscriminatorTeam;
        }
    }
}