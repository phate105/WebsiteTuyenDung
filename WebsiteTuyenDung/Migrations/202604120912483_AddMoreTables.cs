namespace WebsiteTuyenDung.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMoreTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DiaDiems",
                c => new
                    {
                        DiaDiemId = c.Int(nullable: false, identity: true),
                        TenDiaDiem = c.String(),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DiaDiemId);
            
            CreateTable(
                "dbo.NganhNghes",
                c => new
                    {
                        NganhNgheId = c.Int(nullable: false, identity: true),
                        TenNganhNghe = c.String(),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.NganhNgheId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NganhNghes");
            DropTable("dbo.DiaDiems");
        }
    }
}
