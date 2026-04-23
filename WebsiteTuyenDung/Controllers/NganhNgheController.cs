using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebsiteTuyenDung.Models;
using WebsiteTuyenDung.Models.Constants;
using WebsiteTuyenDung.Models.Entities;

namespace WebsiteTuyenDung.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class NganhNgheController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var data = db.NganhNghes.OrderBy(x => x.TenNganhNghe).ToList();
            return View(data);
        }

        public ActionResult Create()
        {
            return View(new NganhNghe { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NganhNghe model)
        {
            model = model ?? new NganhNghe();
            ValidateTen(model?.TenNganhNghe, null);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.TenNganhNghe = model.TenNganhNghe.Trim();
            db.NganhNghes.Add(model);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã thêm ngành nghề mới.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var entity = db.NganhNghes.Find(id.Value);
            if (entity == null)
            {
                return HttpNotFound();
            }

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NganhNghe model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ValidateTen(model?.TenNganhNghe, model?.NganhNgheId);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = db.NganhNghes.Find(model.NganhNgheId);
            if (entity == null)
            {
                return HttpNotFound();
            }

            entity.TenNganhNghe = model.TenNganhNghe.Trim();
            entity.TrangThai = model.TrangThai;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã cập nhật ngành nghề.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var entity = db.NganhNghes.Find(id);
            if (entity == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy ngành nghề cần xử lý.";
                return RedirectToAction("Index");
            }

            if (db.TinTuyenDungs.Any(x => x.NganhNgheId == id))
            {
                entity.TrangThai = false;
                db.SaveChanges();
                TempData["ErrorMessage"] = "Ngành nghề này đã được dùng trong tin tuyển dụng, hệ thống chuyển sang trạng thái ngừng sử dụng.";
                return RedirectToAction("Index");
            }

            db.NganhNghes.Remove(entity);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã xóa ngành nghề.";
            return RedirectToAction("Index");
        }

        private void ValidateTen(string ten, int? currentId)
        {
            if (string.IsNullOrWhiteSpace(ten))
            {
                ModelState.AddModelError("TenNganhNghe", "Vui lòng nhập tên ngành nghề.");
                return;
            }

            var normalized = ten.Trim();
            if (normalized.Length > 100)
            {
                ModelState.AddModelError("TenNganhNghe", "Tên ngành nghề tối đa 100 ký tự.");
                return;
            }

            var normalizedLower = normalized.ToLower();
            var existed = db.NganhNghes.Any(x =>
                x.TenNganhNghe.ToLower() == normalizedLower
                && (!currentId.HasValue || x.NganhNgheId != currentId.Value));

            if (existed)
            {
                ModelState.AddModelError("TenNganhNghe", "Tên ngành nghề đã tồn tại.");
            }
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
