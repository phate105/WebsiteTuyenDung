namespace WebsiteTuyenDung.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using WebsiteTuyenDung.Models;
    using WebsiteTuyenDung.Models.Constants;
    using WebsiteTuyenDung.Models.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<WebsiteTuyenDung.Models.ApplicationDbContext>
    {
        private const string SeedPassword = "123456Aa@";

        private const string IndustryFinance = "Tài chính/Ngân hàng";
        private const string IndustryAccounting = "Kế toán/Kiểm toán";
        private const string IndustryOffice = "Hành chính/Văn phòng";
        private const string IndustrySales = "Kinh doanh/Bán hàng";
        private const string IndustryMarketing = "Marketing/Quảng cáo";
        private const string IndustryConstruction = "Xây dựng/Kiến trúc";
        private const string IndustryIt = "Công nghệ Thông tin";
        private const string IndustryHr = "Nhân sự";
        private const string IndustryManufacturing = "Sản xuất/Kỹ thuật";
        private const string IndustryCustomerService = "Chăm sóc khách hàng";

        private const string LocationHcm = "Hồ Chí Minh";
        private const string LocationHaNoi = "Hà Nội";
        private const string LocationDaNang = "Đà Nẵng";
        private const string LocationCanTho = "Cần Thơ";
        private const string LocationBinhDuong = "Bình Dương";
        private const string LocationHaiPhong = "Hải Phòng";
        private const string LocationDongNai = "Đồng Nai";
        private const string LocationQuangNinh = "Quảng Ninh";

        private const string WorkFullTime = "Toàn thời gian";
        private const string WorkPartTime = "Bán thời gian";
        private const string WorkRemote = "Remote";
        private const string WorkHybrid = "Hybrid";
        private const string WorkIntern = "Thực tập";
        private const string WorkSeasonal = "Thời vụ";

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

            EnsureRole(roleManager, ApplicationRoles.Admin);
            EnsureRole(roleManager, ApplicationRoles.NhaTuyenDung);
            EnsureRole(roleManager, ApplicationRoles.UngVien);

            var adminUser = EnsureUser(userManager, "admin@test.com", SeedPassword, ApplicationRoles.Admin);

            SeedDanhMuc(context);
            var employers = SeedEmployers(context, userManager);
            var candidates = SeedCandidates(context, userManager);
            var jobs = SeedJobs(context, employers, adminUser.Id);
            SeedApplications(context, candidates, jobs, employers);
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
                var roleResult = userManager.AddToRole(user.Id, roleName);
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join("; ", roleResult.Errors));
                }
            }

            if (!user.LockoutEnabled)
            {
                userManager.SetLockoutEnabled(user.Id, true);
            }

            return user;
        }

        private void SeedDanhMuc(ApplicationDbContext context)
        {
            EnsureNganhNghe(context, IndustryFinance);
            EnsureNganhNghe(context, IndustryAccounting);
            EnsureNganhNghe(context, IndustryOffice);
            EnsureNganhNghe(context, IndustrySales, "Kinh doanh / Bán hàng");
            EnsureNganhNghe(context, IndustryMarketing, "Marketing");
            EnsureNganhNghe(context, IndustryConstruction);
            EnsureNganhNghe(context, IndustryIt, "Công nghệ thông tin");
            EnsureNganhNghe(context, IndustryHr);
            EnsureNganhNghe(context, IndustryManufacturing);
            EnsureNganhNghe(context, IndustryCustomerService);

            EnsureDiaDiem(context, LocationHcm, "TP. Hồ Chí Minh", "Thành phố Hồ Chí Minh");
            EnsureDiaDiem(context, LocationHaNoi);
            EnsureDiaDiem(context, LocationDaNang);
            EnsureDiaDiem(context, LocationCanTho);
            EnsureDiaDiem(context, LocationBinhDuong);
            EnsureDiaDiem(context, LocationHaiPhong);
            EnsureDiaDiem(context, LocationDongNai);
            EnsureDiaDiem(context, LocationQuangNinh);

            EnsureLoaiHinh(context, WorkFullTime);
            EnsureLoaiHinh(context, WorkPartTime);
            EnsureLoaiHinh(context, WorkRemote);
            EnsureLoaiHinh(context, WorkHybrid);
            EnsureLoaiHinh(context, WorkIntern);
            EnsureLoaiHinh(context, WorkSeasonal);

            EnsureCapDo(context, "Không yêu cầu", "Thực tập sinh");
            EnsureCapDo(context, "Fresher", "Mới tốt nghiệp");
            EnsureCapDo(context, "Junior", "1-2 năm");
            EnsureCapDo(context, "Senior", "3-5 năm");
            EnsureCapDo(context, "Trưởng nhóm");
        }

        private NganhNghe EnsureNganhNghe(ApplicationDbContext context, string tenNganhNghe, params string[] aliases)
        {
            var item = context.NganhNghes.FirstOrDefault(x => x.TenNganhNghe == tenNganhNghe);
            if (item == null)
            {
                item = FindNganhNgheByAliases(context, aliases);
            }

            if (item == null)
            {
                item = new NganhNghe();
                context.NganhNghes.Add(item);
            }

            item.TenNganhNghe = tenNganhNghe;
            item.TrangThai = true;
            context.SaveChanges();
            return item;
        }

        private NganhNghe FindNganhNgheByAliases(ApplicationDbContext context, IEnumerable<string> aliases)
        {
            if (aliases == null)
            {
                return null;
            }

            foreach (var alias in aliases)
            {
                var item = context.NganhNghes.FirstOrDefault(x => x.TenNganhNghe == alias);
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        private DiaDiem EnsureDiaDiem(ApplicationDbContext context, string tenDiaDiem, params string[] aliases)
        {
            var item = context.DiaDiems.FirstOrDefault(x => x.TenDiaDiem == tenDiaDiem);
            if (item == null)
            {
                item = FindDiaDiemByAliases(context, aliases);
            }

            if (item == null)
            {
                item = new DiaDiem();
                context.DiaDiems.Add(item);
            }

            item.TenDiaDiem = tenDiaDiem;
            item.TrangThai = true;
            context.SaveChanges();
            return item;
        }

        private DiaDiem FindDiaDiemByAliases(ApplicationDbContext context, IEnumerable<string> aliases)
        {
            if (aliases == null)
            {
                return null;
            }

            foreach (var alias in aliases)
            {
                var item = context.DiaDiems.FirstOrDefault(x => x.TenDiaDiem == alias);
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        private LoaiHinhLamViec EnsureLoaiHinh(ApplicationDbContext context, string tenLoaiHinh, params string[] aliases)
        {
            var item = context.LoaiHinhLamViecs.FirstOrDefault(x => x.TenLoaiHinhLamViec == tenLoaiHinh);
            if (item == null)
            {
                item = FindLoaiHinhByAliases(context, aliases);
            }

            if (item == null)
            {
                item = new LoaiHinhLamViec();
                context.LoaiHinhLamViecs.Add(item);
            }

            item.TenLoaiHinhLamViec = tenLoaiHinh;
            item.TrangThai = true;
            context.SaveChanges();
            return item;
        }

        private LoaiHinhLamViec FindLoaiHinhByAliases(ApplicationDbContext context, IEnumerable<string> aliases)
        {
            if (aliases == null)
            {
                return null;
            }

            foreach (var alias in aliases)
            {
                var item = context.LoaiHinhLamViecs.FirstOrDefault(x => x.TenLoaiHinhLamViec == alias);
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        private CapDoKinhNghiem EnsureCapDo(ApplicationDbContext context, string tenCapDo, params string[] aliases)
        {
            var item = context.CapDoKinhNghiems.FirstOrDefault(x => x.TenCapDoKinhNghiem == tenCapDo);
            if (item == null)
            {
                item = FindCapDoByAliases(context, aliases);
            }

            if (item == null)
            {
                item = new CapDoKinhNghiem();
                context.CapDoKinhNghiems.Add(item);
            }

            item.TenCapDoKinhNghiem = tenCapDo;
            item.TrangThai = true;
            context.SaveChanges();
            return item;
        }

        private CapDoKinhNghiem FindCapDoByAliases(ApplicationDbContext context, IEnumerable<string> aliases)
        {
            if (aliases == null)
            {
                return null;
            }

            foreach (var alias in aliases)
            {
                var item = context.CapDoKinhNghiems.FirstOrDefault(x => x.TenCapDoKinhNghiem == alias);
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        private List<HoSoCongTy> SeedEmployers(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var result = new List<HoSoCongTy>();
            var seeds = BuildCuratedCompanySeeds();

            for (var i = 0; i < seeds.Count; i++)
            {
                var seed = seeds[i];
                var user = EnsureUser(userManager, seed.LoginEmail, SeedPassword, ApplicationRoles.NhaTuyenDung);
                var profile = context.HoSoCongTys.FirstOrDefault(x => x.ApplicationUserId == user.Id);
                if (profile == null)
                {
                    profile = new HoSoCongTy
                    {
                        ApplicationUserId = user.Id,
                        NgayTao = DateTime.Now.AddDays(-(90 + i))
                    };
                    context.HoSoCongTys.Add(profile);
                }

                profile.TenCongTy = seed.TenCongTy;
                profile.MaSoThue = string.Format("03{0:00000000}", 10000000 + i);
                profile.MoTa = seed.MoTa;
                profile.DiaChi = seed.DiaChi;
                profile.Website = string.IsNullOrWhiteSpace(seed.Website) ? string.Format("https://nha-tuyen-dung-{0:00}.vietjob.example.com", i + 1) : seed.Website;
                profile.Logo = string.IsNullOrWhiteSpace(seed.LogoPath)
                    ? ResolveEmployerLogoPath(seed.TenCongTy, i)
                    : seed.LogoPath;
                profile.EmailLienHe = string.Format("hr{0:00}@seed.vietjob.com", i + 1);
                profile.SoDienThoaiLienHe = string.Format("09{0:00000000}", 10000000 + i);
                profile.NgayCapNhat = DateTime.Now.AddDays(-(i % 7));

                context.SaveChanges();
                result.Add(profile);
            }

            return result;
        }

        private List<CompanySeed> BuildCuratedCompanySeeds()
        {
            return new List<CompanySeed>
            {
                new CompanySeed("employer@test.com", "Viettel Telecom", "Tập đoàn viễn thông và công nghệ số tuyển dụng cho hạ tầng mạng, bảo mật, kinh doanh doanh nghiệp và giải pháp số.", "Tòa nhà Viettel, Cầu Giấy, Hà Nội", "https://vietteltelecom.vn", IndustryIt, "/Content/Jobentry/img/employers/Viettel-logo.png"),
                new CompanySeed("employer01@seed.vietjob.com", "FPT Software Vietnam", "Công ty công nghệ cung cấp dịch vụ chuyển đổi số, phần mềm doanh nghiệp, cloud, AI và kiểm thử cho thị trường toàn cầu.", "Khu Công nghệ cao, TP. Thủ Đức, Hồ Chí Minh", "https://fptsoftware.com", IndustryIt, "/Content/Jobentry/img/employers/FPT-logo.png"),
                new CompanySeed("employer02@seed.vietjob.com", "Vinamilk", "Doanh nghiệp FMCG hàng đầu trong ngành sữa, tuyển dụng các vị trí bán hàng, trade marketing, chuỗi cung ứng và sản xuất.", "Quận 7, Hồ Chí Minh", "https://vinamilk.com.vn", IndustrySales, "/Content/Jobentry/img/employers/Vinamilk-logo.png"),
                new CompanySeed("employer03@seed.vietjob.com", "Techcombank", "Ngân hàng thương mại tập trung vào ngân hàng số, dữ liệu khách hàng, vận hành giao dịch và phân tích tài chính.", "Bà Triệu, Hoàn Kiếm, Hà Nội", "https://techcombank.com", IndustryFinance, "/Content/Jobentry/img/employers/techcombank2.jpg"),
                new CompanySeed("employer04@seed.vietjob.com", "VietinBank", "Ngân hàng với nhu cầu tuyển dụng giao dịch viên, quan hệ khách hàng, phân tích tài chính, vận hành và dữ liệu.", "Trần Hưng Đạo, Hoàn Kiếm, Hà Nội", "https://vietinbank.vn", IndustryFinance, "/Content/Jobentry/img/employers/Vietinbank-logo.png"),
                new CompanySeed("employer05@seed.vietjob.com", "Agribank", "Ngân hàng thương mại tuyển dụng cho mạng lưới chi nhánh, tín dụng, kế toán, quản trị rủi ro và dịch vụ khách hàng.", "Láng Hạ, Đống Đa, Hà Nội", "https://agribank.com.vn", IndustryFinance, "/Content/Jobentry/img/employers/Agribank-logo.png"),
                new CompanySeed("employer06@seed.vietjob.com", "SHB", "Ngân hàng phát triển các vị trí tài chính, quan hệ khách hàng, vận hành ngân hàng số và phân tích dữ liệu.", "Hoàn Kiếm, Hà Nội", "https://shb.com.vn", IndustryFinance, "/Content/Jobentry/img/employers/SHB-logo.png"),
                new CompanySeed("employer07@seed.vietjob.com", "MobiFone", "Nhà mạng viễn thông tuyển dụng kỹ sư mạng, chăm sóc khách hàng, kinh doanh doanh nghiệp và giải pháp số.", "Cầu Giấy, Hà Nội", "https://mobifone.vn", IndustryCustomerService, "/Content/Jobentry/img/employers/Mobifone-logo.png"),
                new CompanySeed("employer08@seed.vietjob.com", "VinaPhone", "Đơn vị viễn thông với nhu cầu tuyển dụng hạ tầng mạng, kinh doanh, chăm sóc khách hàng và chuyển đổi số.", "Nam Từ Liêm, Hà Nội", "https://vinaphone.com.vn", IndustryCustomerService, "/Content/Jobentry/img/employers/Vinaphone-logo.png"),
                new CompanySeed("employer09@seed.vietjob.com", "LG Electronics Vietnam", "Doanh nghiệp điện tử tuyển dụng kỹ sư sản xuất, QA, chuỗi cung ứng, bảo trì và kinh doanh thiết bị.", "Hải Phòng", "https://www.lg.com/vn", IndustryManufacturing, "/Content/Jobentry/img/employers/LG-logo.png"),
                new CompanySeed("employer10@seed.vietjob.com", "Panasonic Vietnam", "Nhà sản xuất thiết bị điện tử và gia dụng tuyển dụng kỹ thuật, sản xuất, chất lượng và chuỗi cung ứng.", "KCN Thăng Long, Hà Nội", "https://panasonic.com/vn", IndustryManufacturing, "/Content/Jobentry/img/employers/Panasonic-logo.png"),
                new CompanySeed("employer11@seed.vietjob.com", "Yamaha Motor Vietnam", "Doanh nghiệp sản xuất xe máy tuyển dụng kỹ sư sản xuất, bảo trì, chất lượng, kho vận và kinh doanh.", "Sóc Sơn, Hà Nội", "https://yamaha-motor.com.vn", IndustryManufacturing, "/Content/Jobentry/img/employers/yamaha2.png"),
                new CompanySeed("employer12@seed.vietjob.com", "Honda Vietnam", "Công ty sản xuất ô tô, xe máy tuyển dụng kỹ sư cơ điện, QA, supply chain và vận hành nhà máy.", "Vĩnh Phúc", "https://honda.com.vn", IndustryManufacturing, "/Content/Jobentry/img/employers/honda2.png"),
                new CompanySeed("employer13@seed.vietjob.com", "Toyota Vietnam", "Doanh nghiệp ô tô tuyển dụng kỹ sư chất lượng, sản xuất, mua hàng, kho vận và dịch vụ khách hàng.", "Vĩnh Phúc", "https://toyota.com.vn", IndustryManufacturing, "/Content/Jobentry/img/employers/Toyota-logo.png"),
                new CompanySeed("employer14@seed.vietjob.com", "Petrolimex", "Doanh nghiệp năng lượng tuyển dụng kinh doanh, kế toán, vận hành, chuỗi cung ứng và kỹ thuật công nghiệp.", "Khâm Thiên, Hà Nội", "https://petrolimex.com.vn", IndustrySales, "/Content/Jobentry/img/employers/Petrolimex-logo.png"),
                new CompanySeed("employer15@seed.vietjob.com", "On-Off Vietnam", "Thương hiệu thời trang tiêu dùng tuyển dụng bán lẻ, e-commerce, marketing, chăm sóc khách hàng và vận hành.", "Hoàng Mai, Hà Nội", "https://onoff.vn", IndustrySales, "/Content/Jobentry/img/employers/On-Off-logo.png"),

                new CompanySeed("employer16@seed.vietjob.com", "BlueRiver Tech", "Công ty phần mềm phát triển nền tảng web, mobile, dữ liệu và tích hợp hệ thống cho doanh nghiệp.", "Quận 1, Hồ Chí Minh", "https://blueriver-tech.example.com", IndustryIt),
                new CompanySeed("employer17@seed.vietjob.com", "Mekong Digital", "Studio sản phẩm số triển khai website, app nội bộ, CRM và dashboard vận hành cho doanh nghiệp vừa.", "Ninh Kiều, Cần Thơ", "https://mekong-digital.example.com", IndustryIt),
                new CompanySeed("employer18@seed.vietjob.com", "NextGen Data Solutions", "Đơn vị dữ liệu cung cấp BI, data warehouse, data analyst và giải pháp báo cáo quản trị.", "Cầu Giấy, Hà Nội", "https://nextgen-data.example.com", IndustryIt),
                new CompanySeed("employer19@seed.vietjob.com", "Lotus Software", "Công ty outsource phần mềm tuyển dụng backend, frontend, QA, BA và UI/UX cho dự án quốc tế.", "Hải Châu, Đà Nẵng", "https://lotus-software.example.com", IndustryIt),
                new CompanySeed("employer20@seed.vietjob.com", "Nova Cloud Systems", "Công ty cloud-native triển khai DevOps, bảo mật ứng dụng, monitoring và hạ tầng dữ liệu.", "Nam Từ Liêm, Hà Nội", "https://nova-cloud.example.com", IndustryIt),
                new CompanySeed("employer21@seed.vietjob.com", "Capital Trust Vietnam", "Tổ chức tài chính tuyển dụng phân tích tài chính, tín dụng, vận hành và quan hệ khách hàng.", "Quận 3, Hồ Chí Minh", "https://capital-trust.example.com", IndustryFinance),
                new CompanySeed("employer22@seed.vietjob.com", "Horizon Banking Solutions", "Công ty giải pháp ngân hàng số tuyển dụng BA, data analyst, vận hành giao dịch và quản trị rủi ro.", "Hoàn Kiếm, Hà Nội", "https://horizon-banking.example.com", IndustryFinance),
                new CompanySeed("employer23@seed.vietjob.com", "Prosper Capital", "Đơn vị tư vấn đầu tư và tài chính doanh nghiệp tuyển dụng phân tích, kế toán và kiểm soát chi phí.", "Quận 1, Hồ Chí Minh", "https://prosper-capital.example.com", IndustryFinance),
                new CompanySeed("employer24@seed.vietjob.com", "EastAsia Finance", "Công ty tài chính tiêu dùng tuyển dụng tín dụng, chăm sóc khách hàng, vận hành hồ sơ và phân tích dữ liệu.", "Thanh Xuân, Hà Nội", "https://eastasia-finance.example.com", IndustryFinance),
                new CompanySeed("employer25@seed.vietjob.com", "Mekong Securities", "Công ty chứng khoán tuyển dụng phân tích tài chính, môi giới, vận hành và quản trị rủi ro.", "Quận 4, Hồ Chí Minh", "https://mekong-securities.example.com", IndustryFinance),
                new CompanySeed("employer26@seed.vietjob.com", "GreenFactory Solutions", "Doanh nghiệp giải pháp nhà máy tuyển dụng kỹ sư sản xuất, bảo trì, tự động hóa và QA.", "KCN VSIP, Bình Dương", "https://greenfactory.example.com", IndustryManufacturing),
                new CompanySeed("employer27@seed.vietjob.com", "Alpha Manufacturing", "Nhà máy linh kiện tuyển dụng kỹ sư chất lượng, warehouse supervisor, procurement và supply chain.", "Biên Hòa, Đồng Nai", "https://alpha-manufacturing.example.com", IndustryManufacturing),
                new CompanySeed("employer28@seed.vietjob.com", "Sunrise Components", "Doanh nghiệp sản xuất linh kiện điện tử tuyển dụng QA engineer, kỹ sư bảo trì và điều phối kho.", "Hải Phòng", "https://sunrise-components.example.com", IndustryManufacturing),
                new CompanySeed("employer29@seed.vietjob.com", "Delta Automation", "Công ty tự động hóa công nghiệp tuyển dụng kỹ sư cơ điện, lập trình PLC, bảo trì và mua hàng kỹ thuật.", "Bình Dương", "https://delta-automation.example.com", IndustryManufacturing),
                new CompanySeed("employer30@seed.vietjob.com", "An Phát Retail", "Doanh nghiệp bán lẻ tuyển dụng tư vấn bán hàng, quản lý cửa hàng, e-commerce và chăm sóc khách hàng.", "Tân Bình, Hồ Chí Minh", "https://anphat-retail.example.com", IndustrySales),
                new CompanySeed("employer31@seed.vietjob.com", "Metro Retail Hub", "Hệ thống bán lẻ tuyển dụng sales, quản lý cửa hàng, digital marketing và vận hành khu vực.", "Đống Đa, Hà Nội", "https://metro-retail.example.com", IndustrySales),
                new CompanySeed("employer32@seed.vietjob.com", "Nova Consumer", "Công ty hàng tiêu dùng tuyển dụng brand executive, trade marketing, sales supervisor và supply chain.", "Quận 7, Hồ Chí Minh", "https://nova-consumer.example.com", IndustryMarketing),
                new CompanySeed("employer33@seed.vietjob.com", "FreshMart Vietnam", "Chuỗi bán lẻ thực phẩm tuyển dụng mua hàng, kế toán kho, quản lý cửa hàng và chăm sóc khách hàng.", "Sơn Trà, Đà Nẵng", "https://freshmart.example.com", IndustrySales),
                new CompanySeed("employer34@seed.vietjob.com", "BrandBee Vietnam", "Agency thương hiệu tuyển dụng account, content, social media, performance marketing và planner.", "Cầu Giấy, Hà Nội", "https://brandbee.example.com", IndustryMarketing),
                new CompanySeed("employer35@seed.vietjob.com", "Pulse Creative", "Creative agency triển khai chiến dịch truyền thông, thiết kế nội dung và digital marketing.", "Quận 1, Hồ Chí Minh", "https://pulse-creative.example.com", IndustryMarketing),
                new CompanySeed("employer36@seed.vietjob.com", "BlueSeed Marketing", "Agency performance marketing tuyển dụng SEO, ads, content, data analyst marketing và account.", "Ba Đình, Hà Nội", "https://blueseed.example.com", IndustryMarketing),
                new CompanySeed("employer37@seed.vietjob.com", "Lighthouse Digital", "Đơn vị digital growth tuyển dụng marketing automation, content, paid media và phân tích chiến dịch.", "Quận 3, Hồ Chí Minh", "https://lighthouse-digital.example.com", IndustryMarketing),
                new CompanySeed("employer38@seed.vietjob.com", "GreenBuild Vietnam", "Công ty xây dựng xanh tuyển dụng kỹ sư xây dựng, kiến trúc sư, QS, MEP và quản lý dự án.", "Thủ Dầu Một, Bình Dương", "https://greenbuild.example.com", IndustryConstruction),
                new CompanySeed("employer39@seed.vietjob.com", "Urban Arc Studio", "Văn phòng kiến trúc tuyển dụng kiến trúc sư, họa viên, thiết kế nội thất và quản lý hồ sơ thiết kế.", "Tây Hồ, Hà Nội", "https://urban-arc.example.com", IndustryConstruction),
                new CompanySeed("employer40@seed.vietjob.com", "Mekong Construction", "Nhà thầu xây dựng tuyển dụng giám sát công trình, kỹ sư QS, chỉ huy trưởng và hành chính dự án.", "Cần Thơ", "https://mekong-construction.example.com", IndustryConstruction),
                new CompanySeed("employer41@seed.vietjob.com", "Skyline MEP", "Công ty MEP tuyển dụng kỹ sư cơ điện, giám sát, bóc tách khối lượng và quản lý thi công.", "Hà Nội", "https://skyline-mep.example.com", IndustryConstruction),
                new CompanySeed("employer42@seed.vietjob.com", "TalentBridge Vietnam", "Công ty dịch vụ nhân sự tuyển dụng recruiter, HR executive, C&B và đào tạo nội bộ.", "Quận 7, Hồ Chí Minh", "https://talentbridge.example.com", IndustryHr),
                new CompanySeed("employer43@seed.vietjob.com", "HR Connect Asia", "Đơn vị tư vấn nhân sự tuyển dụng HRBP, talent acquisition, payroll và employer branding.", "Long Biên, Hà Nội", "https://hrconnect.example.com", IndustryHr),
                new CompanySeed("employer44@seed.vietjob.com", "VietOffice Partners", "Công ty dịch vụ văn phòng tuyển dụng admin executive, trợ lý kinh doanh, thư ký dự án và lễ tân.", "Hải Châu, Đà Nẵng", "https://vietoffice.example.com", IndustryOffice),
                new CompanySeed("employer45@seed.vietjob.com", "Lotus Office Services", "Đơn vị vận hành văn phòng tuyển dụng hành chính, mua hàng, lưu trữ hồ sơ và hỗ trợ nhân sự.", "Quận 10, Hồ Chí Minh", "https://lotus-office.example.com", IndustryOffice),
                new CompanySeed("employer46@seed.vietjob.com", "CarePlus Services", "Công ty dịch vụ khách hàng tuyển dụng customer support, call center agent và client service.", "Tân Phú, Hồ Chí Minh", "https://careplus.example.com", IndustryCustomerService),
                new CompanySeed("employer47@seed.vietjob.com", "Smart Support Vietnam", "Trung tâm hỗ trợ khách hàng tuyển dụng telesales, chăm sóc khách hàng và vận hành ticket.", "Cầu Giấy, Hà Nội", "https://smartsupport.example.com", IndustryCustomerService),
                new CompanySeed("employer48@seed.vietjob.com", "ContactHub Solutions", "Công ty contact center tuyển dụng call center agent, team leader và QA dịch vụ khách hàng.", "Hải Phòng", "https://contacthub.example.com", IndustryCustomerService),
                new CompanySeed("employer49@seed.vietjob.com", "CustomerFirst Vietnam", "Đơn vị client service tuyển dụng chăm sóc khách hàng, client service specialist và vận hành trải nghiệm.", "Đồng Nai", "https://customerfirst.example.com", IndustryCustomerService)
            };
        }

        private List<CompanySeed> BuildCompanySeeds()
        {
            return new List<CompanySeed>
            {
                new CompanySeed("employer@test.com", "Công ty TNHH ACME Việt Nam", "Doanh nghiệp công nghệ phát triển nền tảng tuyển dụng, CRM và các giải pháp số cho doanh nghiệp vừa và lớn.", "Tầng 12, 72 Lê Thánh Tôn, Quận 1, Hồ Chí Minh", "https://acme.example.com"),
                new CompanySeed("employer01@seed.vietjob.com", "FPT Software Việt Nam", "Công ty công nghệ cung cấp dịch vụ chuyển đổi số, phần mềm doanh nghiệp và giải pháp AI cho thị trường toàn cầu.", "Khu Công nghệ cao, TP. Thủ Đức, Hồ Chí Minh", "https://fptsoftware.com"),
                new CompanySeed("employer02@seed.vietjob.com", "Viettel Solutions", "Đơn vị tư vấn và triển khai giải pháp số trong viễn thông, chính phủ điện tử, tài chính và hạ tầng dữ liệu.", "Tòa nhà Viettel, Cầu Giấy, Hà Nội", "https://viettel-solutions.vn"),
                new CompanySeed("employer03@seed.vietjob.com", "CMC Global", "Công ty dịch vụ công nghệ thông tin tập trung vào outsourcing, cloud, dữ liệu và kiểm thử phần mềm.", "Duy Tân, Cầu Giấy, Hà Nội", "https://cmcglobal.com.vn"),
                new CompanySeed("employer04@seed.vietjob.com", "VNPT Technology", "Doanh nghiệp nghiên cứu, sản xuất và triển khai sản phẩm công nghệ cho viễn thông, IoT và chuyển đổi số.", "Phạm Hùng, Nam Từ Liêm, Hà Nội", "https://vnpt-technology.vn"),
                new CompanySeed("employer05@seed.vietjob.com", "VNG Corporation", "Công ty internet và công nghệ vận hành sản phẩm số, nền tảng cloud, game, thanh toán và truyền thông.", "Z06, Khu chế xuất Tân Thuận, Quận 7, Hồ Chí Minh", "https://vng.com.vn"),
                new CompanySeed("employer06@seed.vietjob.com", "MoMo", "Công ty fintech phát triển ví điện tử, dịch vụ thanh toán và các sản phẩm tài chính cá nhân trên nền tảng di động.", "Nguyễn Đình Chiểu, Quận 3, Hồ Chí Minh", "https://momo.vn"),
                new CompanySeed("employer07@seed.vietjob.com", "Tiki Corporation", "Nền tảng thương mại điện tử vận hành các đội ngũ sản phẩm, dữ liệu, vận hành kho và trải nghiệm khách hàng.", "Tân Bình, Hồ Chí Minh", "https://tiki.vn"),
                new CompanySeed("employer08@seed.vietjob.com", "Shopee Vietnam", "Nền tảng thương mại điện tử có nhu cầu tuyển dụng đa dạng trong công nghệ, kinh doanh, marketing và vận hành.", "Saigon Centre, Quận 1, Hồ Chí Minh", "https://shopee.vn"),
                new CompanySeed("employer09@seed.vietjob.com", "Coteccons", "Tổng thầu xây dựng triển khai các dự án dân dụng, công nghiệp, hạ tầng và quản lý thi công quy mô lớn.", "Điện Biên Phủ, Bình Thạnh, Hồ Chí Minh", "https://coteccons.vn"),
                new CompanySeed("employer10@seed.vietjob.com", "Hòa Bình Construction Group", "Tập đoàn xây dựng chuyên thi công nhà cao tầng, khu phức hợp, hạ tầng và các công trình thương mại.", "Ung Văn Khiêm, Bình Thạnh, Hồ Chí Minh", "https://hbcg.vn"),
                new CompanySeed("employer11@seed.vietjob.com", "Ricons", "Doanh nghiệp xây dựng và thiết kế thi công với các dự án bất động sản, khu công nghiệp và trung tâm thương mại.", "Nguyễn Văn Trỗi, Phú Nhuận, Hồ Chí Minh", "https://ricons.vn"),
                new CompanySeed("employer12@seed.vietjob.com", "Central Construction", "Tổng thầu thi công và quản lý dự án xây dựng với đội ngũ kỹ sư, kiến trúc sư và giám sát công trường.", "Nguyễn Cơ Thạch, TP. Thủ Đức, Hồ Chí Minh", "https://centralcons.vn"),
                new CompanySeed("employer13@seed.vietjob.com", "Techcombank Digital", "Khối ngân hàng số phát triển sản phẩm tài chính, dữ liệu khách hàng, vận hành số và trải nghiệm giao dịch trực tuyến.", "Bà Triệu, Hoàn Kiếm, Hà Nội", "https://techcombank.com"),
                new CompanySeed("employer14@seed.vietjob.com", "VPBank Technology", "Đội ngũ công nghệ hỗ trợ nền tảng ngân hàng số, quản trị dữ liệu, phân tích rủi ro và tự động hóa quy trình.", "Láng Hạ, Đống Đa, Hà Nội", "https://vpbank.com.vn"),
                new CompanySeed("employer15@seed.vietjob.com", "NovaWorks Digital", "Studio sản phẩm số xây dựng web app, mobile app, dashboard nội bộ và giải pháp thương mại điện tử.", "Nguyễn Hữu Cảnh, Bình Thạnh, Hồ Chí Minh", null),
                new CompanySeed("employer16@seed.vietjob.com", "GreenBuild Architects", "Công ty kiến trúc xanh chuyên thiết kế văn phòng, nhà ở cao tầng, khu nghỉ dưỡng và không gian thương mại.", "Võ Văn Kiệt, Quận 1, Hồ Chí Minh", null),
                new CompanySeed("employer17@seed.vietjob.com", "Mekong Retail Solutions", "Doanh nghiệp tư vấn tăng trưởng bán lẻ, triển khai CRM, trade marketing và phát triển đội ngũ kinh doanh vùng.", "Ninh Kiều, Cần Thơ", null),
                new CompanySeed("employer18@seed.vietjob.com", "Lotus Media Group", "Agency truyền thông tích hợp cung cấp dịch vụ nội dung, social, performance marketing và tổ chức chiến dịch thương hiệu.", "Trần Hưng Đạo, Hoàn Kiếm, Hà Nội", null),
                new CompanySeed("employer19@seed.vietjob.com", "BlueOcean Logistics", "Công ty logistics và chuỗi cung ứng tuyển dụng cho vận hành kho, kinh doanh B2B, điều phối và chăm sóc khách hàng.", "Lê Hồng Phong, Hải Phòng", null),
                new CompanySeed("employer20@seed.vietjob.com", "An Phát Finance", "Tổ chức tài chính tiêu dùng tập trung vào phân tích tín dụng, tư vấn khách hàng, thu hồi nợ mềm và phát triển kênh số.", "Nam Kỳ Khởi Nghĩa, Quận 3, Hồ Chí Minh", null),
                new CompanySeed("employer21@seed.vietjob.com", "Saigon Data Hub", "Doanh nghiệp dữ liệu cung cấp giải pháp BI, data warehouse, tích hợp hệ thống và phân tích vận hành cho doanh nghiệp.", "Cộng Hòa, Tân Bình, Hồ Chí Minh", null),
                new CompanySeed("employer22@seed.vietjob.com", "Hanoi Creative Lab", "Xưởng sáng tạo phát triển nội dung số, thiết kế thương hiệu, video marketing và chiến dịch truyền thông đa kênh.", "Tô Ngọc Vân, Tây Hồ, Hà Nội", null),
                new CompanySeed("employer23@seed.vietjob.com", "Da Nang Software Studio", "Công ty phần mềm tại miền Trung chuyên outsource web, mobile, QA và vận hành sản phẩm SaaS cho khách hàng quốc tế.", "Hải Châu, Đà Nẵng", null),
                new CompanySeed("employer24@seed.vietjob.com", "Binh Duong Smart Factory", "Nhà máy thông minh tuyển dụng kỹ sư sản xuất, kế hoạch, nhân sự, kế toán và đội ngũ chuyển đổi số nội bộ.", "KCN VSIP, Bình Dương", null),
                new CompanySeed("employer25@seed.vietjob.com", "Hai Phong Port Services", "Doanh nghiệp dịch vụ cảng biển cần đội ngũ điều phối, kinh doanh logistics, hành chính và quản lý vận hành.", "Đình Vũ, Hải Phòng", null),
                new CompanySeed("employer26@seed.vietjob.com", "Dong Nai Industrial Partners", "Công ty phát triển dịch vụ khu công nghiệp, tuyển dụng các vị trí hành chính, kỹ thuật, xây dựng và quan hệ khách hàng.", "Biên Hòa, Đồng Nai", null),
                new CompanySeed("employer27@seed.vietjob.com", "Quang Ninh Tourism Services", "Doanh nghiệp dịch vụ du lịch và vận hành điểm đến với nhu cầu tuyển dụng bán hàng, marketing và nhân sự mùa cao điểm.", "Hạ Long, Quảng Ninh", null),
                new CompanySeed("employer28@seed.vietjob.com", "Alpha Sales Vietnam", "Công ty phát triển kênh bán hàng B2B, đào tạo sales, thiết kế quy trình chăm sóc khách hàng và mở rộng thị trường.", "Phú Nhuận, Hồ Chí Minh", null),
                new CompanySeed("employer29@seed.vietjob.com", "Vertex Marketing Agency", "Agency performance marketing chuyên quảng cáo số, SEO, nội dung, phân tích chiến dịch và tối ưu chuyển đổi.", "Cầu Giấy, Hà Nội", null),
                new CompanySeed("employer30@seed.vietjob.com", "Summit HR Consulting", "Công ty tư vấn nhân sự cung cấp dịch vụ tuyển dụng, đào tạo, chính sách phúc lợi và đánh giá năng lực.", "Quận 7, Hồ Chí Minh", null),
                new CompanySeed("employer31@seed.vietjob.com", "Golden Ledger Accounting", "Công ty dịch vụ kế toán, kiểm toán nội bộ và tư vấn thuế cho doanh nghiệp vừa, startup và văn phòng đại diện.", "Thanh Xuân, Hà Nội", null),
                new CompanySeed("employer32@seed.vietjob.com", "Capital Bridge Finance", "Đơn vị tư vấn tài chính doanh nghiệp, phân tích đầu tư, quản trị dòng tiền và hỗ trợ gọi vốn.", "Quận 1, Hồ Chí Minh", null),
                new CompanySeed("employer33@seed.vietjob.com", "Pacific Office Services", "Công ty dịch vụ văn phòng chuyên vận hành hành chính, lễ tân, mua sắm, lưu trữ hồ sơ và hỗ trợ nhân sự.", "Sơn Trà, Đà Nẵng", null),
                new CompanySeed("employer34@seed.vietjob.com", "Skyline Architecture", "Văn phòng kiến trúc và quy hoạch đô thị tuyển dụng kiến trúc sư, họa viên, QS và quản lý thiết kế.", "Ba Đình, Hà Nội", null),
                new CompanySeed("employer35@seed.vietjob.com", "BrightPath Education Tech", "Nền tảng edtech phát triển sản phẩm học trực tuyến, tuyển dụng cho công nghệ, marketing, vận hành và chăm sóc học viên.", "Quận 10, Hồ Chí Minh", null),
                new CompanySeed("employer36@seed.vietjob.com", "Nimbus Cloud Solutions", "Công ty cloud-native triển khai hạ tầng, DevOps, bảo mật ứng dụng và dịch vụ managed cloud cho doanh nghiệp.", "Nam Từ Liêm, Hà Nội", null),
                new CompanySeed("employer37@seed.vietjob.com", "TerraBuild Engineering", "Doanh nghiệp kỹ thuật xây dựng chuyên kết cấu, MEP, giám sát thi công và quản lý hồ sơ dự án.", "Thủ Dầu Một, Bình Dương", null),
                new CompanySeed("employer38@seed.vietjob.com", "MarketPulse Vietnam", "Đơn vị nghiên cứu thị trường và dữ liệu khách hàng hỗ trợ brand team, sales team và bộ phận chiến lược.", "Quận 4, Hồ Chí Minh", null),
                new CompanySeed("employer39@seed.vietjob.com", "PeopleFirst Talent", "Công ty dịch vụ nhân sự tập trung tuyển dụng số lượng lớn, employer branding, đào tạo hội nhập và HR operations.", "Long Biên, Hà Nội", null)
            };
        }

        private string ResolveEmployerLogoPath(string companyName, int index)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                return string.Format("/Content/Jobentry/img/com-logo-{0}.jpg", (index % 5) + 1);
            }

            var normalized = companyName.ToLowerInvariant();
            if (normalized.Contains("shopee") || normalized.Contains("lazada"))
            {
                return null;
            }

            var logoMap = new Dictionary<string, string>
            {
                { "viettel", "Viettel-logo.png" },
                { "fpt", "FPT-logo.png" },
                { "vinamilk", "Vinamilk-logo.png" },
                { "techcombank", "techcombank2.jpg" },
                { "vietinbank", "Vietinbank-logo.png" },
                { "agribank", "Agribank-logo.png" },
                { "shb", "SHB-logo.png" },
                { "mobifone", "Mobifone-logo.png" },
                { "vinaphone", "Vinaphone-logo.png" },
                { "lg", "LG-logo.png" },
                { "panasonic", "Panasonic-logo.png" },
                { "yamaha", "yamaha2.png" },
                { "honda", "honda2.png" },
                { "toyota", "Toyota-logo.png" },
                { "petrolimex", "Petrolimex-logo.png" },
                { "on-off", "On-Off-logo.png" },
                { "on off", "On-Off-logo.png" },
                { "vietcombank", "Vietcombank-logo.png" }
            };

            foreach (var item in logoMap)
            {
                if (normalized.Contains(item.Key))
                {
                    return "/Content/Jobentry/img/employers/" + item.Value;
                }
            }

            return string.Format("/Content/Jobentry/img/com-logo-{0}.jpg", (index % 5) + 1);
        }

        private List<HoSoCaNhan> SeedCandidates(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var result = new List<HoSoCaNhan>();
            var seeds = BuildCandidateSeeds();

            for (var i = 0; i < seeds.Count; i++)
            {
                var seed = seeds[i];
                var user = EnsureUser(userManager, seed.Email, SeedPassword, ApplicationRoles.UngVien);
                var profile = context.HoSoCaNhans.FirstOrDefault(x => x.ApplicationUserId == user.Id);
                if (profile == null)
                {
                    profile = new HoSoCaNhan
                    {
                        ApplicationUserId = user.Id
                    };
                    context.HoSoCaNhans.Add(profile);
                }

                profile.HoTen = seed.HoTen;
                profile.NgaySinh = new DateTime(1994 + (i % 9), ((i + 2) % 12) + 1, ((i + 7) % 24) + 1);
                profile.GioiTinh = i % 3 == 0 ? "Nữ" : "Nam";
                profile.SoDienThoai = string.Format("08{0:00000000}", 20000000 + i);
                profile.DiaChi = seed.DiaChi;
                profile.MucTieuNgheNghiep = string.Format("Tìm kiếm cơ hội {0}, ưu tiên môi trường có quy trình rõ ràng, phản hồi minh bạch và cơ hội phát triển dài hạn.", seed.DinhHuong);
                profile.HocVan = string.Format("Cử nhân {0} - {1}", seed.NganhHoc, seed.Truong);
                profile.TomTatKinhNghiem = string.Format("Có {0} năm kinh nghiệm trong lĩnh vực {1}; đã tham gia dự án thực tế, phối hợp liên phòng ban và sử dụng tốt công cụ số trong công việc.", 1 + (i % 6), seed.DinhHuong.ToLower());
                profile.NgayCapNhat = DateTime.Now.AddDays(-(i % 12));

                context.SaveChanges();
                EnsureCandidateCvs(context, profile, seed, i);
                result.Add(profile);
            }

            return result;
        }

        private List<CandidateSeed> BuildCandidateSeeds()
        {
            var names = new[]
            {
                "Nguyễn Văn A", "Trần Minh Anh", "Lê Hoàng Nam", "Phạm Thu Hà", "Võ Quốc Huy",
                "Đặng Ngọc Linh", "Bùi Gia Bảo", "Huỳnh Mai Phương", "Đỗ Thành Đạt", "Ngô Thảo Vy",
                "Phan Nhật Minh", "Vũ Hải Yến", "Mai Đức Anh", "Dương Khánh Ly", "Hoàng Tuấn Kiệt",
                "Lâm Bảo Ngọc", "Cao Minh Quân", "Trương Hà My", "Tạ Anh Khoa", "Châu Nhật Hạ",
                "Đinh Quốc Thịnh", "Lý Thanh Tâm", "Hồ Gia Hân", "Nguyễn Hoài Đức", "Trần Khánh Ngân",
                "Lê Trung Hiếu", "Phạm Kim Chi", "Võ Minh Triết", "Bùi Hồng Nhung", "Đặng Anh Tú"
            };
            var focus = new[]
            {
                "Backend .NET Developer", "Chuyên viên kinh doanh B2B", "Digital Marketing Executive", "Nhân viên hành chính",
                "Kế toán tổng hợp", "Chuyên viên tài chính", "Kỹ sư xây dựng", "Chuyên viên tuyển dụng"
            };
            var majors = new[]
            {
                "Công nghệ thông tin", "Quản trị kinh doanh", "Marketing", "Quản trị văn phòng",
                "Kế toán", "Tài chính ngân hàng", "Kỹ thuật xây dựng", "Quản trị nhân lực"
            };
            var schools = new[]
            {
                "Đại học Kinh tế TP. Hồ Chí Minh", "Đại học Bách khoa Hà Nội", "Đại học Đà Nẵng",
                "Đại học Cần Thơ", "Đại học Tài chính - Marketing", "Đại học Xây dựng Hà Nội"
            };
            var addresses = new[]
            {
                "Bình Thạnh, Hồ Chí Minh", "Cầu Giấy, Hà Nội", "Hải Châu, Đà Nẵng", "Ninh Kiều, Cần Thơ",
                "Thủ Dầu Một, Bình Dương", "Lê Chân, Hải Phòng", "Biên Hòa, Đồng Nai", "Hạ Long, Quảng Ninh"
            };

            var seeds = new List<CandidateSeed>();
            for (var i = 0; i < names.Length; i++)
            {
                seeds.Add(new CandidateSeed
                {
                    Email = i == 0 ? "ungvien@test.com" : string.Format("candidate{0:00}@seed.vietjob.com", i),
                    HoTen = names[i],
                    DinhHuong = focus[i % focus.Length],
                    NganhHoc = majors[i % majors.Length],
                    Truong = schools[i % schools.Length],
                    DiaChi = addresses[i % addresses.Length]
                });
            }

            return seeds;
        }

        private void EnsureCandidateCvs(ApplicationDbContext context, HoSoCaNhan profile, CandidateSeed seed, int index)
        {
            EnsureCv(context, profile.HoSoCaNhanId, string.Format("CV {0}", seed.DinhHuong), string.Format("/Uploads/CV/seed-candidate-{0:00}-main.pdf", index + 1), true, DateTime.Now.AddDays(-(index % 20)));
            EnsureCv(context, profile.HoSoCaNhanId, string.Format("Portfolio {0}", seed.DinhHuong), string.Format("/Uploads/CV/seed-candidate-{0:00}-portfolio.pdf", index + 1), false, DateTime.Now.AddDays(-(20 + index)));
        }

        private CVUngVien EnsureCv(ApplicationDbContext context, int hoSoCaNhanId, string tenCv, string duongDanFile, bool trangThaiSuDung, DateTime ngayTaiLen)
        {
            var cv = context.CVUngViens.FirstOrDefault(x => x.HoSoCaNhanId == hoSoCaNhanId && x.TenCV == tenCv);
            if (cv == null)
            {
                cv = new CVUngVien
                {
                    HoSoCaNhanId = hoSoCaNhanId
                };
                context.CVUngViens.Add(cv);
            }

            cv.TenCV = tenCv;
            cv.DuongDanFile = duongDanFile;
            cv.TrangThaiSuDung = trangThaiSuDung;
            cv.NgayTaiLen = ngayTaiLen;
            context.SaveChanges();
            return cv;
        }

        private List<TinTuyenDung> SeedJobs(ApplicationDbContext context, List<HoSoCongTy> employers, string adminUserId)
        {
            var industries = ToDictionary(context.NganhNghes.ToList(), x => x.TenNganhNghe, x => x.NganhNgheId);
            var locations = ToDictionary(context.DiaDiems.ToList(), x => x.TenDiaDiem, x => x.DiaDiemId);
            var workTypes = ToDictionary(context.LoaiHinhLamViecs.ToList(), x => x.TenLoaiHinhLamViec, x => x.LoaiHinhLamViecId);
            var levels = ToDictionary(context.CapDoKinhNghiems.ToList(), x => x.TenCapDoKinhNghiem, x => x.CapDoKinhNghiemId);

            var industryPlan = BuildWeightedList(
                new Distribution(IndustryIt, 72),
                new Distribution(IndustrySales, 54),
                new Distribution(IndustryMarketing, 36),
                new Distribution(IndustryOffice, 30),
                new Distribution(IndustryAccounting, 24),
                new Distribution(IndustryFinance, 42),
                new Distribution(IndustryConstruction, 30),
                new Distribution(IndustryHr, 24),
                new Distribution(IndustryManufacturing, 30),
                new Distribution(IndustryCustomerService, 18));

            var locationPlan = BuildWeightedList(
                new Distribution(LocationHcm, 126),
                new Distribution(LocationHaNoi, 114),
                new Distribution(LocationDaNang, 30),
                new Distribution(LocationBinhDuong, 24),
                new Distribution(LocationHaiPhong, 18),
                new Distribution(LocationDongNai, 18),
                new Distribution(LocationCanTho, 16),
                new Distribution(LocationQuangNinh, 14));

            var workTypePlan = BuildWeightedList(
                new Distribution(WorkFullTime, 222),
                new Distribution(WorkPartTime, 30),
                new Distribution(WorkRemote, 30),
                new Distribution(WorkHybrid, 30),
                new Distribution(WorkIntern, 24),
                new Distribution(WorkSeasonal, 24));

            var statusPlan = BuildWeightedList(
                new Distribution(TrangThaiTinTuyenDung.DaDuyet, 252),
                new Distribution(TrangThaiTinTuyenDung.ChoDuyet, 36),
                new Distribution(TrangThaiTinTuyenDung.Nhap, 29),
                new Distribution(TrangThaiTinTuyenDung.BiTuChoi, 18),
                new Distribution(TrangThaiTinTuyenDung.DaDong, 18),
                new Distribution(TrangThaiTinTuyenDung.HetHan, 7));

            var result = new List<TinTuyenDung>();
            for (var i = 0; i < 360; i++)
            {
                var location = locationPlan[(i * 43 + 17) % locationPlan.Count];
                var workType = workTypePlan[(i * 47 + 5) % workTypePlan.Count];
                var status = statusPlan[(i * 53 + 9) % statusPlan.Count];
                var company = employers[(i * 17 + 3) % employers.Count];
                var companyIndustries = ResolveCompanyIndustries(company.TenCongTy)
                    .Where(x => industries.ContainsKey(x))
                    .ToList();
                var industry = companyIndustries.Any()
                    ? companyIndustries[(i / Math.Max(1, employers.Count)) % companyIndustries.Count]
                    : industryPlan[(i * 37 + 11) % industryPlan.Count];
                var level = ResolveLevel(workType, i);
                var title = BuildJobTitle(industry, i);
                var seedCode = BuildSeedJobCode(i);
                decimal? minSalary;
                decimal? maxSalary;
                BuildSalaryRange(industry, workType, i, out minSalary, out maxSalary);

                var ngayDang = ResolveNgayDang(status, i);
                var hanNop = ResolveHanNop(status, i);
                var tin = EnsureTin(context,
                    company.HoSoCongTyId,
                    title,
                    seedCode,
                    status,
                    industries[industry],
                    locations[location],
                    workTypes[workType],
                    levels[level],
                    ngayDang,
                    hanNop,
                    1 + (i % 8),
                    minSalary,
                    maxSalary,
                    BuildJobDescription(industry, title, company.TenCongTy, location),
                    BuildJobRequirement(industry, level),
                    BuildJobBenefit(workType, i),
                    DateTime.Now.AddDays(-(60 + (i % 45))));

                SeedReviewLogsForJob(context, tin, company.ApplicationUserId, adminUserId);
                result.Add(tin);
            }

            return result;
        }

        private Dictionary<string, int> ToDictionary<T>(IEnumerable<T> source, Func<T, string> keySelector, Func<T, int> valueSelector)
        {
            return source
                .GroupBy(keySelector)
                .ToDictionary(x => x.Key, x => valueSelector(x.First()));
        }

        private List<string> BuildWeightedList(params Distribution[] distributions)
        {
            var result = new List<string>();
            foreach (var distribution in distributions)
            {
                for (var i = 0; i < distribution.Count; i++)
                {
                    result.Add(distribution.Value);
                }
            }

            return result;
        }

        private List<string> ResolveCompanyIndustries(string companyName)
        {
            var name = (companyName ?? string.Empty).ToLowerInvariant();

            if ((name.Contains("fpt") || name.Contains("tech") || name.Contains("digital") || name.Contains("data") || name.Contains("software") || name.Contains("cloud") || name.Contains("viettel")) && !name.Contains("bank"))
            {
                return new List<string> { IndustryIt, IndustrySales, IndustryCustomerService };
            }

            if (name.Contains("bank") || name.Contains("agribank") || name.Contains("shb") || name.Contains("finance") || name.Contains("capital") || name.Contains("securities"))
            {
                return new List<string> { IndustryFinance, IndustryAccounting, IndustryIt };
            }

            if (name.Contains("mobifone") || name.Contains("vinaphone"))
            {
                return new List<string> { IndustryCustomerService, IndustryIt, IndustrySales };
            }

            if (name.Contains("vinamilk") || name.Contains("petrolimex") || name.Contains("retail") || name.Contains("consumer") || name.Contains("freshmart") || name.Contains("on-off"))
            {
                return new List<string> { IndustrySales, IndustryMarketing, IndustryAccounting, IndustryCustomerService };
            }

            if (name.Contains("lg") || name.Contains("panasonic") || name.Contains("yamaha") || name.Contains("honda") || name.Contains("toyota") || name.Contains("factory") || name.Contains("manufacturing") || name.Contains("components") || name.Contains("automation"))
            {
                return new List<string> { IndustryManufacturing, IndustryAccounting, IndustryOffice };
            }

            if (name.Contains("brand") || name.Contains("creative") || name.Contains("marketing") || name.Contains("lighthouse") || name.Contains("pulse"))
            {
                return new List<string> { IndustryMarketing, IndustrySales, IndustryCustomerService };
            }

            if (name.Contains("build") || name.Contains("construction") || name.Contains("arc") || name.Contains("mep") || name.Contains("skyline"))
            {
                return new List<string> { IndustryConstruction, IndustryOffice, IndustryAccounting };
            }

            if (name.Contains("talent") || name.Contains("hr ") || name.Contains("office"))
            {
                return new List<string> { IndustryHr, IndustryOffice, IndustryCustomerService };
            }

            if (name.Contains("support") || name.Contains("contact") || name.Contains("care") || name.Contains("customer"))
            {
                return new List<string> { IndustryCustomerService, IndustrySales, IndustryOffice };
            }

            return new List<string> { IndustrySales, IndustryOffice };
        }

        private string ResolveLevel(string workType, int index)
        {
            if (workType == WorkIntern)
            {
                return "Fresher";
            }

            if (workType == WorkSeasonal || workType == WorkPartTime)
            {
                return index % 2 == 0 ? "Không yêu cầu" : "Junior";
            }

            var levels = new[] { "Không yêu cầu", "Fresher", "Junior", "Senior", "Trưởng nhóm" };
            return levels[(index * 19 + 3) % levels.Length];
        }

        private string BuildJobTitle(string industry, int index)
        {
            string[] titles;
            if (industry == IndustryIt)
            {
                titles = new[]
                {
                    "Backend Developer", "Frontend Developer", "Fullstack Developer", "QA/QC Tester",
                    "DevOps Engineer", "Data Analyst", "IT Support", "Business Analyst", "Product Owner", "UI/UX Designer"
                };
            }
            else if (industry == IndustrySales)
            {
                titles = new[]
                {
                    "Chuyên viên kinh doanh B2B", "Nhân viên tư vấn khách hàng", "Sales Executive", "Key Account Executive",
                    "Trưởng nhóm kinh doanh", "Telesales Consultant", "Chuyên viên phát triển thị trường", "Giám sát bán hàng"
                };
            }
            else if (industry == IndustryMarketing)
            {
                titles = new[]
                {
                    "Digital Marketing Executive", "Content Marketing Specialist", "Performance Marketing Executive",
                    "SEO Specialist", "Social Media Executive", "Brand Executive", "Trade Marketing Executive", "Marketing Planner"
                };
            }
            else if (industry == IndustryOffice)
            {
                titles = new[]
                {
                    "Nhân viên hành chính văn phòng", "Office Administrator", "Trợ lý giám đốc", "Lễ tân văn phòng",
                    "Chuyên viên mua hàng", "Điều phối vận hành", "Nhân viên chăm sóc hồ sơ"
                };
            }
            else if (industry == IndustryAccounting)
            {
                titles = new[]
                {
                    "Kế toán tổng hợp", "Kế toán thanh toán", "Kế toán công nợ", "Kiểm toán nội bộ",
                    "Chuyên viên thuế", "Kế toán kho", "Accounting Executive"
                };
            }
            else if (industry == IndustryFinance)
            {
                titles = new[]
                {
                    "Giao Dịch Viên", "Chuyên Viên Quan Hệ Khách Hàng", "Chuyên Viên Tín Dụng",
                    "Chuyên Viên Phân Tích Tài Chính", "Chuyên Viên Quản Trị Rủi Ro", "Kế Toán Nội Bộ",
                    "BA Ngân Hàng Số", "Data Analyst Tài Chính", "Chuyên Viên Vận Hành"
                };
            }
            else if (industry == IndustryConstruction)
            {
                titles = new[]
                {
                    "Kỹ sư xây dựng", "Kiến trúc sư thiết kế", "Giám sát công trình", "Kỹ sư QS",
                    "Kỹ sư MEP", "Chỉ huy trưởng công trình", "Họa viên kiến trúc", "Quản lý dự án xây dựng"
                };
            }
            else if (industry == IndustryManufacturing)
            {
                titles = new[]
                {
                    "Kỹ Sư Sản Xuất", "Kỹ Sư Bảo Trì", "Kỹ Sư Cơ Điện", "QA Engineer",
                    "Kỹ Sư Chất Lượng", "Supply Chain Executive", "Procurement Executive", "Warehouse Supervisor"
                };
            }
            else if (industry == IndustryCustomerService)
            {
                titles = new[]
                {
                    "Nhân Viên Chăm Sóc Khách Hàng", "Customer Support Executive", "Telesales Executive",
                    "Call Center Agent", "Client Service Specialist", "Trưởng Nhóm Chăm Sóc Khách Hàng"
                };
            }
            else
            {
                titles = new[]
                {
                    "Chuyên viên tuyển dụng", "HR Executive", "Chuyên viên C&B", "HRBP",
                    "Chuyên viên đào tạo", "Talent Acquisition Specialist"
                };
            }

            var scopes = new[] { "khối doanh nghiệp", "mảng tăng trưởng", "team vận hành", "dự án mới", "thị trường Việt Nam", "kênh online", "khu vực miền Nam", "khu vực miền Bắc" };
            return titles[index % titles.Length];
        }

        private string BuildSeedJobCode(int index)
        {
            return string.Format("JD-{0:000}", index + 1);
        }

        private void BuildSalaryRange(string industry, string workType, int index, out decimal? minSalary, out decimal? maxSalary)
        {
            if (workType == WorkIntern)
            {
                minSalary = 3000000m + (index % 3) * 1000000m;
                maxSalary = minSalary + 3000000m;
                return;
            }

            if (workType == WorkSeasonal || workType == WorkPartTime)
            {
                minSalary = 6000000m + (index % 5) * 1000000m;
                maxSalary = minSalary + 5000000m;
                return;
            }

            var baseMin = 9000000m;
            var baseMax = 18000000m;
            if (industry == IndustryIt)
            {
                baseMin = 18000000m;
                baseMax = 42000000m;
            }
            else if (industry == IndustrySales)
            {
                baseMin = 10000000m;
                baseMax = 26000000m;
            }
            else if (industry == IndustryMarketing)
            {
                baseMin = 12000000m;
                baseMax = 28000000m;
            }
            else if (industry == IndustryAccounting)
            {
                baseMin = 10000000m;
                baseMax = 22000000m;
            }
            else if (industry == IndustryFinance)
            {
                baseMin = 15000000m;
                baseMax = 35000000m;
            }
            else if (industry == IndustryConstruction)
            {
                baseMin = 14000000m;
                baseMax = 32000000m;
            }
            else if (industry == IndustryManufacturing)
            {
                baseMin = 12000000m;
                baseMax = 30000000m;
            }
            else if (industry == IndustryCustomerService)
            {
                baseMin = 8000000m;
                baseMax = 18000000m;
            }
            else if (industry == IndustryHr)
            {
                baseMin = 10000000m;
                baseMax = 23000000m;
            }

            minSalary = baseMin + (index % 5) * 1000000m;
            maxSalary = baseMax + (index % 6) * 1500000m;
        }

        private DateTime? ResolveNgayDang(string status, int index)
        {
            if (status == TrangThaiTinTuyenDung.Nhap || status == TrangThaiTinTuyenDung.ChoDuyet || status == TrangThaiTinTuyenDung.BiTuChoi)
            {
                return null;
            }

            if (status == TrangThaiTinTuyenDung.HetHan)
            {
                return DateTime.Today.AddDays(-(80 + (index % 30)));
            }

            return DateTime.Today.AddDays(-(index % 35));
        }

        private DateTime ResolveHanNop(string status, int index)
        {
            if (status == TrangThaiTinTuyenDung.HetHan)
            {
                return DateTime.Today.AddDays(-(1 + (index % 18)));
            }

            if (status == TrangThaiTinTuyenDung.DaDong)
            {
                return DateTime.Today.AddDays(5 + (index % 20));
            }

            if (status == TrangThaiTinTuyenDung.DaDuyet && index % 17 == 0)
            {
                return DateTime.Today.AddDays(4 + (index % 4));
            }

            return DateTime.Today.AddDays(14 + (index % 45));
        }

        private string BuildJobDescription(string industry, string title, string companyName, string location)
        {
            return string.Join(Environment.NewLine, new[]
            {
                string.Format("Đảm nhiệm vị trí {0} tại {1}, phối hợp cùng các bộ phận liên quan để đạt mục tiêu tuyển dụng và vận hành.", title, companyName),
                string.Format("Tham gia xử lý công việc thực tế trong lĩnh vực {0}, ưu tiên hiệu quả, tính chính xác và trải nghiệm người dùng nội bộ/khách hàng.", industry),
                string.Format("Làm việc với đội ngũ tại {0}, báo cáo tiến độ định kỳ và đề xuất cải tiến quy trình khi phát hiện điểm nghẽn.", location),
                "Sử dụng dữ liệu, công cụ quản lý công việc và tài liệu chuẩn để theo dõi kết quả, bàn giao minh bạch và duy trì chất lượng đầu ra."
            });
        }

        private string BuildJobRequirement(string industry, string level)
        {
            return string.Join(Environment.NewLine, new[]
            {
                string.Format("Phù hợp cấp độ {0}; có kiến thức nền tảng hoặc kinh nghiệm liên quan đến {1}.", level, industry),
                "Giao tiếp rõ ràng, chủ động cập nhật tiến độ và có khả năng làm việc cùng nhiều bộ phận.",
                "Biết sử dụng các công cụ văn phòng, quản lý công việc hoặc phần mềm chuyên môn theo yêu cầu từng vị trí.",
                "Ưu tiên ứng viên có tinh thần học hỏi, tư duy cải tiến và mong muốn gắn bó trong môi trường chuyên nghiệp."
            });
        }

        private string BuildJobBenefit(string workType, int index)
        {
            var flexibleText = workType == WorkRemote || workType == WorkHybrid
                ? "Chính sách làm việc linh hoạt, hỗ trợ công cụ làm việc từ xa và quy trình phối hợp minh bạch."
                : "Môi trường làm việc ổn định, onboarding rõ ràng và có người hướng dẫn trong giai đoạn đầu.";

            return string.Join(Environment.NewLine, new[]
            {
                flexibleText,
                string.Format("Lương tháng 13, đánh giá hiệu suất định kỳ {0} tháng/lần và thưởng theo kết quả kinh doanh.", (index % 2 == 0 ? 6 : 12)),
                "Được tham gia bảo hiểm, ngày phép, chương trình đào tạo nội bộ và các hoạt động gắn kết đội nhóm.",
                "Có lộ trình phát triển nghề nghiệp rõ ràng, cơ hội tham gia dự án thực tế và tiếp xúc trực tiếp với khách hàng/người dùng."
            });
        }

        private TinTuyenDung EnsureTin(
            ApplicationDbContext context,
            int hoSoCongTyId,
            string tieuDe,
            string seedCode,
            string trangThaiTin,
            int nganhNgheId,
            int diaDiemId,
            int loaiHinhLamViecId,
            int capDoKinhNghiemId,
            DateTime? ngayDang,
            DateTime hanNopHoSo,
            int soLuongTuyen,
            decimal? luongToiThieu,
            decimal? luongToiDa,
            string moTaCongViec,
            string yeuCau,
            string quyenLoi,
            DateTime ngayTao)
        {
            var tin = !string.IsNullOrWhiteSpace(seedCode)
                ? context.TinTuyenDungs.FirstOrDefault(x => x.TieuDe.EndsWith(seedCode))
                : null;
            if (tin == null)
            {
                tin = context.TinTuyenDungs.FirstOrDefault(x => x.HoSoCongTyId == hoSoCongTyId && x.TieuDe == tieuDe);
            }

            if (tin == null)
            {
                var legacyPrefix = tieuDe + " - ";
                tin = context.TinTuyenDungs.FirstOrDefault(x =>
                    x.HoSoCongTyId == hoSoCongTyId &&
                    x.TieuDe.StartsWith(legacyPrefix) &&
                    !x.TieuDe.Contains("JD-"));
            }

            if (tin == null)
            {
                tin = new TinTuyenDung
                {
                    HoSoCongTyId = hoSoCongTyId,
                    NgayTao = ngayTao
                };
                context.TinTuyenDungs.Add(tin);
            }

            tin.TieuDe = tieuDe;
            tin.NganhNgheId = nganhNgheId;
            tin.DiaDiemId = diaDiemId;
            tin.LoaiHinhLamViecId = loaiHinhLamViecId;
            tin.CapDoKinhNghiemId = capDoKinhNghiemId;
            tin.MoTaCongViec = moTaCongViec;
            tin.YeuCau = yeuCau;
            tin.QuyenLoi = quyenLoi;
            tin.SoLuongTuyen = soLuongTuyen;
            tin.LuongToiThieu = luongToiThieu;
            tin.LuongToiDa = luongToiDa;
            tin.NgayDang = ngayDang;
            tin.HanNopHoSo = hanNopHoSo;
            tin.TrangThaiTin = trangThaiTin;
            tin.NgayCapNhat = DateTime.Now;
            context.SaveChanges();
            return tin;
        }

        private void SeedReviewLogsForJob(ApplicationDbContext context, TinTuyenDung tin, string employerUserId, string adminUserId)
        {
            if (tin.TrangThaiTin == TrangThaiTinTuyenDung.Nhap)
            {
                return;
            }

            var baseTime = tin.NgayTao;
            EnsureReviewLog(context, tin.TinTuyenDungId, employerUserId, HanhDongDuyetTin.GuiDuyet, null, baseTime.AddHours(2));

            if (tin.TrangThaiTin == TrangThaiTinTuyenDung.DaDuyet ||
                tin.TrangThaiTin == TrangThaiTinTuyenDung.DaDong ||
                tin.TrangThaiTin == TrangThaiTinTuyenDung.HetHan)
            {
                EnsureReviewLog(context, tin.TinTuyenDungId, adminUserId, HanhDongDuyetTin.Duyet, null, baseTime.AddHours(6));
            }

            if (tin.TrangThaiTin == TrangThaiTinTuyenDung.BiTuChoi)
            {
                EnsureReviewLog(context, tin.TinTuyenDungId, adminUserId, HanhDongDuyetTin.TuChoi, "Tin cần bổ sung thông tin yêu cầu công việc, quyền lợi hoặc khoảng lương rõ ràng hơn.", baseTime.AddHours(5));
            }

            if (tin.TrangThaiTin == TrangThaiTinTuyenDung.DaDong)
            {
                EnsureReviewLog(context, tin.TinTuyenDungId, employerUserId, HanhDongDuyetTin.DongTin, "Nhà tuyển dụng đã đủ số lượng hồ sơ phù hợp.", DateTime.Now.AddDays(-2));
            }
        }

        private void EnsureReviewLog(ApplicationDbContext context, int tinTuyenDungId, string applicationUserId, string hanhDong, string lyDoTuChoi, DateTime thoiGianXuLy)
        {
            var exists = context.NhatKyDuyetTins.Any(x =>
                x.TinTuyenDungId == tinTuyenDungId &&
                x.HanhDong == hanhDong &&
                x.ApplicationUserId == applicationUserId);

            if (exists)
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
            context.SaveChanges();
        }

        private void SeedApplications(ApplicationDbContext context, List<HoSoCaNhan> candidates, List<TinTuyenDung> jobs, List<HoSoCongTy> employers)
        {
            var openJobs = jobs
                .Where(x => x.TrangThaiTin == TrangThaiTinTuyenDung.DaDuyet && x.HanNopHoSo >= DateTime.Today)
                .OrderBy(x => x.TinTuyenDungId)
                .ToList();

            if (!openJobs.Any() || !candidates.Any())
            {
                return;
            }

            var statusPlan = BuildWeightedList(
                new Distribution(TrangThaiDonUngTuyen.DaNop, 18),
                new Distribution(TrangThaiDonUngTuyen.DaXem, 14),
                new Distribution(TrangThaiDonUngTuyen.VaoDanhSachNgan, 10),
                new Distribution(TrangThaiDonUngTuyen.BiTuChoi, 8),
                new Distribution(TrangThaiDonUngTuyen.DuocChapNhan, 6),
                new Distribution(TrangThaiDonUngTuyen.DaRut, 4));

            for (var i = 0; i < 60; i++)
            {
                var candidate = candidates[i % candidates.Count];
                var job = ResolveUniqueJobForCandidate(context, openJobs, candidate.HoSoCaNhanId, i);
                if (job == null)
                {
                    continue;
                }

                var cv = context.CVUngViens
                    .Where(x => x.HoSoCaNhanId == candidate.HoSoCaNhanId)
                    .OrderByDescending(x => x.TrangThaiSuDung)
                    .ThenBy(x => x.TenCV)
                    .FirstOrDefault();

                if (cv == null)
                {
                    continue;
                }

                var status = statusPlan[(i * 11 + 1) % statusPlan.Count];
                var employer = employers.FirstOrDefault(x => x.HoSoCongTyId == job.HoSoCongTyId);
                var handlerUserId = status == TrangThaiDonUngTuyen.DaRut || employer == null
                    ? candidate.ApplicationUserId
                    : employer.ApplicationUserId;

                var don = EnsureApplication(context, candidate, job, cv, status, i);
                EnsureApplicationHistory(context, don.DonUngTuyenId, candidate.ApplicationUserId, null, TrangThaiDonUngTuyen.DaNop, "Ứng viên nộp hồ sơ qua dữ liệu seed.", don.NgayNop);

                if (status != TrangThaiDonUngTuyen.DaNop)
                {
                    EnsureApplicationHistory(context, don.DonUngTuyenId, handlerUserId, TrangThaiDonUngTuyen.DaNop, status, BuildApplicationNote(status), don.NgayNop.AddDays(1 + (i % 5)));
                }
            }
        }

        private TinTuyenDung ResolveUniqueJobForCandidate(ApplicationDbContext context, List<TinTuyenDung> openJobs, int hoSoCaNhanId, int index)
        {
            for (var attempt = 0; attempt < openJobs.Count; attempt++)
            {
                var job = openJobs[((index * 7) + attempt) % openJobs.Count];
                var exists = context.DonUngTuyens.Any(x => x.HoSoCaNhanId == hoSoCaNhanId && x.TinTuyenDungId == job.TinTuyenDungId);
                if (!exists)
                {
                    return job;
                }

                if (attempt == 0)
                {
                    return job;
                }
            }

            return null;
        }

        private DonUngTuyen EnsureApplication(ApplicationDbContext context, HoSoCaNhan candidate, TinTuyenDung job, CVUngVien cv, string status, int index)
        {
            var don = context.DonUngTuyens.FirstOrDefault(x => x.HoSoCaNhanId == candidate.HoSoCaNhanId && x.TinTuyenDungId == job.TinTuyenDungId);
            if (don == null)
            {
                don = new DonUngTuyen
                {
                    HoSoCaNhanId = candidate.HoSoCaNhanId,
                    TinTuyenDungId = job.TinTuyenDungId
                };
                context.DonUngTuyens.Add(don);
            }

            don.CVUngVienId = cv.CVUngVienId;
            don.NgayNop = DateTime.Now.AddDays(-(3 + (index % 28)));
            don.TrangThaiDon = status;
            don.ThuGioiThieu = string.Format("Tôi quan tâm đến vị trí {0} và tin rằng kinh nghiệm hiện tại phù hợp với yêu cầu của doanh nghiệp.", job.TieuDe);
            don.GhiChuXuLy = status == TrangThaiDonUngTuyen.DaNop ? null : BuildApplicationNote(status);
            context.SaveChanges();
            return don;
        }

        private void EnsureApplicationHistory(ApplicationDbContext context, int donUngTuyenId, string applicationUserId, string oldStatus, string newStatus, string note, DateTime changedAt)
        {
            var exists = context.LichSuTrangThaiDons.Any(x => x.DonUngTuyenId == donUngTuyenId && x.TrangThaiMoi == newStatus);
            if (exists)
            {
                return;
            }

            context.LichSuTrangThaiDons.Add(new LichSuTrangThaiDon
            {
                DonUngTuyenId = donUngTuyenId,
                ApplicationUserId = applicationUserId,
                TrangThaiCu = oldStatus,
                TrangThaiMoi = newStatus,
                GhiChu = note,
                ThoiGianThayDoi = changedAt
            });
            context.SaveChanges();
        }

        private string BuildApplicationNote(string status)
        {
            switch (status)
            {
                case TrangThaiDonUngTuyen.DaXem:
                    return "Nhà tuyển dụng đã xem hồ sơ và đang đánh giá mức độ phù hợp.";
                case TrangThaiDonUngTuyen.VaoDanhSachNgan:
                    return "Ứng viên có kỹ năng phù hợp và được chuyển vào danh sách phỏng vấn sơ bộ.";
                case TrangThaiDonUngTuyen.BiTuChoi:
                    return "Hồ sơ chưa phù hợp với ưu tiên tuyển dụng hiện tại của vị trí.";
                case TrangThaiDonUngTuyen.DuocChapNhan:
                    return "Ứng viên được đánh giá phù hợp và chuyển sang bước trao đổi offer.";
                case TrangThaiDonUngTuyen.DaRut:
                    return "Ứng viên chủ động rút đơn vì đã thay đổi kế hoạch ứng tuyển.";
                default:
                    return "Hồ sơ đang được xử lý.";
            }
        }

        private class CompanySeed
        {
            public CompanySeed(string loginEmail, string tenCongTy, string moTa, string diaChi, string website, string industry = null, string logoPath = null)
            {
                LoginEmail = loginEmail;
                TenCongTy = tenCongTy;
                MoTa = moTa;
                DiaChi = diaChi;
                Website = website;
                Industry = industry;
                LogoPath = logoPath;
            }

            public string LoginEmail { get; private set; }
            public string TenCongTy { get; private set; }
            public string MoTa { get; private set; }
            public string DiaChi { get; private set; }
            public string Website { get; private set; }
            public string Industry { get; private set; }
            public string LogoPath { get; private set; }
        }

        private class CandidateSeed
        {
            public string Email { get; set; }
            public string HoTen { get; set; }
            public string DinhHuong { get; set; }
            public string NganhHoc { get; set; }
            public string Truong { get; set; }
            public string DiaChi { get; set; }
        }

        private class Distribution
        {
            public Distribution(string value, int count)
            {
                Value = value;
                Count = count;
            }

            public string Value { get; private set; }
            public int Count { get; private set; }
        }
    }
}
