using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebsiteTuyenDung.Models;
using WebsiteTuyenDung.Models.Constants;
using WebsiteTuyenDung.Models.Services;
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
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
        }

        public ActionResult Index(string vaiTro, string trangThai)
        {
            vaiTro = NormalizeRoleFilter(vaiTro);
            trangThai = NormalizeStatusFilter(trangThai);

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

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            var user = userManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(BuildEditViewModel(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminTaiKhoanEditViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.UserId))
            {
                return HttpNotFound();
            }

            var user = userManager.FindById(model.UserId);
            if (user == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var isCurrentUser = string.Equals(user.Id, currentUserId, StringComparison.OrdinalIgnoreCase);
            model.LaTaiKhoanHienTai = isCurrentUser;
            model.VaiTro = GetPrimaryRole(user);
            ModelState.Remove(nameof(model.VaiTro));

            model.Email = (model.Email ?? string.Empty).Trim();
            model.UserName = (model.UserName ?? string.Empty).Trim();
            model.HoTen = NullIfWhiteSpace(model.HoTen);
            model.SoDienThoai = NullIfWhiteSpace(model.SoDienThoai);
            model.DiaChi = NullIfWhiteSpace(model.DiaChi);
            model.TenCongTy = NullIfWhiteSpace(model.TenCongTy);
            model.Website = NullIfWhiteSpace(model.Website);
            model.EmailLienHe = NullIfWhiteSpace(model.EmailLienHe);
            model.SoDienThoaiLienHe = NullIfWhiteSpace(model.SoDienThoaiLienHe);
            model.DiaChiCongTy = NullIfWhiteSpace(model.DiaChiCongTy);

            if (isCurrentUser && model.BiKhoa)
            {
                ModelState.AddModelError("BiKhoa", "Không thể tự khóa tài khoản admin đang đăng nhập.");
            }

            if (!string.IsNullOrWhiteSpace(model.Website) && !IsValidWebsiteUrl(model.Website))
            {
                ModelState.AddModelError("Website", "Website công ty không hợp lệ.");
            }

            if (!IsValidPhoneNumber(model.SoDienThoai))
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại ứng viên không hợp lệ.");
            }

            if (!IsValidPhoneNumber(model.SoDienThoaiLienHe))
            {
                ModelState.AddModelError("SoDienThoaiLienHe", "Số điện thoại công ty không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                PopulateLinkedProfileFlags(model);
                return View(model);
            }

            model.Website = NormalizeUrl(model.Website);

            user.Email = model.Email;
            user.UserName = model.UserName;
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.PasswordHash = userManager.PasswordHasher.HashPassword(model.NewPassword);
                user.SecurityStamp = Guid.NewGuid().ToString();
            }

            var updateResult = userManager.Update(user);
            if (!updateResult.Succeeded)
            {
                AddIdentityErrors(updateResult);
                PopulateLinkedProfileFlags(model);
                return View(model);
            }

            if (!isCurrentUser)
            {
                var lockoutResult = model.BiKhoa
                    ? userManager.SetLockoutEndDate(user.Id, DateTimeOffset.UtcNow.AddYears(100))
                    : userManager.SetLockoutEndDate(user.Id, DateTimeOffset.UtcNow.AddMinutes(-1));
                var enableResult = userManager.SetLockoutEnabled(user.Id, true);
                var resetResult = model.BiKhoa ? IdentityResult.Success : userManager.ResetAccessFailedCount(user.Id);

                if (!lockoutResult.Succeeded || !enableResult.Succeeded || !resetResult.Succeeded)
                {
                    TempData["ErrorMessage"] = string.Join("; ", lockoutResult.Errors.Concat(enableResult.Errors).Concat(resetResult.Errors));
                    PopulateLinkedProfileFlags(model);
                    return View(model);
                }
            }

            UpdateLinkedProfile(user.Id, model);
            db.SaveChanges();
            AuditLogService.Write(
                currentUserId,
                User.Identity.Name,
                "TaiKhoan.Edit",
                user.Id,
                $"Email={user.Email}; Role={model.VaiTro}; Locked={model.BiKhoa}; PasswordChanged={!string.IsNullOrWhiteSpace(model.NewPassword)}");

            TempData["SuccessMessage"] = $"Đã cập nhật tài khoản {user.Email}.";
            return RedirectToAction("Index");
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
            AuditLogService.Write(User.Identity.GetUserId(), User.Identity.Name, "TaiKhoan.Khoa", user.Id, $"Email={user.Email}");
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
            AuditLogService.Write(User.Identity.GetUserId(), User.Identity.Name, "TaiKhoan.MoKhoa", user.Id, $"Email={user.Email}");
            return RedirectToAction("Index", new { vaiTro, trangThai });
        }

        private AdminTaiKhoanEditViewModel BuildEditViewModel(ApplicationUser user)
        {
            var vaiTro = GetPrimaryRole(user);
            var hoSoCaNhan = db.HoSoCaNhans.FirstOrDefault(x => x.ApplicationUserId == user.Id);
            var hoSoCongTy = db.HoSoCongTys.FirstOrDefault(x => x.ApplicationUserId == user.Id);
            var biKhoa = user.LockoutEnabled && user.LockoutEndDateUtc.HasValue && user.LockoutEndDateUtc.Value > DateTime.UtcNow;

            return new AdminTaiKhoanEditViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                VaiTro = vaiTro,
                BiKhoa = biKhoa,
                LaTaiKhoanHienTai = string.Equals(user.Id, User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase),
                CoHoSoCaNhan = hoSoCaNhan != null,
                CoHoSoCongTy = hoSoCongTy != null,
                HoTen = hoSoCaNhan?.HoTen,
                SoDienThoai = hoSoCaNhan?.SoDienThoai,
                DiaChi = hoSoCaNhan?.DiaChi,
                TenCongTy = hoSoCongTy?.TenCongTy,
                Website = hoSoCongTy?.Website,
                EmailLienHe = hoSoCongTy?.EmailLienHe,
                SoDienThoaiLienHe = hoSoCongTy?.SoDienThoaiLienHe,
                DiaChiCongTy = hoSoCongTy?.DiaChi
            };
        }

        private void PopulateLinkedProfileFlags(AdminTaiKhoanEditViewModel model)
        {
            model.CoHoSoCaNhan = db.HoSoCaNhans.Any(x => x.ApplicationUserId == model.UserId);
            model.CoHoSoCongTy = db.HoSoCongTys.Any(x => x.ApplicationUserId == model.UserId);
        }

        private void UpdateLinkedProfile(string userId, AdminTaiKhoanEditViewModel model)
        {
            var hoSoCaNhan = db.HoSoCaNhans.FirstOrDefault(x => x.ApplicationUserId == userId);
            if (hoSoCaNhan != null)
            {
                if (!string.IsNullOrWhiteSpace(model.HoTen))
                {
                    hoSoCaNhan.HoTen = model.HoTen;
                }

                hoSoCaNhan.SoDienThoai = model.SoDienThoai;
                hoSoCaNhan.DiaChi = model.DiaChi;
                hoSoCaNhan.NgayCapNhat = DateTime.Now;
            }
            else if (model.VaiTro == ApplicationRoles.UngVien && !string.IsNullOrWhiteSpace(model.HoTen))
            {
                db.HoSoCaNhans.Add(new Models.Entities.HoSoCaNhan
                {
                    ApplicationUserId = userId,
                    HoTen = model.HoTen,
                    SoDienThoai = model.SoDienThoai,
                    DiaChi = model.DiaChi,
                    NgayCapNhat = DateTime.Now
                });
            }

            var hoSoCongTy = db.HoSoCongTys.FirstOrDefault(x => x.ApplicationUserId == userId);
            if (hoSoCongTy != null)
            {
                if (!string.IsNullOrWhiteSpace(model.TenCongTy))
                {
                    hoSoCongTy.TenCongTy = model.TenCongTy;
                }

                hoSoCongTy.Website = model.Website;
                hoSoCongTy.EmailLienHe = model.EmailLienHe;
                hoSoCongTy.SoDienThoaiLienHe = model.SoDienThoaiLienHe;
                hoSoCongTy.DiaChi = model.DiaChiCongTy;
                hoSoCongTy.NgayCapNhat = DateTime.Now;
            }
            else if (model.VaiTro == ApplicationRoles.NhaTuyenDung && !string.IsNullOrWhiteSpace(model.TenCongTy))
            {
                db.HoSoCongTys.Add(new Models.Entities.HoSoCongTy
                {
                    ApplicationUserId = userId,
                    TenCongTy = model.TenCongTy,
                    Website = model.Website,
                    EmailLienHe = model.EmailLienHe,
                    SoDienThoaiLienHe = model.SoDienThoaiLienHe,
                    DiaChi = model.DiaChiCongTy,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                });
            }
        }

        private string GetPrimaryRole(ApplicationUser user)
        {
            var roleLookup = db.Roles.ToDictionary(x => x.Id, x => x.Name);
            return user.Roles
                .Select(x => roleLookup.ContainsKey(x.RoleId) ? roleLookup[x.RoleId] : null)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? "Chưa phân quyền";
        }

        private static string NormalizeRoleFilter(string role)
        {
            role = role?.Trim();
            if (role == ApplicationRoles.Admin || role == ApplicationRoles.NhaTuyenDung || role == ApplicationRoles.UngVien)
            {
                return role;
            }

            return null;
        }

        private static string NormalizeStatusFilter(string status)
        {
            status = status?.Trim();
            if (string.Equals(status, "DaKhoa", StringComparison.OrdinalIgnoreCase))
            {
                return "DaKhoa";
            }

            if (string.Equals(status, "DangHoatDong", StringComparison.OrdinalIgnoreCase))
            {
                return "DangHoatDong";
            }

            return null;
        }

        private void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private static string NullIfWhiteSpace(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static string NormalizeUrl(string website)
        {
            if (string.IsNullOrWhiteSpace(website))
            {
                return null;
            }

            website = website.Trim();
            if (website.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || website.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return website;
            }

            return "https://" + website;
        }

        private static bool IsValidWebsiteUrl(string website)
        {
            var normalizedUrl = NormalizeUrl(website);
            Uri uri;
            return Uri.TryCreate(normalizedUrl, UriKind.Absolute, out uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return true;
            }

            var value = phoneNumber.Trim();
            if (value.Length < 8 || value.Length > 20)
            {
                return false;
            }

            return value.All(ch => char.IsDigit(ch) || ch == '+' || ch == '-' || ch == ' ' || ch == '(' || ch == ')' || ch == '.');
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
