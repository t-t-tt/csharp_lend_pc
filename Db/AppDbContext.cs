using Microsoft.EntityFrameworkCore;
using csharp_lend_pc.Models;

namespace csharp_lend_pc.Db
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<EmployeeEntity> employees { get; set; }
        public DbSet<PcEntity> pcs { get; set; }
        public DbSet<LendEntity> lends { get; set; }
        public DbSet<UserEntity> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeEntity>();
            modelBuilder.Entity<PcEntity>();
            modelBuilder.Entity<LendEntity>();
            modelBuilder.Entity<UserEntity>();
            base.OnModelCreating(modelBuilder);
        }
    }
}