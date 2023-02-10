using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Entities
{
    //Klasa reprezentuje połączenie do bazy danych poprzez Entity Framework
    //
    //Utwórz i wykonaj migracje, która zmieni typy kolumn City i
    //Street Z tabeli Addresses, na wymagane o maksymalnej
    //dtugoci 50 znaków.
    //Nastepnie upewnij sie, ze zmiany na bazie zostaty poprawnie
    //zaaplikowane.

    public class RestaurantDbContext : DbContext
    {
        private string _connectionString = 
            "Server=JAKUB-PC;Database=estaurantDb;Trusted_connection=True;";
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Adresses { get; set; }
        public DbSet<Dish> Dishes { get; set; }

        //Cechy dla konkretnych kolumn
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(25);
            modelBuilder.Entity<Dish>()
               .Property(d => d.Name)
               .IsRequired();
            modelBuilder.Entity<Address>()
                .Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<Address>()
                .Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(50);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
