using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace CredipathAPI.Data
{
    public class DataContext: DbContext
    {

        public string? Conexion { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public void setConnectionString()
        {
            var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            string? connectionString = config.GetConnectionString("CadenaSQL");

            Conexion = connectionString;

        }
    }
}
