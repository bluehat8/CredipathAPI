using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CredipathAPI.Model;

    public class YourDbContext : DbContext
    {
        public YourDbContext (DbContextOptions<YourDbContext> options)
            : base(options)
        {
        }

        public DbSet<CredipathAPI.Model.Route> Route { get; set; } = default!;
    }
