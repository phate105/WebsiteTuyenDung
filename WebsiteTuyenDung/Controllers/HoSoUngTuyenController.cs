using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebsiteTuyenDung.Models;
using WebsiteTuyenDung.Models.Constants;
using WebsiteTuyenDung.Models.Entities;
using WebsiteTuyenDung.Models.ViewModels;

namespace WebsiteTuyenDung.Controllers
{
    [Authorize(Roles = ApplicationRoles.NhaTuyenDung)]
    public class HoSoUngTuyenController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? tinTuyenDungId, string trangThai)
        {
            var hoSoCongTy = GetHoSoCongTyHienTai();
            if (hoSoCongTy == null)
            {
                TempData["ErrorMessage"] = "Bạn cần tạo hồ sơ công ty trước khi xử lý hồ sơ ứng tuyển.";
                return RedirectToAction("Create", "HoSoCongTy");
            }

            var query = GetDonUngTuyenQuery(hoSoCongTy.HoSoCongTyId);

            if (tinTuyenDungId.HasValue)
            {
                query = query.Where(x => x.TinTuyenDungId == tinTuyenDungId.Value);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(x => x.TrangThaiDon == trangThai);
            }

            var dsTin = db.TinTuyenDungs
                .Where(x => x.HoSoCongTyId == hoSoCongTy.HoSoCongTyId)
                .OrderByDescending(x => x.NgayCapNhat)
                .Select(x => new SelectListItem
                {
                    Value = x.TinTuyenDungId.ToString(),
                    Text = x.TieuDe,
                    Selected = tinTuyenDungId.HasValue && x.TinTuyenDungId == tinTuyenDungId.Value
                })
                .ToList();

            var model = new HoSoUngTuyenIndexViewModel
            {
                DonUngTuyens = query.OrderByDescending(x => x.NgayNop).ToList(),
                TinTuyenDungOptions = dsTin,
                TinTuyenDungId = tinTuyenDungId,
                TrangThai = trangThai
            };

            return View(model);
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var hoSoCongTy = GetHoSoCongTyHienTai();
            if (hoSoCongTy == null)
            {
                TempData["ErrorMessage"] = "Bạn cần tạo hồ sơ công ty trước khi xử lý hồ sơ ứng tuyển.";
                return RedirectToAction("Create", "HoSoCongTy");
            }

            var don = GetDonUngTuyenQuery(hoSoCongTy.HoSoCongTyId).FirstOrDefault(x => x.DonUngTuyenId == id.Value);
            if (don == null)
            {
                return HttpNotFound();
            }

            var lichSu = db.LichSuTrangThaiDons
                .Include(x => x.ApplicationUser)
                .Where(x => x.DonUngTuyenId == don.DonUngTuyenId)
                .OrderByDescending(x => x.ThoiGianThayDoi)
                .ToList();

            var model = new DonUngTuyenDetailsViewModel
            {
                DonUngTuyen = don,
                LichSuTrangThaiDons = lichSu,
                TrangThaiOptions = BuildStatusOptions(don.TrangThaiDon),
                CoTheCapNhat = don.TrangThaiDon != TrangThaiDonUngTuyen.DaRut
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhat(int id, string trangThaiDon, string ghiChuXuLy)
        {
            var hoSoCongTy = GetHoSoCongTyHienTai();
            if (hoSoCongTy == null)
            {
                TempData["ErrorMessage"] = "Bạn cần tạo hồ sơ công ty trước khi xử lý hồ sơ ứng tuyển.";
                return RedirectToAction("Create", "HoSoCongTy");
            }

            var don = GetDonUngTuyenQuery(hoSoCongTy.HoSoCongTyId).FirstOrDefault(x => x.DonUngTuyenId == id);
            if (don == null)
            {
                return HttpNotFound();
            }

            if (don.TrangThaiDon == TrangThaiDonUngTuyen.DaRut)
            {
                TempData["ErrorMessage"] = "Đơn này đã được ứng viên rút nên không thể cập nhật thêm.";
                return RedirectToAction("Details", new { id });
            }

            ghiChuXuLy = string.IsNullOrWhiteSpace(ghiChuXuLy) ? null : ghiChuXuLy.Trim();
            if (string.IsNullOrWhiteSpace(trangThaiDon) || !TrangThaiDonUngTuyen.EmployerCapNhatHopLe.Contains(trangThaiDon))
            {
                TempData["ErrorMessage"] = "Trạng thái xử lý không hợp lệ.";
                return RedirectToAction("Details", new { id });
            }

            var trangThaiCu = don.TrangThaiDon;
            don.GhiChuXuLy = ghiChuXuLy;

            if (!string.Equals(trangThaiCu, trangThaiDon, StringComparison.OrdinalIgnoreCase))
            {
                don.TrangThaiDon = trangThaiDon;
                db.LichSuTrangThaiDons.Add(new LichSuTrangThaiDon
                {
                    DonUngTuyenId = don.DonUngTuyenId,
                    ApplicationUserId = User.Identity.GetUserId(),
                    TrangThaiCu = trangThaiCu,
                    TrangThaiMoi = trangThaiDon,
                    GhiChu = ghiChuXuLy,
                    ThoiGianThayDoi = DateTime.Now
                });
            }

            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã cập nhật trạng thái hồ sơ ứng tuyển.";
            return RedirectToAction("Details", new { id });
        }

        private HoSoCongTy GetHoSoCongTyHienTai()
        {
            var userId = User.Identity.GetUserId();
            return db.HoSoCongTys.FirstOrDefault(x => x.ApplicationUserId == userId);
        }

        private IQueryable<DonUngTuyen> GetDonUngTuyenQuery(int hoSoCongTyId)
        {
            return db.DonUngTuyens
                .Include(x => x.HoSoCaNhan.ApplicationUser)
                .Include(x => x.CVUngVien)
                .Include(x => x.TinTuyenDung.HoSoCongTy)
                .Where(x => x.TinTuyenDung.HoSoCongTyId == hoSoCongTyId);
        }

        private static IEnumerable<SelectListItem> BuildStatusOptions(string selectedStatus)
        {
            var options = new[]
            {
                new SelectListItem { Value = TrangThaiDonUngTuyen.DaNop, Text = TrangThaiDonUngTuyen.ToDisplayText(TrangThaiDonUngTuyen.DaNop) },
                new SelectListItem { Value = TrangThaiDonUngTuyen.DaXem, Text = TrangThaiDonUngTuyen.ToDisplayText(TrangThaiDonUngTuyen.DaXem) },
                new SelectListItem { Value = TrangThaiDonUngTuyen.VaoDanhSachNgan, Text = TrangThaiDonUngTuyen.ToDisplayText(TrangThaiDonUngTuyen.VaoDanhSachNgan) },
                new SelectListItem { Value = TrangThaiDonUngTuyen.BiTuChoi, Text = TrangThaiDonUngTuyen.ToDisplayText(TrangThaiDonUngTuyen.BiTuChoi) },
                new SelectListItem { Value = TrangThaiDonUngTuyen.DuocChapNhan, Text = TrangThaiDonUngTuyen.ToDisplayText(TrangThaiDonUngTuyen.DuocChapNhan) }
            }.ToList();

            foreach (var option in options)
            {
                option.Selected = string.Equals(option.Value, selectedStatus, StringComparison.OrdinalIgnoreCase);
            }

            return options;
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
