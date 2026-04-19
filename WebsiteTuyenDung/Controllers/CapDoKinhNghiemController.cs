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
    public class CapDoKinhNghiemController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var data = db.CapDoKinhNghiems.OrderBy(x => x.TenCapDoKinhNghiem).ToList();
            return View(data);
        }

        public ActionResult Create()
        {
            return View(new CapDoKinhNghiem { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CapDoKinhNghiem model)
        {
            ValidateTen(model?.TenCapDoKinhNghiem, null);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.TenCapDoKinhNghiem = model.TenCapDoKinhNghiem.Trim();
            db.CapDoKinhNghiems.Add(model);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã thêm cấp độ kinh nghiệm mới.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var entity = db.CapDoKinhNghiems.Find(id.Value);
            if (entity == null)
            {
                return HttpNotFound();
            }

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CapDoKinhNghiem model)
        {
            ValidateTen(model?.TenCapDoKinhNghiem, model?.CapDoKinhNghiemId);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = db.CapDoKinhNghiems.Find(model.CapDoKinhNghiemId);
            if (entity == null)
            {
                return HttpNotFound();
            }

            entity.TenCapDoKinhNghiem = model.TenCapDoKinhNghiem.Trim();
            entity.TrangThai = model.TrangThai;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã cập nhật cấp độ kinh nghiệm.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var entity = db.CapDoKinhNghiems.Find(id);
            if (entity == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy cấp độ kinh nghiệm cần xử lý.";
                return RedirectToAction("Index");
            }

            if (db.TinTuyenDungs.Any(x => x.CapDoKinhNghiemId == id))
            {
                entity.TrangThai = false;
                db.SaveChanges();
                TempData["ErrorMessage"] = "Cấp độ kinh nghiệm này đã được dùng trong tin tuyển dụng, hệ thống chuyển sang trạng thái ngừng sử dụng.";
                return RedirectToAction("Index");
            }

            db.CapDoKinhNghiems.Remove(entity);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã xóa cấp độ kinh nghiệm.";
            return RedirectToAction("Index");
        }

        private void ValidateTen(string ten, int? currentId)
        {
            if (string.IsNullOrWhiteSpace(ten))
            {
                ModelState.AddModelError("TenCapDoKinhNghiem", "Vui lòng nhập tên cấp độ kinh nghiệm.");
                return;
            }

            var normalized = ten.Trim();
            var normalizedLower = normalized.ToLower();
            var existed = db.CapDoKinhNghiems.Any(x =>
                x.TenCapDoKinhNghiem.ToLower() == normalizedLower
                && (!currentId.HasValue || x.CapDoKinhNghiemId != currentId.Value));

            if (existed)
            {
                ModelState.AddModelError("TenCapDoKinhNghiem", "Tên cấp độ kinh nghiệm đã tồn tại.");
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
