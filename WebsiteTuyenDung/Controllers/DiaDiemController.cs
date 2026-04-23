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
    public class DiaDiemController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var data = db.DiaDiems.OrderBy(x => x.TenDiaDiem).ToList();
            return View(data);
        }

        public ActionResult Create()
        {
            return View(new DiaDiem { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DiaDiem model)
        {
            model = model ?? new DiaDiem();
            ValidateTen(model?.TenDiaDiem, null);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.TenDiaDiem = model.TenDiaDiem.Trim();
            db.DiaDiems.Add(model);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã thêm địa điểm mới.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var entity = db.DiaDiems.Find(id.Value);
            if (entity == null)
            {
                return HttpNotFound();
            }

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DiaDiem model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ValidateTen(model?.TenDiaDiem, model?.DiaDiemId);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = db.DiaDiems.Find(model.DiaDiemId);
            if (entity == null)
            {
                return HttpNotFound();
            }

            entity.TenDiaDiem = model.TenDiaDiem.Trim();
            entity.TrangThai = model.TrangThai;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã cập nhật địa điểm.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var entity = db.DiaDiems.Find(id);
            if (entity == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy địa điểm cần xử lý.";
                return RedirectToAction("Index");
            }

            if (db.TinTuyenDungs.Any(x => x.DiaDiemId == id))
            {
                entity.TrangThai = false;
                db.SaveChanges();
                TempData["ErrorMessage"] = "Địa điểm này đã được dùng trong tin tuyển dụng, hệ thống chuyển sang trạng thái ngừng sử dụng.";
                return RedirectToAction("Index");
            }

            db.DiaDiems.Remove(entity);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã xóa địa điểm.";
            return RedirectToAction("Index");
        }

        private void ValidateTen(string ten, int? currentId)
        {
            if (string.IsNullOrWhiteSpace(ten))
            {
                ModelState.AddModelError("TenDiaDiem", "Vui lòng nhập tên địa điểm.");
                return;
            }

            var normalized = ten.Trim();
            if (normalized.Length > 100)
            {
                ModelState.AddModelError("TenDiaDiem", "Tên địa điểm tối đa 100 ký tự.");
                return;
            }

            var normalizedLower = normalized.ToLower();
            var existed = db.DiaDiems.Any(x =>
                x.TenDiaDiem.ToLower() == normalizedLower
                && (!currentId.HasValue || x.DiaDiemId != currentId.Value));

            if (existed)
            {
                ModelState.AddModelError("TenDiaDiem", "Tên địa điểm đã tồn tại.");
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
