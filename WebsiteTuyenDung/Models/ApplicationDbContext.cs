using System.Data.Entity;
using WebsiteTuyenDung.Models.Entities;

namespace WebsiteTuyenDung.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<HoSoCongTy> HoSoCongTies { get; set; }
        public DbSet<HoSoCaNhan> HoSoCaNhans { get; set; }
        public DbSet<TinTuyenDung> TinTuyenDungs { get; set; }
        public DbSet<DonUngTuyen> DonUngTuyens { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<CVUngVien> CVUngViens { get; set; }
        public DbSet<LoaiHinhLamViec> LoaiHinhLamViecs { get; set; }
        public DbSet<CapDoKinhNghiem> CapDoKinhNghiems { get; set; }
        public DbSet<NhatKyDuyetTin> NhatKyDuyetTins { get; set; }
        public DbSet<LichSuTrangThaiDon> LichSuTrangThaiDons { get; set; }
        public DbSet<DiaDiem> DiaDiems  { get; set; }
        public DbSet<NganhNghe> NganhNghes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tắt cascade delete
            modelBuilder.Conventions.Remove<
                System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention>();
        }
    }
}