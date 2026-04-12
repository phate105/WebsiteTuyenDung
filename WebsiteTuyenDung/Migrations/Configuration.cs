namespace WebsiteTuyenDung.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebsiteTuyenDung.Models.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<WebsiteTuyenDung.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebsiteTuyenDung.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            context.VaiTros.AddOrUpdate(
                v => v.TenVaiTro,
                new VaiTro { TenVaiTro = "Admin" },
                new VaiTro { TenVaiTro = "NhaTuyenDung" },
                new VaiTro { TenVaiTro = "UngVien" }
            );
        }
    }
}
