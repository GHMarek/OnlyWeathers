using Microsoft.EntityFrameworkCore;
using OnlyWeathersApi.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace OnlyWeathersApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<FavoriteCity> FavoriteCities => Set<FavoriteCity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteCities)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId);
        }
    }
}
