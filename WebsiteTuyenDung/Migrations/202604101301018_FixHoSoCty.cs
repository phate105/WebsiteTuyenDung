namespace WebsiteTuyenDung.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixHoSoCty : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CVUngViens",
                c => new
                    {
                        CVUngVienId = c.Int(nullable: false, identity: true),
                        HoSoCaNhanId = c.Int(nullable: false),
                        TenCV = c.String(),
                        DuongDanFile = c.String(),
                        NgayTaiLen = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CVUngVienId)
                .ForeignKey("dbo.HoSoCaNhans", t => t.HoSoCaNhanId, cascadeDelete: true)
                .Index(t => t.HoSoCaNhanId);
            
            CreateTable(
                "dbo.HoSoCaNhans",
                c => new
                    {
                        HoSoCaNhanId = c.Int(nullable: false),
                        NguoiDungId = c.Int(nullable: false),
                        HoTen = c.String(),
                    })
                .PrimaryKey(t => t.HoSoCaNhanId)
                .ForeignKey("dbo.NguoiDungs", t => t.HoSoCaNhanId)
                .Index(t => t.HoSoCaNhanId);
            
            CreateTable(
                "dbo.NguoiDungs",
                c => new
                    {
                        NguoiDungId = c.Int(nullable: false, identity: true),
                        TenDangNhap = c.String(),
                        Email = c.String(),
                        MatKhau = c.String(),
                        VaiTroId = c.Int(nullable: false),
                        TrangThaiHoatDong = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.NguoiDungId)
                .ForeignKey("dbo.VaiTroes", t => t.VaiTroId, cascadeDelete: true)
                .Index(t => t.VaiTroId);
            
            CreateTable(
                "dbo.HoSoCties",
                c => new
                    {
                        HoSoCongTyId = c.Int(nullable: false),
                        NguoiDungId = c.Int(nullable: false),
                        TenCongTy = c.String(),
                        DiaChi = c.String(),
                    })
                .PrimaryKey(t => t.HoSoCongTyId)
                .ForeignKey("dbo.NguoiDungs", t => t.HoSoCongTyId)
                .Index(t => t.HoSoCongTyId);
            
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
                        HoSoCty_HoSoCongTyId = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CongTies", t => t.CongTyID, cascadeDelete: true)
                .ForeignKey("dbo.DanhMucs", t => t.DanhMucID, cascadeDelete: true)
                .ForeignKey("dbo.HoSoCties", t => t.HoSoCty_HoSoCongTyId)
                .Index(t => t.CongTyID)
                .Index(t => t.DanhMucID)
                .Index(t => t.HoSoCty_HoSoCongTyId);
            
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
                "dbo.DanhMucs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TenDanhMuc = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.VaiTroes",
                c => new
                    {
                        VaiTroId = c.Int(nullable: false, identity: true),
                        TenVaiTro = c.String(),
                    })
                .PrimaryKey(t => t.VaiTroId);
            
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
            DropForeignKey("dbo.NguoiDungs", "VaiTroId", "dbo.VaiTroes");
            DropForeignKey("dbo.HoSoCties", "HoSoCongTyId", "dbo.NguoiDungs");
            DropForeignKey("dbo.TinTuyenDungs", "HoSoCty_HoSoCongTyId", "dbo.HoSoCties");
            DropForeignKey("dbo.TinTuyenDungs", "DanhMucID", "dbo.DanhMucs");
            DropForeignKey("dbo.TinTuyenDungs", "CongTyID", "dbo.CongTies");
            DropForeignKey("dbo.CongTies", "TaiKhoanID", "dbo.TaiKhoans");
            DropForeignKey("dbo.HoSoCaNhans", "HoSoCaNhanId", "dbo.NguoiDungs");
            DropForeignKey("dbo.CVUngViens", "HoSoCaNhanId", "dbo.HoSoCaNhans");
            DropIndex("dbo.HoSoUngViens", new[] { "TaiKhoanID" });
            DropIndex("dbo.DonUngTuyens", new[] { "TinTuyenDung_ID" });
            DropIndex("dbo.DonUngTuyens", new[] { "HoSoUngVien_ID" });
            DropIndex("dbo.CongTies", new[] { "TaiKhoanID" });
            DropIndex("dbo.TinTuyenDungs", new[] { "HoSoCty_HoSoCongTyId" });
            DropIndex("dbo.TinTuyenDungs", new[] { "DanhMucID" });
            DropIndex("dbo.TinTuyenDungs", new[] { "CongTyID" });
            DropIndex("dbo.HoSoCties", new[] { "HoSoCongTyId" });
            DropIndex("dbo.NguoiDungs", new[] { "VaiTroId" });
            DropIndex("dbo.HoSoCaNhans", new[] { "HoSoCaNhanId" });
            DropIndex("dbo.CVUngViens", new[] { "HoSoCaNhanId" });
            DropTable("dbo.HoSoUngViens");
            DropTable("dbo.DonUngTuyens");
            DropTable("dbo.VaiTroes");
            DropTable("dbo.DanhMucs");
            DropTable("dbo.TaiKhoans");
            DropTable("dbo.CongTies");
            DropTable("dbo.TinTuyenDungs");
            DropTable("dbo.HoSoCties");
            DropTable("dbo.NguoiDungs");
            DropTable("dbo.HoSoCaNhans");
            DropTable("dbo.CVUngViens");
        }
    }
}
