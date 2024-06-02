using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyModel.Models.DTOs;
using MyModel.Models.Entitties;

namespace Server_side.Data
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<Ceremony> Ceremonies { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<DeadPerson> DeadPersons { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .HasOne(u => u.AppUser)
                .WithMany(c => c.Clients)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Report>()
                .HasOne(u => u.AppUser)
                .WithMany(c => c.Reports)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(u => u.Ceremony)
                .WithMany(c => c.Orders)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Ignore<UserSession>();
        }
    }
}
