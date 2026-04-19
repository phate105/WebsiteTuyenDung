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
    public class LoaiHinhLamViecController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var data = db.LoaiHinhLamViecs.OrderBy(x => x.TenLoaiHinhLamViec).ToList();
            return View(data);
        }

        public ActionResult Create()
        {
            return View(new LoaiHinhLamViec { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoaiHinhLamViec model)
        {
            ValidateTen(model?.TenLoaiHinhLamViec, null);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.TenLoaiHinhLamViec = model.TenLoaiHinhLamViec.Trim();
            db.LoaiHinhLamViecs.Add(model);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã thêm loại hình làm việc mới.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var entity = db.LoaiHinhLamViecs.Find(id.Value);
            if (entity == null)
            {
                return HttpNotFound();
            }

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoaiHinhLamViec model)
        {
            ValidateTen(model?.TenLoaiHinhLamViec, model?.LoaiHinhLamViecId);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = db.LoaiHinhLamViecs.Find(model.LoaiHinhLamViecId);
            if (entity == null)
            {
                return HttpNotFound();
            }

            entity.TenLoaiHinhLamViec = model.TenLoaiHinhLamViec.Trim();
            entity.TrangThai = model.TrangThai;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã cập nhật loại hình làm việc.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var entity = db.LoaiHinhLamViecs.Find(id);
            if (entity == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại hình làm việc cần xử lý.";
                return RedirectToAction("Index");
            }

            if (db.TinTuyenDungs.Any(x => x.LoaiHinhLamViecId == id))
            {
                entity.TrangThai = false;
                db.SaveChanges();
                TempData["ErrorMessage"] = "Loại hình làm việc này đã được dùng trong tin tuyển dụng, hệ thống chuyển sang trạng thái ngừng sử dụng.";
                return RedirectToAction("Index");
            }

            db.LoaiHinhLamViecs.Remove(entity);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã xóa loại hình làm việc.";
            return RedirectToAction("Index");
        }

        private void ValidateTen(string ten, int? currentId)
        {
            if (string.IsNullOrWhiteSpace(ten))
            {
                ModelState.AddModelError("TenLoaiHinhLamViec", "Vui lòng nhập tên loại hình làm việc.");
                return;
            }

            var normalized = ten.Trim();
            var normalizedLower = normalized.ToLower();
            var existed = db.LoaiHinhLamViecs.Any(x =>
                x.TenLoaiHinhLamViec.ToLower() == normalizedLower
                && (!currentId.HasValue || x.LoaiHinhLamViecId != currentId.Value));

            if (existed)
            {
                ModelState.AddModelError("TenLoaiHinhLamViec", "Tên loại hình làm việc đã tồn tại.");
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
