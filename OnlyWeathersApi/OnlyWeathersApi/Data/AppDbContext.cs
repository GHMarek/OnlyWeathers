using Microsoft.EntityFrameworkCore;
using OnlyWeathersApi.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace OnlyWeathersApi.Data
{
    /// <summary>
    /// Klasa kontekstu bazy danych, która dziedziczy po DbContext.
    /// Odpowiada za konfigurację i dostęp do bazy danych aplikacji.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Konstruktor klasy AppDbContext.
        /// Umożliwia przekazanie opcji konfiguracji bazy danych do klasy bazowej.
        /// </summary>
        /// <param name="options">Opcje konfiguracji bazy danych.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options){}

        /// <summary>
        /// DbSet reprezentujący kolekcję użytkowników w bazie danych.
        /// </summary>
        public DbSet<User> Users => Set<User>();

        /// <summary>
        /// DbSet reprezentujący kolekcję ulubionych miast użytkowników.
        /// </summary>
        public DbSet<FavoriteCity> FavoriteCities => Set<FavoriteCity>();

        /// <summary>
        /// Metoda odpowiedzialna za konfigurację relacji między encjami.
        /// Zdefiniowane relacje to:
        /// - User posiada wiele ulubionych miast (HasMany)
        /// - FavoriteCity ma jednego użytkownika (WithOne)
        /// - Relacja opiera się na kluczu obcym UserId.
        /// </summary>
        /// <param name="modelBuilder">Obiekt modelBuilder do konfigurowania encji.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteCities)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId);
        }
    }
}
