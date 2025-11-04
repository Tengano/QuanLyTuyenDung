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
        
        public DbSet<tblMenu> tblMenu { get; set; }
        public DbSet<AdminMenu> AdminMenu { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure self-referencing relationship for menu hierarchy
            modelBuilder.Entity<tblMenu>()
                .HasOne(m => m.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(m => m.parent_id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}