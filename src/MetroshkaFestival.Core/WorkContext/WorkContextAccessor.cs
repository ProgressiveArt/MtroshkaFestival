using Interfaces.Application;

namespace MetroshkaFestival.Core.WorkContext
{
    public class WorkContextAccessor : IService
    {
        private MetroshkaFestival.Core.WorkContext.WorkContext _currentContext;

        public virtual MetroshkaFestival.Core.WorkContext.WorkContext CurrentContext
        {
            get => _currentContext;
            set
            {
                if (_currentContext == null)
                {
                    _currentContext = value;
                }
            }
        }
    }
}