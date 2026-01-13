using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Domain.Entities;

namespace ExpenseTracking.Data
{
    public class ExpenseTrackingDbContext : DbContext
    {
        public ExpenseTrackingDbContext(DbContextOptions<ExpenseTrackingDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetAlert> BudgetAlerts { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; }
        public DbSet<ExpenseGroup> ExpenseGroups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.MobileNumber);
                
                entity.HasMany(u => u.Expenses)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasMany(u => u.Budgets)
                    .WithOne(b => b.User)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(u => u.NotificationPreference)
                    .WithOne(n => n.User)
                    .HasForeignKey<NotificationPreference>(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.CategoryName).IsUnique();
                
                entity.HasMany(c => c.Expenses)
                    .WithOne(e => e.Category)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Configure Budget entity
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.HasIndex(b => new { b.UserId, b.CategoryId, b.Year, b.GroupId }).IsUnique();
            });
            
            // Configure ExpenseGroup entity
            modelBuilder.Entity<ExpenseGroup>(entity =>
            {
                entity.HasOne(g => g.Owner)
                    .WithMany()
                    .HasForeignKey(g => g.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Configure UserGroup entity
            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasIndex(ug => new { ug.UserId, ug.GroupId }).IsUnique();
            });
        }
    }
}
