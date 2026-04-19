namespace WebsiteTuyenDung.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitIdentityCore : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CapDoKinhNghiem",
                c => new
                    {
                        CapDoKinhNghiemId = c.Int(nullable: false, identity: true),
                        TenCapDoKinhNghiem = c.String(nullable: false, maxLength: 100),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CapDoKinhNghiemId)
                .Index(t => t.TenCapDoKinhNghiem, unique: true, name: "IX_CapDoKinhNghiem_TenCapDoKinhNghiem");
            
            CreateTable(
                "dbo.TinTuyenDung",
                c => new
                    {
                        TinTuyenDungId = c.Int(nullable: false, identity: true),
                        HoSoCongTyId = c.Int(nullable: false),
                        NganhNgheId = c.Int(nullable: false),
                        DiaDiemId = c.Int(nullable: false),
                        LoaiHinhLamViecId = c.Int(nullable: false),
                        CapDoKinhNghiemId = c.Int(nullable: false),
                        TieuDe = c.String(nullable: false, maxLength: 200),
                        MoTaCongViec = c.String(nullable: false),
                        YeuCau = c.String(),
                        QuyenLoi = c.String(),
                        SoLuongTuyen = c.Int(nullable: false),
                        LuongToiThieu = c.Decimal(precision: 18, scale: 2),
                        LuongToiDa = c.Decimal(precision: 18, scale: 2),
                        NgayDang = c.DateTime(),
                        HanNopHoSo = c.DateTime(nullable: false),
                        TrangThaiTin = c.String(nullable: false, maxLength: 30),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TinTuyenDungId)
                .ForeignKey("dbo.CapDoKinhNghiem", t => t.CapDoKinhNghiemId)
                .ForeignKey("dbo.DiaDiem", t => t.DiaDiemId)
                .ForeignKey("dbo.HoSoCongTy", t => t.HoSoCongTyId)
                .ForeignKey("dbo.LoaiHinhLamViec", t => t.LoaiHinhLamViecId)
                .ForeignKey("dbo.NganhNghe", t => t.NganhNgheId)
                .Index(t => t.HoSoCongTyId)
                .Index(t => t.NganhNgheId)
                .Index(t => t.DiaDiemId)
                .Index(t => t.LoaiHinhLamViecId)
                .Index(t => t.CapDoKinhNghiemId);
            
            CreateTable(
                "dbo.DiaDiem",
                c => new
                    {
                        DiaDiemId = c.Int(nullable: false, identity: true),
                        TenDiaDiem = c.String(nullable: false, maxLength: 100),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DiaDiemId)
                .Index(t => t.TenDiaDiem, unique: true, name: "IX_DiaDiem_TenDiaDiem");
            
            CreateTable(
                "dbo.DonUngTuyen",
                c => new
                    {
                        DonUngTuyenId = c.Int(nullable: false, identity: true),
                        HoSoCaNhanId = c.Int(nullable: false),
                        TinTuyenDungId = c.Int(nullable: false),
                        CVUngVienId = c.Int(nullable: false),
                        NgayNop = c.DateTime(nullable: false),
                        TrangThaiDon = c.String(nullable: false, maxLength: 30),
                        ThuGioiThieu = c.String(),
                        GhiChuXuLy = c.String(),
                    })
                .PrimaryKey(t => t.DonUngTuyenId)
                .ForeignKey("dbo.CVUngVien", t => t.CVUngVienId)
                .ForeignKey("dbo.HoSoCaNhan", t => t.HoSoCaNhanId)
                .ForeignKey("dbo.TinTuyenDung", t => t.TinTuyenDungId)
                .Index(t => new { t.TinTuyenDungId, t.HoSoCaNhanId }, unique: true, name: "IX_DonUngTuyen_TinTuyenDung_HoSoCaNhan")
                .Index(t => t.CVUngVienId);
            
            CreateTable(
                "dbo.CVUngVien",
                c => new
                    {
                        CVUngVienId = c.Int(nullable: false, identity: true),
                        HoSoCaNhanId = c.Int(nullable: false),
                        TenCV = c.String(nullable: false, maxLength: 200),
                        DuongDanFile = c.String(nullable: false, maxLength: 255),
                        NgayTaiLen = c.DateTime(nullable: false),
                        TrangThaiSuDung = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CVUngVienId)
                .ForeignKey("dbo.HoSoCaNhan", t => t.HoSoCaNhanId)
                .Index(t => t.HoSoCaNhanId);
            
            CreateTable(
                "dbo.HoSoCaNhan",
                c => new
                    {
                        HoSoCaNhanId = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        HoTen = c.String(nullable: false, maxLength: 150),
                        NgaySinh = c.DateTime(storeType: "date"),
                        GioiTinh = c.String(maxLength: 20),
                        SoDienThoai = c.String(maxLength: 20),
                        DiaChi = c.String(maxLength: 300),
                        MucTieuNgheNghiep = c.String(),
                        HocVan = c.String(),
                        TomTatKinhNghiem = c.String(),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.HoSoCaNhanId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId, unique: true, name: "IX_HoSoCaNhan_ApplicationUserId");
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.LichSuTrangThaiDon",
                c => new
                    {
                        LichSuTrangThaiDonId = c.Int(nullable: false, identity: true),
                        DonUngTuyenId = c.Int(nullable: false),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        TrangThaiCu = c.String(maxLength: 30),
                        TrangThaiMoi = c.String(maxLength: 30),
                        GhiChu = c.String(),
                        ThoiGianThayDoi = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LichSuTrangThaiDonId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.DonUngTuyen", t => t.DonUngTuyenId)
                .Index(t => t.DonUngTuyenId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.HoSoCongTy",
                c => new
                    {
                        HoSoCongTyId = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        TenCongTy = c.String(nullable: false, maxLength: 200),
                        MaSoThue = c.String(maxLength: 50),
                        MoTa = c.String(),
                        DiaChi = c.String(maxLength: 300),
                        Website = c.String(maxLength: 255),
                        Logo = c.String(maxLength: 255),
                        EmailLienHe = c.String(maxLength: 256),
                        SoDienThoaiLienHe = c.String(maxLength: 20),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.HoSoCongTyId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId, unique: true, name: "IX_HoSoCongTy_ApplicationUserId");
            
            CreateTable(
                "dbo.LoaiHinhLamViec",
                c => new
                    {
                        LoaiHinhLamViecId = c.Int(nullable: false, identity: true),
                        TenLoaiHinhLamViec = c.String(nullable: false, maxLength: 100),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LoaiHinhLamViecId)
                .Index(t => t.TenLoaiHinhLamViec, unique: true, name: "IX_LoaiHinhLamViec_TenLoaiHinhLamViec");
            
            CreateTable(
                "dbo.NganhNghe",
                c => new
                    {
                        NganhNgheId = c.Int(nullable: false, identity: true),
                        TenNganhNghe = c.String(nullable: false, maxLength: 100),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.NganhNgheId)
                .Index(t => t.TenNganhNghe, unique: true, name: "IX_NganhNghe_TenNganhNghe");
            
            CreateTable(
                "dbo.NhatKyDuyetTin",
                c => new
                    {
                        NhatKyDuyetTinId = c.Int(nullable: false, identity: true),
                        TinTuyenDungId = c.Int(nullable: false),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        HanhDong = c.String(nullable: false, maxLength: 30),
                        LyDoTuChoi = c.String(),
                        ThoiGianXuLy = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NhatKyDuyetTinId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.TinTuyenDung", t => t.TinTuyenDungId)
                .Index(t => t.TinTuyenDungId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.NhatKyDuyetTin", "TinTuyenDungId", "dbo.TinTuyenDung");
            DropForeignKey("dbo.NhatKyDuyetTin", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TinTuyenDung", "NganhNgheId", "dbo.NganhNghe");
            DropForeignKey("dbo.TinTuyenDung", "LoaiHinhLamViecId", "dbo.LoaiHinhLamViec");
            DropForeignKey("dbo.TinTuyenDung", "HoSoCongTyId", "dbo.HoSoCongTy");
            DropForeignKey("dbo.HoSoCongTy", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DonUngTuyen", "TinTuyenDungId", "dbo.TinTuyenDung");
            DropForeignKey("dbo.LichSuTrangThaiDon", "DonUngTuyenId", "dbo.DonUngTuyen");
            DropForeignKey("dbo.LichSuTrangThaiDon", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DonUngTuyen", "HoSoCaNhanId", "dbo.HoSoCaNhan");
            DropForeignKey("dbo.CVUngVien", "HoSoCaNhanId", "dbo.HoSoCaNhan");
            DropForeignKey("dbo.HoSoCaNhan", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DonUngTuyen", "CVUngVienId", "dbo.CVUngVien");
            DropForeignKey("dbo.TinTuyenDung", "DiaDiemId", "dbo.DiaDiem");
            DropForeignKey("dbo.TinTuyenDung", "CapDoKinhNghiemId", "dbo.CapDoKinhNghiem");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.NhatKyDuyetTin", new[] { "ApplicationUserId" });
            DropIndex("dbo.NhatKyDuyetTin", new[] { "TinTuyenDungId" });
            DropIndex("dbo.NganhNghe", "IX_NganhNghe_TenNganhNghe");
            DropIndex("dbo.LoaiHinhLamViec", "IX_LoaiHinhLamViec_TenLoaiHinhLamViec");
            DropIndex("dbo.HoSoCongTy", "IX_HoSoCongTy_ApplicationUserId");
            DropIndex("dbo.LichSuTrangThaiDon", new[] { "ApplicationUserId" });
            DropIndex("dbo.LichSuTrangThaiDon", new[] { "DonUngTuyenId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.HoSoCaNhan", "IX_HoSoCaNhan_ApplicationUserId");
            DropIndex("dbo.CVUngVien", new[] { "HoSoCaNhanId" });
            DropIndex("dbo.DonUngTuyen", new[] { "CVUngVienId" });
            DropIndex("dbo.DonUngTuyen", "IX_DonUngTuyen_TinTuyenDung_HoSoCaNhan");
            DropIndex("dbo.DiaDiem", "IX_DiaDiem_TenDiaDiem");
            DropIndex("dbo.TinTuyenDung", new[] { "CapDoKinhNghiemId" });
            DropIndex("dbo.TinTuyenDung", new[] { "LoaiHinhLamViecId" });
            DropIndex("dbo.TinTuyenDung", new[] { "DiaDiemId" });
            DropIndex("dbo.TinTuyenDung", new[] { "NganhNgheId" });
            DropIndex("dbo.TinTuyenDung", new[] { "HoSoCongTyId" });
            DropIndex("dbo.CapDoKinhNghiem", "IX_CapDoKinhNghiem_TenCapDoKinhNghiem");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.NhatKyDuyetTin");
            DropTable("dbo.NganhNghe");
            DropTable("dbo.LoaiHinhLamViec");
            DropTable("dbo.HoSoCongTy");
            DropTable("dbo.LichSuTrangThaiDon");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.HoSoCaNhan");
            DropTable("dbo.CVUngVien");
            DropTable("dbo.DonUngTuyen");
            DropTable("dbo.DiaDiem");
            DropTable("dbo.TinTuyenDung");
            DropTable("dbo.CapDoKinhNghiem");
        }
    }
}
