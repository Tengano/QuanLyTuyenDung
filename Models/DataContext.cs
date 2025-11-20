using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TrangChu.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        
        public DbSet<TaiKhoan> TaiKhoan { get; set; }
        public DbSet<NhaTuyenDung> NhaTuyenDung { get; set; }
        public DbSet<TinTuyenDung> TinTuyenDung { get; set; }
        public DbSet<HoSoUngTuyen> HoSoUngTuyen { get; set; }
        public DbSet<QuaTrinhPhongVan> QuaTrinhPhongVan { get; set; }
        public DbSet<KetQuaPhongVan> KetQuaPhongVan { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<TinTuyenDung>()
                .HasOne(t => t.NhaTuyenDung)
                .WithMany(n => n.TinTuyenDungs)
                .HasForeignKey(t => t.NhaTuyenDungId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<HoSoUngTuyen>()
                .HasOne(h => h.TinTuyenDung)
                .WithMany(t => t.HoSoUngTuyens)
                .HasForeignKey(h => h.TinTuyenDungId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<HoSoUngTuyen>()
                .HasOne(h => h.TaiKhoan)
                .WithMany()
                .HasForeignKey(h => h.TaiKhoanId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<QuaTrinhPhongVan>()
                .HasOne(q => q.HoSoUngTuyen)
                .WithMany(h => h.QuaTrinhPhongVans)
                .HasForeignKey(q => q.HoSoUngTuyenId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<KetQuaPhongVan>()
                .HasOne(k => k.QuaTrinhPhongVan)
                .WithOne(q => q.KetQuaPhongVan)
                .HasForeignKey<KetQuaPhongVan>(k => k.QuaTrinhPhongVanId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.NhaTuyenDung)
                .WithMany()
                .HasForeignKey(t => t.NhaTuyenDungId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}