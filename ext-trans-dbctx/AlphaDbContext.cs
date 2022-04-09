using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ext_trans_dbctx
{
    public class Alpha
    {
        [Key]
        public int AlphaId { get; set; }
        [MaxLength(32)]
        public string Name { get; set; }
    }

    public class AlphaDbContext : DbContext
    {
        public DbSet<Alpha> Alphas { get; set; }
        public AlphaDbContext(DbContextOptions<AlphaDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alpha>().HasIndex(c => c.Name).IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}