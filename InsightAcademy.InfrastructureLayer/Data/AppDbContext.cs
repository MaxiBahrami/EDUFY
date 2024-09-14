using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace InsightAcademy.InfrastructureLayer.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Tutor> Tutor { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<TutorSubject> TutorSubject { get; set; }
        public DbSet<WishList> WishList { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Chat> Chat { get; set; }
        public DbSet<Like> Like { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<InsightAcademy.DomainLayer.Entities.City> City { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<NewSubjectRequest> NewSubjectRequest { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Availability> Availability { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Category>().HasKey(k => k.Id);
            builder.Entity<Category>().HasData(
                new Category() { Id = new Guid("d9342661-6549-47ca-810a-78a47eab07d5"), Name = "Middle-School" },
                 new Category() { Id = new Guid("d9c5b313-fc04-4bcc-b779-a21f3a66246a"), Name = "High-School" }
                );

            builder.Entity<Chat>().HasOne(z => z.Recipient).WithMany(z => z.MessagesReceived).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Chat>().HasOne(z => z.Sender).WithMany(z => z.MessagesSent).OnDelete(DeleteBehavior.Restrict);
        }
    }
}