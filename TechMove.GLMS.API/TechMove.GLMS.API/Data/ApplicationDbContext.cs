using Microsoft.EntityFrameworkCore;
using TechMove.GLMS.API.Models;

namespace TechMove.GLMS.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Agreement -> Client relationship
            modelBuilder.Entity<Agreement>()
                .HasOne(a => a.Client)
                .WithMany(c => c.Agreements)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceRequest -> Agreement relationship
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Agreement)
                .WithMany(a => a.ServiceRequests)
                .HasForeignKey(sr => sr.AgreementId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}