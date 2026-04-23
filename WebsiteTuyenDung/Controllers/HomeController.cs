using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebsiteTuyenDung.Models;
using WebsiteTuyenDung.Models.Constants;
using WebsiteTuyenDung.Models.ViewModels;

namespace WebsiteTuyenDung.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var homNay = DateTime.Today;
            var model = new HomeIndexViewModel
            {
                NganhNgheOptions = new SelectList(
                    db.NganhNghes.Where(x => x.TrangThai).OrderBy(x => x.TenNganhNghe),
                    "NganhNgheId",
                    "TenNganhNghe"),
                DiaDiemOptions = new SelectList(
                    db.DiaDiems.Where(x => x.TrangThai).OrderBy(x => x.TenDiaDiem),
                    "DiaDiemId",
                    "TenDiaDiem"),
                NganhNgheNoiBat = db.NganhNghes
                    .Where(x => x.TrangThai)
                    .Select(x => new PublicHomeCategoryViewModel
                    {
                        NganhNgheId = x.NganhNgheId,
                        TenNganhNghe = x.TenNganhNghe,
                        SoLuongTin = x.TinTuyenDungs.Count(t => t.TrangThaiTin == TrangThaiTinTuyenDung.DaDuyet && t.HanNopHoSo >= homNay)
                    })
                    .Where(x => x.SoLuongTin > 0)
                    .OrderByDescending(x => x.SoLuongTin)
                    .ThenBy(x => x.TenNganhNghe)
                    .Take(8)
                    .ToList(),
                TinTuyenDungMoiNhat = GetCongViecDangMoQuery()
                    .OrderByDescending(x => x.NgayDang ?? x.NgayTao)
                    .Take(48)
                    .ToList()
                    .Select(MapToJobCard)
                    .ToList(),
                NhaTuyenDungTieuBieu = db.HoSoCongTys
                    .Select(x => new
                    {
                        x.HoSoCongTyId,
                        x.TenCongTy,
                        x.Logo,
                        x.DiaChi,
                        x.Website,
                        SoLuongTinDangTuyen = x.TinTuyenDungs.Count(t => t.TrangThaiTin == TrangThaiTinTuyenDung.DaDuyet && t.HanNopHoSo >= homNay)
                    })
                    .Where(x => x.SoLuongTinDangTuyen > 0)
                    .ToList()
                    .Where(x => !IsExcludedEmployerBrand(x.TenCongTy) && !IsExcludedEmployerBrand(x.Logo))
                    .OrderByDescending(x => IsPriorityEmployerLogo(x.Logo))
                    .ThenByDescending(x => !string.IsNullOrWhiteSpace(x.Logo))
                    .ThenByDescending(x => x.SoLuongTinDangTuyen)
                    .ThenBy(x => x.TenCongTy)
                    .Take(12)
                    .Select(x => new FeaturedEmployerViewModel
                    {
                        HoSoCongTyId = x.HoSoCongTyId,
                        TenCongTy = x.TenCongTy,
                        LogoCongTy = NormalizeUrl(x.Logo),
                        DiaChi = x.DiaChi,
                        Website = x.Website,
                        SoLuongTinDangTuyen = x.SoLuongTinDangTuyen
                    })
                    .ToList()
            };

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "VietJob là nền tảng kết nối nhà tuyển dụng và ứng viên với quy trình đăng tin, duyệt tin và ứng tuyển bằng tiếng Việt.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Thông tin liên hệ và hỗ trợ người dùng của VietJob.";
            return View();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        public ActionResult AdminDashboard()
        {
            var nhaTuyenDungRole = db.Roles.FirstOrDefault(x => x.Name == ApplicationRoles.NhaTuyenDung);
            var ungVienRole = db.Roles.FirstOrDefault(x => x.Name == ApplicationRoles.UngVien);

            var model = new AdminDashboardViewModel
            {
                SoTinChoDuyet = db.TinTuyenDungs.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.ChoDuyet),
                SoTinDaDuyet = db.TinTuyenDungs.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.DaDuyet),
                SoTinBiTuChoi = db.TinTuyenDungs.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.BiTuChoi),
                SoTinDaDong = db.TinTuyenDungs.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.DaDong),
                TongTinTuyenDung = db.TinTuyenDungs.Count(),
                TongDonUngTuyen = db.DonUngTuyens.Count(),
                TongTaiKhoan = db.Users.Count(),
                SoCongTy = db.HoSoCongTys.Count(),
                SoNhaTuyenDung = nhaTuyenDungRole == null ? 0 : db.Users.Count(x => x.Roles.Any(r => r.RoleId == nhaTuyenDungRole.Id)),
                SoUngVien = ungVienRole == null ? 0 : db.Users.Count(x => x.Roles.Any(r => r.RoleId == ungVienRole.Id))
            };

            return View("AdminTest", model);
        }

        [Authorize(Roles = ApplicationRoles.NhaTuyenDung)]
        public ActionResult EmployerDashboard()
        {
            var userId = User.Identity.GetUserId();
            var hoSoCongTy = db.HoSoCongTys.FirstOrDefault(x => x.ApplicationUserId == userId);

            var model = new EmployerDashboardViewModel
            {
                HasCompanyProfile = hoSoCongTy != null,
                CompanyName = hoSoCongTy != null ? hoSoCongTy.TenCongTy : null
            };

            if (hoSoCongTy != null)
            {
                var tinQuery = db.TinTuyenDungs.Where(x => x.HoSoCongTyId == hoSoCongTy.HoSoCongTyId);
                var donQuery = db.DonUngTuyens.Where(x => x.TinTuyenDung.HoSoCongTyId == hoSoCongTy.HoSoCongTyId);

                model.SoTinNhap = tinQuery.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.Nhap);
                model.SoTinChoDuyet = tinQuery.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.ChoDuyet);
                model.SoTinDaDuyet = tinQuery.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.DaDuyet);
                model.SoTinBiTuChoi = tinQuery.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.BiTuChoi);
                model.SoHoSoChoXuLy = donQuery.Count(x => TrangThaiDonUngTuyen.DangXuLy.Contains(x.TrangThaiDon));
                model.TongHoSoUngTuyen = donQuery.Count();
            }

            return View("EmployerTest", model);
        }

        private IQueryable<Models.Entities.TinTuyenDung> GetCongViecDangMoQuery()
        {
            var homNay = DateTime.Today;

            return db.TinTuyenDungs
                .Include(x => x.HoSoCongTy)
                .Include(x => x.NganhNghe)
                .Include(x => x.DiaDiem)
                .Include(x => x.LoaiHinhLamViec)
                .Include(x => x.CapDoKinhNghiem)
                .Where(x => x.TrangThaiTin == TrangThaiTinTuyenDung.DaDuyet && x.HanNopHoSo >= homNay);
        }

        private static PublicJobCardViewModel MapToJobCard(Models.Entities.TinTuyenDung tinTuyenDung)
        {
            return new PublicJobCardViewModel
            {
                TinTuyenDungId = tinTuyenDung.TinTuyenDungId,
                HoSoCongTyId = tinTuyenDung.HoSoCongTyId,
                TieuDe = tinTuyenDung.TieuDe,
                TenCongTy = tinTuyenDung.HoSoCongTy != null ? tinTuyenDung.HoSoCongTy.TenCongTy : "Doanh nghiệp",
                LogoCongTy = NormalizeUrl(tinTuyenDung.HoSoCongTy != null ? tinTuyenDung.HoSoCongTy.Logo : null),
                TenDiaDiem = tinTuyenDung.DiaDiem != null ? tinTuyenDung.DiaDiem.TenDiaDiem : "Chưa cập nhật",
                TenLoaiHinhLamViec = tinTuyenDung.LoaiHinhLamViec != null ? tinTuyenDung.LoaiHinhLamViec.TenLoaiHinhLamViec : "Chưa cập nhật",
                TenNganhNghe = tinTuyenDung.NganhNghe != null ? tinTuyenDung.NganhNghe.TenNganhNghe : "Chưa cập nhật",
                TenCapDoKinhNghiem = tinTuyenDung.CapDoKinhNghiem != null ? tinTuyenDung.CapDoKinhNghiem.TenCapDoKinhNghiem : "Chưa cập nhật",
                LuongHienThi = BuildSalaryText(tinTuyenDung.LuongToiThieu, tinTuyenDung.LuongToiDa),
                NgayDang = tinTuyenDung.NgayDang,
                HanNopHoSo = tinTuyenDung.HanNopHoSo,
                CoTheUngTuyen = tinTuyenDung.HanNopHoSo.Date >= DateTime.Today
            };
        }

        private static string BuildSalaryText(decimal? luongToiThieu, decimal? luongToiDa)
        {
            if (!luongToiThieu.HasValue && !luongToiDa.HasValue)
            {
                return "Thỏa thuận";
            }

            if (luongToiThieu.HasValue && luongToiDa.HasValue)
            {
                return string.Format("{0:#,##0} - {1:#,##0} VNĐ", luongToiThieu.Value, luongToiDa.Value);
            }

            if (luongToiThieu.HasValue)
            {
                return string.Format("Từ {0:#,##0} VNĐ", luongToiThieu.Value);
            }

            return string.Format("Đến {0:#,##0} VNĐ", luongToiDa.Value);
        }

        private static string NormalizeUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (value.StartsWith("http", StringComparison.OrdinalIgnoreCase) || value.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }

            return "/" + value.TrimStart('/');
        }

        private static bool IsExcludedEmployerBrand(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalized = value.ToLowerInvariant();
            return normalized.Contains("shopee")
                || normalized.Contains("lazada")
                || normalized.Contains("yamaha")
                || normalized.Contains("alpha");
        }

        private static bool IsPriorityEmployerLogo(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalized = value.ToLowerInvariant();
            return normalized.Contains("techcombank2")
                || normalized.Contains("honda2")
                || normalized.Contains("lg-logo");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
