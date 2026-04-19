using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WebsiteTuyenDung.Models.Entities;

namespace WebsiteTuyenDung.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<HoSoCongTy> HoSoCongTys { get; set; }
        public DbSet<HoSoCaNhan> HoSoCaNhans { get; set; }
        public DbSet<CVUngVien> CVUngViens { get; set; }
        public DbSet<NganhNghe> NganhNghes { get; set; }
        public DbSet<DiaDiem> DiaDiems { get; set; }
        public DbSet<LoaiHinhLamViec> LoaiHinhLamViecs { get; set; }
        public DbSet<CapDoKinhNghiem> CapDoKinhNghiems { get; set; }
        public DbSet<TinTuyenDung> TinTuyenDungs { get; set; }
        public DbSet<DonUngTuyen> DonUngTuyens { get; set; }
        public DbSet<NhatKyDuyetTin> NhatKyDuyetTins { get; set; }
        public DbSet<LichSuTrangThaiDon> LichSuTrangThaiDons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<HoSoCaNhan>()
                .HasRequired(x => x.ApplicationUser)
                .WithMany()
                .HasForeignKey(x => x.ApplicationUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HoSoCongTy>()
                .HasRequired(x => x.ApplicationUser)
                .WithMany()
                .HasForeignKey(x => x.ApplicationUserId)
                .WillCascadeOnDelete(false);
        }
    }
}
