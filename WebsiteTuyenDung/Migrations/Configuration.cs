namespace WebsiteTuyenDung.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using WebsiteTuyenDung.Models;
    using WebsiteTuyenDung.Models.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<WebsiteTuyenDung.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebsiteTuyenDung.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonLetterOrDigit = true
            };

            EnsureRole(roleManager, "Admin");
            EnsureRole(roleManager, "NhaTuyenDung");
            EnsureRole(roleManager, "UngVien");

            var adminUser = EnsureUser(userManager, "admin@test.com", "123456Aa@", "Admin");
            var employerUser = EnsureUser(userManager, "employer@test.com", "123456Aa@", "NhaTuyenDung");
            var candidateUser = EnsureUser(userManager, "ungvien@test.com", "123456Aa@", "UngVien");

            SeedDanhMuc(context);
            context.SaveChanges();

            var hoSoCongTy = EnsureEmployerProfile(context, employerUser.Id);
            var hoSoCaNhan = EnsureCandidateProfile(context, candidateUser.Id);
            context.SaveChanges();

            var cvMacDinh = EnsureDefaultCv(context, hoSoCaNhan.HoSoCaNhanId);
            context.SaveChanges();

            SeedTinMau(context, hoSoCongTy.HoSoCongTyId, adminUser.Id, employerUser.Id);
            context.SaveChanges();

            SeedDonMau(context, candidateUser.Id, hoSoCaNhan.HoSoCaNhanId, cvMacDinh.CVUngVienId, hoSoCongTy.HoSoCongTyId);
            context.SaveChanges();
        }

        private void EnsureRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }

        private ApplicationUser EnsureUser(UserManager<ApplicationUser> userManager, string email, string password, string roleName)
        {
            var user = userManager.FindByEmail(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email
                };

                var result = userManager.Create(user, password);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(string.Join("; ", result.Errors));
                }
            }

            if (!userManager.IsInRole(user.Id, roleName))
            {
                userManager.AddToRole(user.Id, roleName);
            }

            if (!user.LockoutEnabled)
            {
                userManager.SetLockoutEnabled(user.Id, true);
            }

            return user;
        }

        private void SeedDanhMuc(ApplicationDbContext context)
        {
            context.NganhNghes.AddOrUpdate(x => x.TenNganhNghe,
                new NganhNghe { TenNganhNghe = "Công nghệ thông tin", TrangThai = true },
                new NganhNghe { TenNganhNghe = "Kinh doanh / Bán hàng", TrangThai = true },
                new NganhNghe { TenNganhNghe = "Marketing", TrangThai = true });

            context.DiaDiems.AddOrUpdate(x => x.TenDiaDiem,
                new DiaDiem { TenDiaDiem = "TP. Hồ Chí Minh", TrangThai = true },
                new DiaDiem { TenDiaDiem = "Hà Nội", TrangThai = true },
                new DiaDiem { TenDiaDiem = "Đà Nẵng", TrangThai = true });

            context.LoaiHinhLamViecs.AddOrUpdate(x => x.TenLoaiHinhLamViec,
                new LoaiHinhLamViec { TenLoaiHinhLamViec = "Toàn thời gian", TrangThai = true },
                new LoaiHinhLamViec { TenLoaiHinhLamViec = "Bán thời gian", TrangThai = true },
                new LoaiHinhLamViec { TenLoaiHinhLamViec = "Hybrid", TrangThai = true },
                new LoaiHinhLamViec { TenLoaiHinhLamViec = "Remote", TrangThai = true });

            context.CapDoKinhNghiems.AddOrUpdate(x => x.TenCapDoKinhNghiem,
                new CapDoKinhNghiem { TenCapDoKinhNghiem = "Thực tập sinh", TrangThai = true },
                new CapDoKinhNghiem { TenCapDoKinhNghiem = "Mới tốt nghiệp", TrangThai = true },
                new CapDoKinhNghiem { TenCapDoKinhNghiem = "1-2 năm", TrangThai = true },
                new CapDoKinhNghiem { TenCapDoKinhNghiem = "3-5 năm", TrangThai = true });
        }

        private HoSoCongTy EnsureEmployerProfile(ApplicationDbContext context, string employerUserId)
        {
            var hoSoCongTy = context.HoSoCongTys.FirstOrDefault(x => x.ApplicationUserId == employerUserId);
            if (hoSoCongTy != null)
            {
                return hoSoCongTy;
            }

            hoSoCongTy = new HoSoCongTy
            {
                ApplicationUserId = employerUserId,
                TenCongTy = "Công ty TNHH ACME Việt Nam",
                MaSoThue = "0312345678",
                MoTa = "Doanh nghiệp công nghệ tập trung vào phát triển sản phẩm web, phần mềm doanh nghiệp và trải nghiệm tuyển dụng số.",
                DiaChi = "Quận 1, TP. Hồ Chí Minh",
                Website = "https://acme.example.com",
                Logo = "https://dummyimage.com/240x240/0f6cbf/ffffff.png&text=ACME",
                EmailLienHe = "hr@acme.example.com",
                SoDienThoaiLienHe = "0909123456",
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            context.HoSoCongTys.Add(hoSoCongTy);
            context.SaveChanges();
            return hoSoCongTy;
        }

        private HoSoCaNhan EnsureCandidateProfile(ApplicationDbContext context, string candidateUserId)
        {
            var hoSoCaNhan = context.HoSoCaNhans.FirstOrDefault(x => x.ApplicationUserId == candidateUserId);
            if (hoSoCaNhan != null)
            {
                return hoSoCaNhan;
            }

            hoSoCaNhan = new HoSoCaNhan
            {
                ApplicationUserId = candidateUserId,
                HoTen = "Nguyễn Văn A",
                NgaySinh = new DateTime(2001, 5, 12),
                GioiTinh = "Nam",
                SoDienThoai = "0912345678",
                DiaChi = "Thành phố Thủ Đức, TP. Hồ Chí Minh",
                MucTieuNgheNghiep = "Phát triển sự nghiệp ở vị trí .NET Developer và tham gia các dự án thực tế có quy trình bài bản.",
                HocVan = "Cử nhân Công nghệ thông tin - Đại học Công nghệ TP. Hồ Chí Minh",
                TomTatKinhNghiem = "Đã thực tập 6 tháng ở vị trí backend .NET, có kinh nghiệm với ASP.NET MVC, Entity Framework và SQL Server.",
                NgayCapNhat = DateTime.Now
            };

            context.HoSoCaNhans.Add(hoSoCaNhan);
            context.SaveChanges();
            return hoSoCaNhan;
        }

        private CVUngVien EnsureDefaultCv(ApplicationDbContext context, int hoSoCaNhanId)
        {
            var dsCv = context.CVUngViens.Where(x => x.HoSoCaNhanId == hoSoCaNhanId).OrderByDescending(x => x.TrangThaiSuDung).ThenByDescending(x => x.NgayTaiLen).ToList();
            if (!dsCv.Any())
            {
                var cv = new CVUngVien
                {
                    HoSoCaNhanId = hoSoCaNhanId,
                    TenCV = "CV Backend .NET",
                    DuongDanFile = "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
                    NgayTaiLen = DateTime.Now,
                    TrangThaiSuDung = true
                };

                context.CVUngViens.Add(cv);
                context.SaveChanges();
                return cv;
            }

            var cvMacDinh = dsCv.FirstOrDefault(x => x.TrangThaiSuDung);
            if (cvMacDinh != null)
            {
                return cvMacDinh;
            }

            cvMacDinh = dsCv.First();
            cvMacDinh.TrangThaiSuDung = true;
            context.SaveChanges();
            return cvMacDinh;
        }

        private void SeedTinMau(ApplicationDbContext context, int hoSoCongTyId, string adminUserId, string employerUserId)
        {
            var nganhCongNghe = context.NganhNghes.FirstOrDefault(x => x.TenNganhNghe == "Công nghệ thông tin") ?? context.NganhNghes.FirstOrDefault();
            var nganhMarketing = context.NganhNghes.FirstOrDefault(x => x.TenNganhNghe == "Marketing") ?? context.NganhNghes.FirstOrDefault();
            var diaDiemHcm = context.DiaDiems.FirstOrDefault(x => x.TenDiaDiem == "TP. Hồ Chí Minh") ?? context.DiaDiems.FirstOrDefault();
            var loaiFullTime = context.LoaiHinhLamViecs.FirstOrDefault(x => x.TenLoaiHinhLamViec == "Toàn thời gian") ?? context.LoaiHinhLamViecs.FirstOrDefault();
            var loaiHybrid = context.LoaiHinhLamViecs.FirstOrDefault(x => x.TenLoaiHinhLamViec == "Hybrid") ?? context.LoaiHinhLamViecs.FirstOrDefault();
            var capDoIntern = context.CapDoKinhNghiems.FirstOrDefault(x => x.TenCapDoKinhNghiem == "Thực tập sinh") ?? context.CapDoKinhNghiems.FirstOrDefault();
            var capDoJunior = context.CapDoKinhNghiems.FirstOrDefault(x => x.TenCapDoKinhNghiem == "1-2 năm") ?? context.CapDoKinhNghiems.FirstOrDefault();

            if (nganhCongNghe == null || diaDiemHcm == null || loaiFullTime == null || capDoIntern == null)
            {
                return;
            }

            var tinDaDuyet = EnsureTin(context, hoSoCongTyId, "Thực tập sinh .NET", "DaDuyet", nganhCongNghe.NganhNgheId, diaDiemHcm.DiaDiemId, loaiFullTime.LoaiHinhLamViecId, capDoIntern.CapDoKinhNghiemId, DateTime.Now.AddDays(-3), DateTime.Today.AddDays(20), 2, 4000000m, 7000000m, "Tham gia phát triển và bảo trì tính năng cho hệ thống tuyển dụng sử dụng ASP.NET MVC và SQL Server.", "Có kiến thức cơ bản về C#, HTML, CSS và SQL. Chủ động học hỏi, giao tiếp tốt.", "Được mentoring trực tiếp, hỗ trợ dấu mộc thực tập, phụ cấp theo năng lực.");
            EnsureReviewLog(context, tinDaDuyet.TinTuyenDungId, employerUserId, "GuiDuyet", null, tinDaDuyet.NgayTao.AddHours(2));
            EnsureReviewLog(context, tinDaDuyet.TinTuyenDungId, adminUserId, "Duyet", null, tinDaDuyet.NgayTao.AddHours(5));

            var tinChoDuyet = EnsureTin(context, hoSoCongTyId, "Nhân viên QA Manual", "ChoDuyet", nganhCongNghe.NganhNgheId, diaDiemHcm.DiaDiemId, loaiHybrid.LoaiHinhLamViecId, capDoJunior.CapDoKinhNghiemId, null, DateTime.Today.AddDays(15), 1, 9000000m, 12000000m, "Kiểm thử chức năng web tuyển dụng, viết test case và phối hợp với nhóm phát triển để xử lý lỗi.", "Có từ 1 năm kinh nghiệm kiểm thử thủ công, nắm quy trình test và biết viết bug report rõ ràng.", "Môi trường làm việc hybrid, review lương định kỳ, có lộ trình lên QA Lead.");
            EnsureReviewLog(context, tinChoDuyet.TinTuyenDungId, employerUserId, "GuiDuyet", null, tinChoDuyet.NgayTao.AddHours(1));

            var tinBiTuChoi = EnsureTin(context, hoSoCongTyId, "Nhân viên Marketing Nội dung", "BiTuChoi", nganhMarketing != null ? nganhMarketing.NganhNgheId : nganhCongNghe.NganhNgheId, diaDiemHcm.DiaDiemId, loaiFullTime.LoaiHinhLamViecId, capDoJunior.CapDoKinhNghiemId, null, DateTime.Today.AddDays(10), 1, 8000000m, 10000000m, "Lên kế hoạch nội dung cho fanpage, website tuyển dụng và các chiến dịch thu hút ứng viên.", "Có kinh nghiệm viết nội dung tuyển dụng và tối ưu SEO cơ bản.", "Được cấp ngân sách đào tạo và tham gia các chiến dịch employer branding.");
            EnsureReviewLog(context, tinBiTuChoi.TinTuyenDungId, employerUserId, "GuiDuyet", null, tinBiTuChoi.NgayTao.AddHours(1));
            EnsureReviewLog(context, tinBiTuChoi.TinTuyenDungId, adminUserId, "TuChoi", "Nội dung mô tả chưa nêu rõ KPI công việc và thông tin phúc lợi còn quá chung chung.", tinBiTuChoi.NgayTao.AddHours(3));
        }

        private TinTuyenDung EnsureTin(ApplicationDbContext context, int hoSoCongTyId, string tieuDe, string trangThaiTin, int nganhNgheId, int diaDiemId, int loaiHinhLamViecId, int capDoKinhNghiemId, DateTime? ngayDang, DateTime hanNopHoSo, int soLuongTuyen, decimal? luongToiThieu, decimal? luongToiDa, string moTaCongViec, string yeuCau, string quyenLoi)
        {
            var tin = context.TinTuyenDungs.FirstOrDefault(x => x.HoSoCongTyId == hoSoCongTyId && x.TieuDe == tieuDe);
            if (tin != null)
            {
                return tin;
            }

            tin = new TinTuyenDung
            {
                HoSoCongTyId = hoSoCongTyId,
                NganhNgheId = nganhNgheId,
                DiaDiemId = diaDiemId,
                LoaiHinhLamViecId = loaiHinhLamViecId,
                CapDoKinhNghiemId = capDoKinhNghiemId,
                TieuDe = tieuDe,
                MoTaCongViec = moTaCongViec,
                YeuCau = yeuCau,
                QuyenLoi = quyenLoi,
                SoLuongTuyen = soLuongTuyen,
                LuongToiThieu = luongToiThieu,
                LuongToiDa = luongToiDa,
                NgayDang = ngayDang,
                HanNopHoSo = hanNopHoSo,
                TrangThaiTin = trangThaiTin,
                NgayTao = DateTime.Now.AddDays(-5),
                NgayCapNhat = DateTime.Now.AddDays(-4)
            };

            context.TinTuyenDungs.Add(tin);
            context.SaveChanges();
            return tin;
        }

        private void EnsureReviewLog(ApplicationDbContext context, int tinTuyenDungId, string applicationUserId, string hanhDong, string lyDoTuChoi, DateTime thoiGianXuLy)
        {
            var daTonTai = context.NhatKyDuyetTins.Any(x => x.TinTuyenDungId == tinTuyenDungId && x.HanhDong == hanhDong && x.ApplicationUserId == applicationUserId);
            if (daTonTai)
            {
                return;
            }

            context.NhatKyDuyetTins.Add(new NhatKyDuyetTin
            {
                TinTuyenDungId = tinTuyenDungId,
                ApplicationUserId = applicationUserId,
                HanhDong = hanhDong,
                LyDoTuChoi = lyDoTuChoi,
                ThoiGianXuLy = thoiGianXuLy
            });
        }

        private void SeedDonMau(ApplicationDbContext context, string candidateUserId, int hoSoCaNhanId, int cvUngVienId, int hoSoCongTyId)
        {
            var tinDaDuyet = context.TinTuyenDungs.FirstOrDefault(x => x.HoSoCongTyId == hoSoCongTyId && x.TieuDe == "Thực tập sinh .NET");
            if (tinDaDuyet == null)
            {
                return;
            }

            var don = context.DonUngTuyens.FirstOrDefault(x => x.HoSoCaNhanId == hoSoCaNhanId && x.TinTuyenDungId == tinDaDuyet.TinTuyenDungId);
            if (don == null)
            {
                don = new DonUngTuyen
                {
                    HoSoCaNhanId = hoSoCaNhanId,
                    TinTuyenDungId = tinDaDuyet.TinTuyenDungId,
                    CVUngVienId = cvUngVienId,
                    NgayNop = DateTime.Now.AddDays(-1),
                    TrangThaiDon = "DaNop",
                    ThuGioiThieu = "Em mong muốn được tham gia vị trí thực tập sinh .NET để phát triển kỹ năng backend và học hỏi quy trình làm việc thực tế."
                };

                context.DonUngTuyens.Add(don);
                context.SaveChanges();
            }

            var daCoLichSu = context.LichSuTrangThaiDons.Any(x => x.DonUngTuyenId == don.DonUngTuyenId && x.TrangThaiMoi == "DaNop");
            if (!daCoLichSu)
            {
                context.LichSuTrangThaiDons.Add(new LichSuTrangThaiDon
                {
                    DonUngTuyenId = don.DonUngTuyenId,
                    ApplicationUserId = candidateUserId,
                    TrangThaiMoi = "DaNop",
                    GhiChu = "Ứng viên đã nộp hồ sơ từ tài khoản mẫu.",
                    ThoiGianThayDoi = don.NgayNop
                });
            }
        }
    }
}
