using DreamDay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectTask = DreamDay.Models.ProjectTask;

namespace DreamDay.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Wedding> Weddings { get; set; }
        public DbSet<Couple> Couples { get; set; }
        public DbSet<WeddingPlanner> WeddingPlanners { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<ProjectTask> Tasks { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<VendorService> VendorServices { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<VendorBooking> VendorBookings { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Timeline> Timelines { get; set; }
        public DbSet<TimelineEvent> TimelineEvents { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Wedding>()
                .HasOne(w => w.Couple)
                .WithMany(c => c.Weddings)
                .HasForeignKey(w => w.CoupleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Wedding>()
                .HasOne(w => w.WeddingPlanner)
                .WithMany(p => p.ManagedWeddings)
                .HasForeignKey(w => w.WeddingPlannerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Report>()
                .HasOne(r => r.GeneratedBy)
                .WithMany()
                .HasForeignKey(r => r.GeneratedById)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}