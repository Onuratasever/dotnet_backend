using HelloWorld.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HelloWorld.Data
{
    public class DataContextEF : DbContext
    {
        private IConfiguration _config;

        public DataContextEF(IConfiguration config)
        {
            _config = config;
        }
        public DbSet<Computer> Computers { get; set; } // This will create a table called Computers in the database.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if(!options.IsConfigured) // This will check if the options are already configured. If they are not, it will configure them.
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                options => options.EnableRetryOnFailure()); //This will enable the retry on failure feature. Which means that if the connection fails, it will try to reconnect.
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TutorialAppSchema"); // This will set the default schema to TutorialAppSchema. Because it checks DBO shcema by default.
            modelBuilder.Entity<Computer>().HasKey(c => c.ComputerId); // This will set the ComputerId as the primary key.
            modelBuilder.Entity<Computer>()
                .ToTable("Computer", "TutorialAppSchema"); // This will map the Computer class to the Computer table in the TutorialAppSchema schema.
        }
    }
}