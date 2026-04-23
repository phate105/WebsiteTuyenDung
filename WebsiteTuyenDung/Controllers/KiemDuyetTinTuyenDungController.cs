using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebsiteTuyenDung.Models;
using WebsiteTuyenDung.Models.Constants;
using WebsiteTuyenDung.Models.Entities;

namespace WebsiteTuyenDung.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class KiemDuyetTinTuyenDungController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string trangThai = TrangThaiTinTuyenDung.ChoDuyet)
        {
            if (!string.IsNullOrWhiteSpace(trangThai) && !LaTrangThaiTinHopLe(trangThai))
            {
                trangThai = TrangThaiTinTuyenDung.ChoDuyet;
            }

            var query = db.TinTuyenDungs
                .Include(x => x.HoSoCongTy)
                .Include(x => x.NganhNghe)
                .Include(x => x.DiaDiem)
                .Include(x => x.LoaiHinhLamViec)
                .Include(x => x.CapDoKinhNghiem)
                .Include(x => x.NhatKyDuyetTins)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(x => x.TrangThaiTin == trangThai);
            }

            ViewBag.TrangThai = trangThai;

            var data = query
                .OrderByDescending(x => x.NgayCapNhat)
                .ThenByDescending(x => x.NgayTao)
                .ToList();

            return View(data);
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tin = db.TinTuyenDungs
                .Include(x => x.HoSoCongTy)
                .Include(x => x.NganhNghe)
                .Include(x => x.DiaDiem)
                .Include(x => x.LoaiHinhLamViec)
                .Include(x => x.CapDoKinhNghiem)
                .Include(x => x.NhatKyDuyetTins.Select(y => y.ApplicationUser))
                .FirstOrDefault(x => x.TinTuyenDungId == id.Value);

            if (tin == null)
            {
                return HttpNotFound();
            }

            return View(tin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Duyet(int id)
        {
            var tin = db.TinTuyenDungs.Find(id);
            if (tin == null)
            {
                return HttpNotFound();
            }

            if (!string.Equals(tin.TrangThaiTin, TrangThaiTinTuyenDung.ChoDuyet, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Chỉ có thể duyệt tin đang ở trạng thái chờ duyệt.";
                return RedirectToAction("Details", new { id });
            }

            tin.TrangThaiTin = TrangThaiTinTuyenDung.DaDuyet;
            tin.NgayDang = DateTime.Now;
            tin.NgayCapNhat = DateTime.Now;
            db.NhatKyDuyetTins.Add(new NhatKyDuyetTin
            {
                TinTuyenDungId = tin.TinTuyenDungId,
                ApplicationUserId = User.Identity.GetUserId(),
                HanhDong = HanhDongDuyetTin.Duyet,
                ThoiGianXuLy = DateTime.Now
            });

            db.SaveChanges();

            TempData["Success"] = "Đã duyệt tin tuyển dụng.";
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TuChoi(int id, string lyDoTuChoi)
        {
            var tin = db.TinTuyenDungs.Find(id);
            if (tin == null)
            {
                return HttpNotFound();
            }

            if (!string.Equals(tin.TrangThaiTin, TrangThaiTinTuyenDung.ChoDuyet, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Chỉ có thể từ chối tin đang ở trạng thái chờ duyệt.";
                return RedirectToAction("Details", new { id });
            }

            lyDoTuChoi = lyDoTuChoi?.Trim();
            if (string.IsNullOrWhiteSpace(lyDoTuChoi))
            {
                TempData["Error"] = "Vui lòng nhập lý do từ chối.";
                return RedirectToAction("Details", new { id });
            }

            if (lyDoTuChoi.Length > 1000)
            {
                TempData["Error"] = "Lý do từ chối tối đa 1000 ký tự.";
                return RedirectToAction("Details", new { id });
            }

            tin.TrangThaiTin = TrangThaiTinTuyenDung.BiTuChoi;
            tin.NgayCapNhat = DateTime.Now;
            db.NhatKyDuyetTins.Add(new NhatKyDuyetTin
            {
                TinTuyenDungId = tin.TinTuyenDungId,
                ApplicationUserId = User.Identity.GetUserId(),
                HanhDong = HanhDongDuyetTin.TuChoi,
                LyDoTuChoi = lyDoTuChoi,
                ThoiGianXuLy = DateTime.Now
            });

            db.SaveChanges();

            TempData["Success"] = "Đã từ chối tin tuyển dụng.";
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DongTin(int id)
        {
            var tin = db.TinTuyenDungs.Find(id);
            if (tin == null)
            {
                return HttpNotFound();
            }

            if (!TrangThaiTinTuyenDung.CoTheDong(tin.TrangThaiTin))
            {
                TempData["Error"] = "Chỉ có thể đóng tin đã duyệt.";
                return RedirectToAction("Details", new { id });
            }

            tin.TrangThaiTin = TrangThaiTinTuyenDung.DaDong;
            tin.NgayCapNhat = DateTime.Now;
            db.NhatKyDuyetTins.Add(new NhatKyDuyetTin
            {
                TinTuyenDungId = tin.TinTuyenDungId,
                ApplicationUserId = User.Identity.GetUserId(),
                HanhDong = HanhDongDuyetTin.DongTin,
                ThoiGianXuLy = DateTime.Now
            });

            db.SaveChanges();

            TempData["Success"] = "Đã đóng tin tuyển dụng.";
            return RedirectToAction("Details", new { id });
        }

        private static bool LaTrangThaiTinHopLe(string trangThai)
        {
            return string.Equals(trangThai, TrangThaiTinTuyenDung.Nhap, StringComparison.OrdinalIgnoreCase)
                || string.Equals(trangThai, TrangThaiTinTuyenDung.ChoDuyet, StringComparison.OrdinalIgnoreCase)
                || string.Equals(trangThai, TrangThaiTinTuyenDung.DaDuyet, StringComparison.OrdinalIgnoreCase)
                || string.Equals(trangThai, TrangThaiTinTuyenDung.BiTuChoi, StringComparison.OrdinalIgnoreCase)
                || string.Equals(trangThai, TrangThaiTinTuyenDung.DaDong, StringComparison.OrdinalIgnoreCase)
                || string.Equals(trangThai, TrangThaiTinTuyenDung.HetHan, StringComparison.OrdinalIgnoreCase);
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
