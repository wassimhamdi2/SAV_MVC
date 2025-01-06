using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniProjet.ViewModels;

namespace MiniProjet.Models
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Intervention> Interventions { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Technicien> Techniciens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map Complaint to IdentityUser with foreign key
            modelBuilder.Entity<Complaint>()
                .HasOne<IdentityUser>(c => c.Customer)
                .WithMany()  // One customer can have many complaints (no navigation property on IdentityUser)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            // Set precision for decimal properties
            modelBuilder.Entity<Article>()
                .Property(s => s.Prix)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.article)
                .WithMany(a => a.Complaints) // Assuming `Article` has a `Complaints` collection
                .HasForeignKey(c => c.ArticleId); // Match the FK with Article's PK

            // Ensure foreign key on Intervention
            modelBuilder.Entity<Intervention>()
                .HasOne(i => i.Complaint)
                .WithMany(c => c.Interventions)
                .HasForeignKey(i => i.ComplaintId)

                .OnDelete(DeleteBehavior.Restrict); // Optional: Define delete behavior
            modelBuilder.Entity<Intervention>()
               .Property(i => i.TotalCost)
               .HasColumnType("decimal(18, 2)"); // Specify precision and scale (18 digits, 2 decimal places)
            // Exclude RegisterViewModel from the database
            modelBuilder.Entity<RegisterViewModel>(entity =>
            {
                entity.HasNoKey(); // Specify that RegisterViewModel is not a database entity
                entity.ToTable(nameof(RegisterViewModel), t => t.ExcludeFromMigrations()); // Exclude from migrations
            });
        }
    }
}
