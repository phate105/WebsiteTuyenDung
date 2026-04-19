using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteTuyenDung.Controllers
{
    public class HomeController : Controller
    {
        private readonly WebsiteTuyenDung.Models.ApplicationDbContext db = new WebsiteTuyenDung.Models.ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Nền tảng kết nối nhà tuyển dụng và ứng viên với quy trình đăng tin, duyệt tin và ứng tuyển bằng tiếng Việt.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Thông tin liên hệ và hỗ trợ người dùng của website tuyển dụng.";

            return View();
        }

        [Authorize(Roles = WebsiteTuyenDung.Models.Constants.ApplicationRoles.Admin)]
        public ActionResult AdminDashboard()
        {
            var nhaTuyenDungRole = db.Roles.FirstOrDefault(x => x.Name == WebsiteTuyenDung.Models.Constants.ApplicationRoles.NhaTuyenDung);
            var ungVienRole = db.Roles.FirstOrDefault(x => x.Name == WebsiteTuyenDung.Models.Constants.ApplicationRoles.UngVien);

            var model = new WebsiteTuyenDung.Models.ViewModels.AdminDashboardViewModel
            {
                SoTinChoDuyet = db.TinTuyenDungs.Count(x => x.TrangThaiTin == WebsiteTuyenDung.Models.Constants.TrangThaiTinTuyenDung.ChoDuyet),
                SoTinDaDuyet = db.TinTuyenDungs.Count(x => x.TrangThaiTin == WebsiteTuyenDung.Models.Constants.TrangThaiTinTuyenDung.DaDuyet),
                SoTinBiTuChoi = db.TinTuyenDungs.Count(x => x.TrangThaiTin == WebsiteTuyenDung.Models.Constants.TrangThaiTinTuyenDung.BiTuChoi),
                SoTinDaDong = db.TinTuyenDungs.Count(x => x.TrangThaiTin == WebsiteTuyenDung.Models.Constants.TrangThaiTinTuyenDung.DaDong),
                TongTinTuyenDung = db.TinTuyenDungs.Count(),
                TongDonUngTuyen = db.DonUngTuyens.Count(),
                TongTaiKhoan = db.Users.Count(),
                SoCongTy = db.HoSoCongTys.Count(),
                SoNhaTuyenDung = nhaTuyenDungRole == null ? 0 : db.Users.Count(x => x.Roles.Any(r => r.RoleId == nhaTuyenDungRole.Id)),
                SoUngVien = ungVienRole == null ? 0 : db.Users.Count(x => x.Roles.Any(r => r.RoleId == ungVienRole.Id))
            };

            return View("AdminTest", model);
        }

        [Authorize(Roles = WebsiteTuyenDung.Models.Constants.ApplicationRoles.NhaTuyenDung)]
        public ActionResult EmployerDashboard()
        {
            var userId = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
            var hoSoCongTy = db.HoSoCongTys.FirstOrDefault(x => x.ApplicationUserId == userId);

            var model = new WebsiteTuyenDung.Models.ViewModels.EmployerDashboardViewModel
            {
                HasCompanyProfile = hoSoCongTy != null,
                CompanyName = hoSoCongTy != null ? hoSoCongTy.TenCongTy : null
            };

            if (hoSoCongTy != null)
            {
                var tinQuery = db.TinTuyenDungs.Where(x => x.HoSoCongTyId == hoSoCongTy.HoSoCongTyId);
                var donQuery = db.DonUngTuyens.Where(x => x.TinTuyenDung.HoSoCongTyId == hoSoCongTy.HoSoCongTyId);

                model.SoTinNhap = tinQuery.Count(x => x.TrangThaiTin == WebsiteTuyenDung.Models.Constants.TrangThaiTinTuyenDung.Nhap);
                model.SoTinChoDuyet = tinQuery.Count(x => x.TrangThaiTin == WebsiteTuyenDung.Models.Constants.TrangThaiTinTuyenDung.ChoDuyet);
                model.SoTinDaDuyet = tinQuery.Count(x => x.TrangThaiTin == WebsiteTuyenDung.Models.Constants.TrangThaiTinTuyenDung.DaDuyet);
                model.SoTinBiTuChoi = tinQuery.Count(x => x.TrangThaiTin == WebsiteTuyenDung.Models.Constants.TrangThaiTinTuyenDung.BiTuChoi);
                model.SoHoSoChoXuLy = donQuery.Count(x => WebsiteTuyenDung.Models.Constants.TrangThaiDonUngTuyen.DangXuLy.Contains(x.TrangThaiDon));
                model.TongHoSoUngTuyen = donQuery.Count();
            }

            return View("EmployerTest", model);
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
