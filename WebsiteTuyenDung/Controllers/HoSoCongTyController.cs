using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebsiteTuyenDung.Models;
using WebsiteTuyenDung.Models.Constants;
using WebsiteTuyenDung.Models.Entities;
using WebsiteTuyenDung.Models.ViewModels;

namespace WebsiteTuyenDung.Controllers
{
    [Authorize(Roles = ApplicationRoles.NhaTuyenDung)]
    public class HoSoCongTyController : Controller
    {
        private static readonly string[] AllowedLogoExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private static readonly string[] AllowedLogoContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private const int LogoMaxSizeInBytes = 5 * 1024 * 1024;

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var model = GetHoSoCongTyHienTai();
            return View(model);
        }

        public ActionResult Create()
        {
            if (GetHoSoCongTyHienTai() != null)
            {
                TempData["ErrorMessage"] = "Tài khoản này đã có hồ sơ công ty. Bạn chỉ có thể chỉnh sửa hồ sơ hiện tại.";
                return RedirectToAction("Index");
            }

            return View(new HoSoCongTyFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HoSoCongTyFormViewModel model, HttpPostedFileBase logoFile)
        {
            model = model ?? new HoSoCongTyFormViewModel();
            if (GetHoSoCongTyHienTai() != null)
            {
                TempData["ErrorMessage"] = "Tài khoản này đã có hồ sơ công ty.";
                return RedirectToAction("Index");
            }

            ValidateHoSoCongTy(model, logoFile);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = new HoSoCongTy
            {
                ApplicationUserId = User.Identity.GetUserId(),
                TenCongTy = model.TenCongTy?.Trim(),
                MaSoThue = NullIfWhiteSpace(model.MaSoThue),
                MoTa = NullIfWhiteSpace(model.MoTa),
                DiaChi = NullIfWhiteSpace(model.DiaChi),
                Website = NormalizeUrl(model.Website),
                EmailLienHe = NullIfWhiteSpace(model.EmailLienHe),
                SoDienThoaiLienHe = NullIfWhiteSpace(model.SoDienThoaiLienHe),
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            if (logoFile != null && logoFile.ContentLength > 0)
            {
                entity.Logo = SaveLogoFile(logoFile);
            }

            db.HoSoCongTys.Add(entity);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã tạo hồ sơ công ty thành công.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            var entity = GetHoSoCongTyHienTai();
            if (entity == null)
            {
                TempData["ErrorMessage"] = "Bạn cần tạo hồ sơ công ty trước khi chỉnh sửa.";
                return RedirectToAction("Create");
            }

            if (id.HasValue && id.Value != entity.HoSoCongTyId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(MapToForm(entity));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HoSoCongTyFormViewModel model, HttpPostedFileBase logoFile)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var entity = GetHoSoCongTyHienTai();
            if (entity == null)
            {
                TempData["ErrorMessage"] = "Bạn cần tạo hồ sơ công ty trước khi chỉnh sửa.";
                return RedirectToAction("Create");
            }

            if (model.HoSoCongTyId != entity.HoSoCongTyId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            ValidateHoSoCongTy(model, logoFile);
            if (!ModelState.IsValid)
            {
                model.CurrentLogo = entity.Logo;
                return View(model);
            }

            entity.TenCongTy = model.TenCongTy?.Trim();
            entity.MaSoThue = NullIfWhiteSpace(model.MaSoThue);
            entity.MoTa = NullIfWhiteSpace(model.MoTa);
            entity.DiaChi = NullIfWhiteSpace(model.DiaChi);
            entity.Website = NormalizeUrl(model.Website);
            entity.EmailLienHe = NullIfWhiteSpace(model.EmailLienHe);
            entity.SoDienThoaiLienHe = NullIfWhiteSpace(model.SoDienThoaiLienHe);
            entity.NgayCapNhat = DateTime.Now;

            if (logoFile != null && logoFile.ContentLength > 0)
            {
                DeleteOldLocalLogo(entity.Logo);
                entity.Logo = SaveLogoFile(logoFile);
            }

            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã cập nhật hồ sơ công ty.";
            return RedirectToAction("Index");
        }

        private HoSoCongTy GetHoSoCongTyHienTai()
        {
            var userId = User.Identity.GetUserId();
            return db.HoSoCongTys.FirstOrDefault(x => x.ApplicationUserId == userId);
        }

        private static HoSoCongTyFormViewModel MapToForm(HoSoCongTy entity)
        {
            return new HoSoCongTyFormViewModel
            {
                HoSoCongTyId = entity.HoSoCongTyId,
                TenCongTy = entity.TenCongTy,
                MaSoThue = entity.MaSoThue,
                MoTa = entity.MoTa,
                DiaChi = entity.DiaChi,
                Website = entity.Website,
                EmailLienHe = entity.EmailLienHe,
                SoDienThoaiLienHe = entity.SoDienThoaiLienHe,
                CurrentLogo = entity.Logo
            };
        }

        private void ValidateHoSoCongTy(HoSoCongTyFormViewModel model, HttpPostedFileBase logoFile)
        {
            model.TenCongTy = model.TenCongTy?.Trim();
            model.MaSoThue = model.MaSoThue?.Trim();
            model.EmailLienHe = model.EmailLienHe?.Trim();
            model.SoDienThoaiLienHe = model.SoDienThoaiLienHe?.Trim();
            model.Website = model.Website?.Trim();

            if (string.IsNullOrWhiteSpace(model.TenCongTy))
            {
                ModelState.AddModelError(nameof(model.TenCongTy), "Vui lòng nhập tên công ty.");
            }

            if (!string.IsNullOrWhiteSpace(model.Website) && !IsValidWebsiteUrl(model.Website))
            {
                ModelState.AddModelError(nameof(model.Website), "Website không hợp lệ.");
            }

            if (!string.IsNullOrWhiteSpace(model.SoDienThoaiLienHe) && !IsValidPhoneNumber(model.SoDienThoaiLienHe))
            {
                ModelState.AddModelError(nameof(model.SoDienThoaiLienHe), "Số điện thoại liên hệ không hợp lệ.");
            }

            if (logoFile == null || logoFile.ContentLength <= 0)
            {
                return;
            }

            var extension = Path.GetExtension(logoFile.FileName);
            if (!AllowedLogoExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.CurrentLogo), "Logo phải là tệp ảnh JPG, PNG, GIF hoặc WEBP.");
            }

            if (logoFile.ContentLength > LogoMaxSizeInBytes)
            {
                ModelState.AddModelError(nameof(model.CurrentLogo), "Dung lượng logo tối đa là 5MB.");
            }

            if (!string.IsNullOrWhiteSpace(logoFile.ContentType) &&
                !AllowedLogoContentTypes.Contains(logoFile.ContentType, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.CurrentLogo), "Nội dung tệp logo phải là ảnh JPG, PNG, GIF hoặc WEBP.");
            }
        }

        private string SaveLogoFile(HttpPostedFileBase logoFile)
        {
            var extension = Path.GetExtension(logoFile.FileName);
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var logoFolder = Server.MapPath("~/Uploads/Logo");
            Directory.CreateDirectory(logoFolder);
            var filePath = Path.Combine(logoFolder, fileName);
            logoFile.SaveAs(filePath);
            return $"/Uploads/Logo/{fileName}";
        }

        private void DeleteOldLocalLogo(string currentLogo)
        {
            if (string.IsNullOrWhiteSpace(currentLogo) || !currentLogo.StartsWith("/Uploads/Logo/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var physicalPath = Server.MapPath("~" + currentLogo);
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
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
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
