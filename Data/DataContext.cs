using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CredipathAPI.Data
{
    public class DataContext : DbContext
    {

        public string? Conexion { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Model.Route> Routes { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<InterestTypes> InterestTypes { get; set; }
        public DbSet<Loans> Loans { get; set; }
        public DbSet<Paymentfrequencies> paymentfrequencies { get; set; }
        public DbSet<UserRoute> userRoutes { get; set; }
        public DbSet<ExcludedDays> excludedDays { get; set; }
        public DbSet<LoanAmortization> LoanAmortization { get; set; }
        public DbSet<ViewExpectedvsReality> viewExpectedvsRealities { get; set; }



        public void setConnectionString()
        {
            var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            string? connectionString = config.GetConnectionString("CadenaSQL");

            Conexion = connectionString;

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar el email como único
            modelBuilder.Entity<User>()
                .HasIndex(u => u.email)
                .IsUnique();
        }
    }
}
