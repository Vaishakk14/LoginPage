using LoginPage.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginPage.Data

{
    public class LoginDBContext : DbContext
    {
        public LoginDBContext(DbContextOptions<LoginDBContext> options) : base(options)
        {

        }

        //DbSet

        public DbSet<Login> LoginPage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Login>().ToTable("LoginPage");
        }

    }
}
