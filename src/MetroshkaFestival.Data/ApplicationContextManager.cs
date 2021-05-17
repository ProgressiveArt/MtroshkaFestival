using Interfaces.Data;
using Microsoft.EntityFrameworkCore;

namespace MetroshkaFestival.Data
{
    public class ApplicationContextManager : IEfContextManager<DataContext>
    {
        private readonly DataContext _context;

        public ApplicationContextManager(string connectionString)
        {
            ConnectionString = connectionString;
            _context = CreateContext();
        }

        public string ConnectionString { get; }

        public DataContext GetContext()
        {
            return _context;
        }

        public DataContext CreateContext()
        {
            var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseNpgsql(ConnectionString);

            var options = builder.Options;

            return new DataContext(options);
        }
    }
}