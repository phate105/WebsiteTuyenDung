namespace WebsiteTuyenDung.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CapDoKinhNghiems",
                c => new
                    {
                        CapDoKinhNghiemId = c.Int(nullable: false, identity: true),
                        TenCapDoKinhNghiem = c.String(),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CapDoKinhNghiemId);
            
            CreateTable(
                "dbo.TinTuyenDungs",
                c => new
                    {
                        TinTuyenDungId = c.Int(nullable: false, identity: true),
                        HoSoCongTyId = c.Int(nullable: false),
                        NganhNgheId = c.Int(nullable: false),
                        DiaDiemId = c.Int(nullable: false),
                        LoaiHinhLamViecId = c.Int(nullable: false),
                        CapDoKinhNghiemId = c.Int(nullable: false),
                        TieuDe = c.String(),
                        MoTaCongViec = c.String(),
                        NgayDang = c.DateTime(nullable: false),
                        HanNopHoSo = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TinTuyenDungId)
                .ForeignKey("dbo.CapDoKinhNghiems", t => t.CapDoKinhNghiemId)
                .ForeignKey("dbo.HoSoCongTies", t => t.HoSoCongTyId)
                .ForeignKey("dbo.LoaiHinhLamViecs", t => t.LoaiHinhLamViecId)
                .Index(t => t.HoSoCongTyId)
                .Index(t => t.LoaiHinhLamViecId)
                .Index(t => t.CapDoKinhNghiemId);
            
            CreateTable(
                "dbo.HoSoCongTies",
                c => new
                    {
                        NguoiDungId = c.Int(nullable: false),
                        TenCongTy = c.String(nullable: false),
                        MaSoThue = c.String(maxLength: 20),
                        MoTa = c.String(),
                        DiaChi = c.String(),
                        Website = c.String(),
                        Logo = c.String(),
                        EmailLienHe = c.String(),
                        SoDienThoaiLienHe = c.String(maxLength: 15),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NguoiDungId)
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDungId)
                .Index(t => t.NguoiDungId);
            
            CreateTable(
                "dbo.NguoiDungs",
                c => new
                    {
                        NguoiDungId = c.Int(nullable: false, identity: true),
                        TenDangNhap = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 100),
                        MatKhau = c.String(nullable: false, maxLength: 50),
                        TrangThaiHoatDong = c.Boolean(nullable: false),
                        VaiTroId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.NguoiDungId)
                .ForeignKey("dbo.VaiTroes", t => t.VaiTroId)
                .Index(t => t.TenDangNhap, unique: true)
                .Index(t => t.Email, unique: true)
                .Index(t => t.VaiTroId);
            
            CreateTable(
                "dbo.HoSoCaNhans",
                c => new
                    {
                        NguoiDungId = c.Int(nullable: false),
                        HoTen = c.String(nullable: false),
                        NgaySinh = c.DateTime(),
                        GioiTinh = c.String(),
                        SoDienThoai = c.String(maxLength: 15),
                        DiaChi = c.String(),
                        MucTieuNgheNghiep = c.String(),
                        HocVan = c.String(),
                        TomTatKinhNghiem = c.String(),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NguoiDungId)
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDungId)
                .Index(t => t.NguoiDungId);
            
            CreateTable(
                "dbo.CVUngViens",
                c => new
                    {
                        CVUngVienId = c.Int(nullable: false, identity: true),
                        HoSoCaNhanId = c.Int(nullable: false),
                        TenCV = c.String(),
                        DuongDanFile = c.String(),
                        NgayTaiLen = c.DateTime(nullable: false),
                        TrangThaiSuDung = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CVUngVienId)
                .ForeignKey("dbo.HoSoCaNhans", t => t.HoSoCaNhanId)
                .Index(t => t.HoSoCaNhanId);
            
            CreateTable(
                "dbo.VaiTroes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenVaiTro = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LoaiHinhLamViecs",
                c => new
                    {
                        LoaiHinhLamViecId = c.Int(nullable: false, identity: true),
                        TenLoaiHinhLamViec = c.String(),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LoaiHinhLamViecId);
            
            CreateTable(
                "dbo.DonUngTuyens",
                c => new
                    {
                        DonUngTuyenId = c.Int(nullable: false, identity: true),
                        HoSoCaNhanId = c.Int(nullable: false),
                        TinTuyenDungId = c.Int(nullable: false),
                        CVUngVienId = c.Int(nullable: false),
                        NgayNop = c.DateTime(nullable: false),
                        TrangThaiDon = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DonUngTuyenId)
                .ForeignKey("dbo.TinTuyenDungs", t => t.TinTuyenDungId)
                .Index(t => new { t.TinTuyenDungId, t.HoSoCaNhanId }, unique: true, name: "IX_UniqueApply");
            
            CreateTable(
                "dbo.LichSuTrangThaiDons",
                c => new
                    {
                        LichSuTrangThaiDonId = c.Int(nullable: false, identity: true),
                        DonUngTuyenId = c.Int(nullable: false),
                        NguoiDungId = c.Int(nullable: false),
                        TrangThaiCu = c.String(),
                        TrangThaiMoi = c.String(),
                        GhiChu = c.String(),
                        ThoiGianThayDoi = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LichSuTrangThaiDonId)
                .ForeignKey("dbo.DonUngTuyens", t => t.DonUngTuyenId)
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDungId)
                .Index(t => t.DonUngTuyenId)
                .Index(t => t.NguoiDungId);
            
            CreateTable(
                "dbo.NhatKyDuyetTins",
                c => new
                    {
                        NhatKyDuyetTinId = c.Int(nullable: false, identity: true),
                        TinTuyenDungId = c.Int(nullable: false),
                        NguoiDungId = c.Int(nullable: false),
                        HanhDong = c.String(),
                        LyDoTuChoi = c.String(),
                        ThoiGianXuLy = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NhatKyDuyetTinId)
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDungId)
                .ForeignKey("dbo.TinTuyenDungs", t => t.TinTuyenDungId)
                .Index(t => t.TinTuyenDungId)
                .Index(t => t.NguoiDungId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NhatKyDuyetTins", "TinTuyenDungId", "dbo.TinTuyenDungs");
            DropForeignKey("dbo.NhatKyDuyetTins", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.LichSuTrangThaiDons", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.LichSuTrangThaiDons", "DonUngTuyenId", "dbo.DonUngTuyens");
            DropForeignKey("dbo.DonUngTuyens", "TinTuyenDungId", "dbo.TinTuyenDungs");
            DropForeignKey("dbo.TinTuyenDungs", "LoaiHinhLamViecId", "dbo.LoaiHinhLamViecs");
            DropForeignKey("dbo.TinTuyenDungs", "HoSoCongTyId", "dbo.HoSoCongTies");
            DropForeignKey("dbo.HoSoCongTies", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.NguoiDungs", "VaiTroId", "dbo.VaiTroes");
            DropForeignKey("dbo.HoSoCaNhans", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.CVUngViens", "HoSoCaNhanId", "dbo.HoSoCaNhans");
            DropForeignKey("dbo.TinTuyenDungs", "CapDoKinhNghiemId", "dbo.CapDoKinhNghiems");
            DropIndex("dbo.NhatKyDuyetTins", new[] { "NguoiDungId" });
            DropIndex("dbo.NhatKyDuyetTins", new[] { "TinTuyenDungId" });
            DropIndex("dbo.LichSuTrangThaiDons", new[] { "NguoiDungId" });
            DropIndex("dbo.LichSuTrangThaiDons", new[] { "DonUngTuyenId" });
            DropIndex("dbo.DonUngTuyens", "IX_UniqueApply");
            DropIndex("dbo.CVUngViens", new[] { "HoSoCaNhanId" });
            DropIndex("dbo.HoSoCaNhans", new[] { "NguoiDungId" });
            DropIndex("dbo.NguoiDungs", new[] { "VaiTroId" });
            DropIndex("dbo.NguoiDungs", new[] { "Email" });
            DropIndex("dbo.NguoiDungs", new[] { "TenDangNhap" });
            DropIndex("dbo.HoSoCongTies", new[] { "NguoiDungId" });
            DropIndex("dbo.TinTuyenDungs", new[] { "CapDoKinhNghiemId" });
            DropIndex("dbo.TinTuyenDungs", new[] { "LoaiHinhLamViecId" });
            DropIndex("dbo.TinTuyenDungs", new[] { "HoSoCongTyId" });
            DropTable("dbo.NhatKyDuyetTins");
            DropTable("dbo.LichSuTrangThaiDons");
            DropTable("dbo.DonUngTuyens");
            DropTable("dbo.LoaiHinhLamViecs");
            DropTable("dbo.VaiTroes");
            DropTable("dbo.CVUngViens");
            DropTable("dbo.HoSoCaNhans");
            DropTable("dbo.NguoiDungs");
            DropTable("dbo.HoSoCongTies");
            DropTable("dbo.TinTuyenDungs");
            DropTable("dbo.CapDoKinhNghiems");
        }
    }
}
