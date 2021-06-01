using System;

namespace MetroshkaFestival.Application.Queries.Models.Players
{
    public class GetPlayerSummaryQueryModel
    {
        public string ReturnUrl {get;set;}
        public int TeamId {get;set;}
        public int PlayerId {get;set;}
    }

    public class PlayerModel
    {
        public string ReturnUrl { get; set; }

        public int PlayerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }
        public int NumberInTeam { get; set; }
        public int TeamId { get; set; }
    }
}