using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrangChu.Areas.Admin.Models;

namespace TrangChu.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        
        public DbSet<Menu> Menu { get; set; }
        public DbSet<AdminMenu> AdminMenu { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure self-referencing relationship for menu hierarchy
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Parent)
                .WithMany(m => m.Children)
            .HasForeignKey(m => m.ParentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}