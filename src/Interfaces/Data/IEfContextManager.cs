using Microsoft.EntityFrameworkCore;

namespace Interfaces.Data
{
    public interface IEfContextManager<out TDataContext> where TDataContext : DbContext
    {
        TDataContext CreateContext();

        TDataContext GetContext();
    }
}