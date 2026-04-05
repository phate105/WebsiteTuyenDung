using System;
using System.Data.Entity;

namespace WebsiteTuyenDung.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection") { }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<CongTy> CongTys { get; set; }
        public DbSet<TinTuyenDung> TinTuyenDungs { get; set; }
        public DbSet<HoSoUngVien> HoSoUngViens { get; set; }
        public DbSet<DonUngTuyen> DonUngTuyens { get; set; }
    }
}