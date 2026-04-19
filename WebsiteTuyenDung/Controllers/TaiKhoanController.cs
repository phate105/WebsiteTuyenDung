using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebsiteTuyenDung.Models;
using WebsiteTuyenDung.Models.Constants;
using WebsiteTuyenDung.Models.ViewModels;

namespace WebsiteTuyenDung.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly UserManager<ApplicationUser> userManager;

        public TaiKhoanController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public ActionResult Index(string vaiTro, string trangThai)
        {
            var currentUserId = User.Identity.GetUserId();
            var roleLookup = db.Roles.ToDictionary(x => x.Id, x => x.Name);
            var companyLookup = db.HoSoCongTys.ToDictionary(x => x.ApplicationUserId, x => x.TenCongTy);
            var candidateLookup = db.HoSoCaNhans.ToDictionary(x => x.ApplicationUserId, x => x.HoTen);
            var soTinLookup = db.HoSoCongTys
                .Select(x => new
                {
                    x.ApplicationUserId,
                    SoTin = x.TinTuyenDungs.Count()
                })
                .ToDictionary(x => x.ApplicationUserId, x => x.SoTin);
            var soDonLookup = db.HoSoCaNhans
                .Select(x => new
                {
                    x.ApplicationUserId,
                    SoDon = x.DonUngTuyens.Count()
                })
                .ToDictionary(x => x.ApplicationUserId, x => x.SoDon);

            var allUsers = db.Users.OrderBy(x => x.Email).ToList();
            var allItems = allUsers.Select(user =>
            {
                var vaiTroChinh = user.Roles
                    .Select(x => roleLookup.ContainsKey(x.RoleId) ? roleLookup[x.RoleId] : null)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? "Chưa phân quyền";
                var biKhoa = user.LockoutEnabled && user.LockoutEndDateUtc.HasValue && user.LockoutEndDateUtc.Value > DateTime.UtcNow;
                var thongTinLienKet =
                    vaiTroChinh == ApplicationRoles.NhaTuyenDung && companyLookup.ContainsKey(user.Id)
                        ? companyLookup[user.Id]
                        : vaiTroChinh == ApplicationRoles.UngVien && candidateLookup.ContainsKey(user.Id)
                            ? candidateLookup[user.Id]
                            : vaiTroChinh == ApplicationRoles.Admin
                                ? "Quản trị hệ thống"
                                : "Chưa có hồ sơ liên kết";

                return new AdminTaiKhoanItemViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    VaiTro = vaiTroChinh,
                    ThongTinLienKet = thongTinLienKet,
                    BiKhoa = biKhoa,
                    KhoaDenLuc = user.LockoutEndDateUtc,
                    SoTinTuyenDung = soTinLookup.ContainsKey(user.Id) ? soTinLookup[user.Id] : 0,
                    SoDonUngTuyen = soDonLookup.ContainsKey(user.Id) ? soDonLookup[user.Id] : 0,
                    LaTaiKhoanHienTai = string.Equals(user.Id, currentUserId, StringComparison.OrdinalIgnoreCase)
                };
            }).ToList();

            var data = allItems.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(vaiTro))
            {
                data = data.Where(x => string.Equals(x.VaiTro, vaiTro, StringComparison.OrdinalIgnoreCase));
            }

            if (string.Equals(trangThai, "DaKhoa", StringComparison.OrdinalIgnoreCase))
            {
                data = data.Where(x => x.BiKhoa);
            }
            else if (string.Equals(trangThai, "DangHoatDong", StringComparison.OrdinalIgnoreCase))
            {
                data = data.Where(x => !x.BiKhoa);
            }

            var tongBiKhoa = allItems.Count(x => x.BiKhoa);
            var model = new AdminTaiKhoanIndexViewModel
            {
                TaiKhoans = data.ToList(),
                VaiTro = vaiTro,
                TrangThai = trangThai,
                TongTaiKhoan = allItems.Count,
                TongDangHoatDong = allItems.Count - tongBiKhoa,
                TongBiKhoa = tongBiKhoa
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Khoa(string id, string vaiTro, string trangThai)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Không xác định được tài khoản cần khóa.";
                return RedirectToAction("Index", new { vaiTro, trangThai });
            }

            var currentUserId = User.Identity.GetUserId();
            if (string.Equals(id, currentUserId, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Bạn không thể tự khóa tài khoản admin đang đăng nhập.";
                return RedirectToAction("Index", new { vaiTro, trangThai });
            }

            var user = userManager.FindById(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản cần khóa.";
                return RedirectToAction("Index", new { vaiTro, trangThai });
            }

            var enableResult = userManager.SetLockoutEnabled(user.Id, true);
            var lockResult = userManager.SetLockoutEndDate(user.Id, DateTimeOffset.UtcNow.AddYears(100));
            if (!enableResult.Succeeded || !lockResult.Succeeded)
            {
                TempData["ErrorMessage"] = string.Join("; ", enableResult.Errors.Concat(lockResult.Errors));
                return RedirectToAction("Index", new { vaiTro, trangThai });
            }

            TempData["SuccessMessage"] = $"Đã khóa tài khoản {user.Email}.";
            return RedirectToAction("Index", new { vaiTro, trangThai });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoKhoa(string id, string vaiTro, string trangThai)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Không xác định được tài khoản cần mở khóa.";
                return RedirectToAction("Index", new { vaiTro, trangThai });
            }

            var user = userManager.FindById(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản cần mở khóa.";
                return RedirectToAction("Index", new { vaiTro, trangThai });
            }

            var enableResult = userManager.SetLockoutEnabled(user.Id, true);
            var unlockResult = userManager.SetLockoutEndDate(user.Id, DateTimeOffset.UtcNow.AddMinutes(-1));
            var resetResult = userManager.ResetAccessFailedCount(user.Id);
            if (!enableResult.Succeeded || !unlockResult.Succeeded || !resetResult.Succeeded)
            {
                TempData["ErrorMessage"] = string.Join("; ", enableResult.Errors.Concat(unlockResult.Errors).Concat(resetResult.Errors));
                return RedirectToAction("Index", new { vaiTro, trangThai });
            }

            TempData["SuccessMessage"] = $"Đã mở khóa tài khoản {user.Email}.";
            return RedirectToAction("Index", new { vaiTro, trangThai });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                userManager.Dispose();
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
