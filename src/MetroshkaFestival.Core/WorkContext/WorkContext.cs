using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Core.WorkContext
{
    public sealed class WorkContext
    {
        public WorkContext(int userId, ApplicationRole[] role)
        {
            UserId = userId;
            Role = role;
        }

        public int UserId { get; }
        public ApplicationRole[] Role { get; }
    }
}