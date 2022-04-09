using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ext_trans_dbctx
{
    public class Beta {
        [Key]
        public int BetaId {get; set;}
        [MaxLength(32)]
        public string Name { get; set;}
    }

    public class BetaDbContext : DbContext
    {
        public DbSet<Beta> Betas { get; set; }
        public BetaDbContext(DbContextOptions<BetaDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Beta>().HasIndex(c => c.Name).IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    } 
}