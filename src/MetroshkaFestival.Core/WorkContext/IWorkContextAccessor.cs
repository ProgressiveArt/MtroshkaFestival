namespace MetroshkaFestival.Core.WorkContext
{
    public interface IWorkContextAccessor
    {
        WorkContext CurrentContext { get; set; }
    }
}