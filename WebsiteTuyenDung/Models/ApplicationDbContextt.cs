using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebsiteTuyenDung.Models.Entities;

namespace WebsiteTuyenDung.Models
{
    public class ApplicationDbContextt : DbContext  
    {
        public ApplicationDbContextt() : base("DefaultConnection")
        {
        }

        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<HoSoCty> HoSoCongTys { get; set; }
        public DbSet<HoSoCaNhan> HoSoCaNhans { get; set; }
        public DbSet<CVUngVien> CVUngViens { get; set; }
        public DbSet<TinTuyenDung> TinTuyenDungs { get; set; }
        public DbSet<DonUngTuyen> DonUngTuyens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1-1
            modelBuilder.Entity<NguoiDung>()
                .HasOptional(x => x.HoSoCongTy)
                .WithRequired(x => x.NguoiDung);

            modelBuilder.Entity<NguoiDung>()
                .HasOptional(x => x.HoSoCaNhan)
                .WithRequired(x => x.NguoiDung);
        }
    }
}