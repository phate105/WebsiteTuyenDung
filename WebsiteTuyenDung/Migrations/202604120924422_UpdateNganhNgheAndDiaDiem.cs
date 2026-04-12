namespace WebsiteTuyenDung.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateNganhNgheAndDiaDiem : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DiaDiems", "TenDiaDiem", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.NganhNghes", "TenNganhNghe", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.TinTuyenDungs", "NganhNgheId");
            CreateIndex("dbo.TinTuyenDungs", "DiaDiemId");
            AddForeignKey("dbo.TinTuyenDungs", "DiaDiemId", "dbo.DiaDiems", "DiaDiemId");
            AddForeignKey("dbo.TinTuyenDungs", "NganhNgheId", "dbo.NganhNghes", "NganhNgheId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TinTuyenDungs", "NganhNgheId", "dbo.NganhNghes");
            DropForeignKey("dbo.TinTuyenDungs", "DiaDiemId", "dbo.DiaDiems");
            DropIndex("dbo.TinTuyenDungs", new[] { "DiaDiemId" });
            DropIndex("dbo.TinTuyenDungs", new[] { "NganhNgheId" });
            AlterColumn("dbo.NganhNghes", "TenNganhNghe", c => c.String());
            AlterColumn("dbo.DiaDiems", "TenDiaDiem", c => c.String());
        }
    }
}
