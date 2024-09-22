using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

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
        public DbSet<InterestTypes> InterestTypes { get; set; } 

        public DbSet<Paymentfrequencies> paymentfrequencies { get; set; }   

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
