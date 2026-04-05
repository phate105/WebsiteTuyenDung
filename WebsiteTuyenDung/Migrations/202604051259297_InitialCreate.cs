namespace WebsiteTuyenDung.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CongTies",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TaiKhoanID = c.Int(nullable: false),
                        TenCongTy = c.String(nullable: false),
                        DiaChi = c.String(),
                        Logo = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TaiKhoans", t => t.TaiKhoanID, cascadeDelete: true)
                .Index(t => t.TaiKhoanID);
            
            CreateTable(
                "dbo.TaiKhoans",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        MatKhau = c.String(nullable: false),
                        VaiTro = c.String(),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TinTuyenDungs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CongTyID = c.Int(nullable: false),
                        DanhMucID = c.Int(nullable: false),
                        TieuDe = c.String(nullable: false),
                        MoTa = c.String(),
                        TrangThai = c.String(),
                        NgayDang = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CongTies", t => t.CongTyID, cascadeDelete: true)
                .ForeignKey("dbo.DanhMucs", t => t.DanhMucID, cascadeDelete: true)
                .Index(t => t.CongTyID)
                .Index(t => t.DanhMucID);
            
            CreateTable(
                "dbo.DanhMucs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TenDanhMuc = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DonUngTuyens",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TinID = c.Int(nullable: false),
                        HoSoID = c.Int(nullable: false),
                        NgayNop = c.DateTime(nullable: false),
                        TrangThaiDon = c.String(),
                        HoSoUngVien_ID = c.Int(),
                        TinTuyenDung_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HoSoUngViens", t => t.HoSoUngVien_ID)
                .ForeignKey("dbo.TinTuyenDungs", t => t.TinTuyenDung_ID)
                .Index(t => t.HoSoUngVien_ID)
                .Index(t => t.TinTuyenDung_ID);
            
            CreateTable(
                "dbo.HoSoUngViens",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TaiKhoanID = c.Int(nullable: false),
                        HoTen = c.String(nullable: false),
                        SoDienThoai = c.String(),
                        FileCV = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TaiKhoans", t => t.TaiKhoanID, cascadeDelete: true)
                .Index(t => t.TaiKhoanID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DonUngTuyens", "TinTuyenDung_ID", "dbo.TinTuyenDungs");
            DropForeignKey("dbo.DonUngTuyens", "HoSoUngVien_ID", "dbo.HoSoUngViens");
            DropForeignKey("dbo.HoSoUngViens", "TaiKhoanID", "dbo.TaiKhoans");
            DropForeignKey("dbo.TinTuyenDungs", "DanhMucID", "dbo.DanhMucs");
            DropForeignKey("dbo.TinTuyenDungs", "CongTyID", "dbo.CongTies");
            DropForeignKey("dbo.CongTies", "TaiKhoanID", "dbo.TaiKhoans");
            DropIndex("dbo.HoSoUngViens", new[] { "TaiKhoanID" });
            DropIndex("dbo.DonUngTuyens", new[] { "TinTuyenDung_ID" });
            DropIndex("dbo.DonUngTuyens", new[] { "HoSoUngVien_ID" });
            DropIndex("dbo.TinTuyenDungs", new[] { "DanhMucID" });
            DropIndex("dbo.TinTuyenDungs", new[] { "CongTyID" });
            DropIndex("dbo.CongTies", new[] { "TaiKhoanID" });
            DropTable("dbo.HoSoUngViens");
            DropTable("dbo.DonUngTuyens");
            DropTable("dbo.DanhMucs");
            DropTable("dbo.TinTuyenDungs");
            DropTable("dbo.TaiKhoans");
            DropTable("dbo.CongTies");
        }
    }
}
