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
    [Authorize(Roles = ApplicationRoles.NhaTuyenDung)]
    public class TinTuyenDungController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var hoSoCongTy = GetHoSoCongTyHienTai();
            if (hoSoCongTy == null)
            {
                TempData["ErrorMessage"] = "Bạn cần tạo hồ sơ công ty trước khi quản lý tin tuyển dụng.";
                return RedirectToAction("Create", "HoSoCongTy");
            }

            var data = db.TinTuyenDungs
                .Include(x => x.HoSoCongTy)
                .Include(x => x.NganhNghe)
                .Include(x => x.DiaDiem)
                .Include(x => x.LoaiHinhLamViec)
                .Include(x => x.CapDoKinhNghiem)
                .Include(x => x.DonUngTuyens)
                .Where(x => x.HoSoCongTyId == hoSoCongTy.HoSoCongTyId)
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

            var tin = GetTinThuocEmployer(id.Value, true);
            if (tin == null)
            {
                return HttpNotFound();
            }

            return View(tin);
        }

        public ActionResult Create()
        {
            var hoSoCongTy = GetHoSoCongTyHienTai();
            if (hoSoCongTy == null)
            {
                TempData["ErrorMessage"] = "Bạn cần tạo hồ sơ công ty trước khi đăng tin tuyển dụng.";
                return RedirectToAction("Create", "HoSoCongTy");
            }

            var model = new TinTuyenDung
            {
                TrangThaiTin = TrangThaiTinTuyenDung.Nhap,
                HanNopHoSo = DateTime.Today.AddDays(30),
                SoLuongTuyen = 1
            };

            PopulateDanhMucSelections(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TinTuyenDung model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var hoSoCongTy = GetHoSoCongTyHienTai();
            if (hoSoCongTy == null)
            {
                TempData["ErrorMessage"] = "Bạn cần tạo hồ sơ công ty trước khi đăng tin tuyển dụng.";
                return RedirectToAction("Create", "HoSoCongTy");
            }

            ValidateTinTuyenDung(model);
            if (!ModelState.IsValid)
            {
                PopulateDanhMucSelections(model);
                return View(model);
            }

            model.HoSoCongTyId = hoSoCongTy.HoSoCongTyId;
            model.TrangThaiTin = TrangThaiTinTuyenDung.Nhap;
            model.NgayDang = null;
            model.NgayTao = DateTime.Now;
            model.NgayCapNhat = DateTime.Now;
            db.TinTuyenDungs.Add(model);
            db.SaveChanges();

            TempData["Success"] = "Đã tạo tin tuyển dụng ở trạng thái nháp.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tin = GetTinThuocEmployer(id.Value, false);
            if (tin == null)
            {
                return HttpNotFound();
            }

            if (!TrangThaiTinTuyenDung.CoTheSuaBoiEmployer(tin.TrangThaiTin))
            {
                TempData["Error"] = "Chỉ có thể sửa tin đang ở trạng thái nháp hoặc bị từ chối.";
                return RedirectToAction("Details", new { id = tin.TinTuyenDungId });
            }

            PopulateDanhMucSelections(tin);
            return View(tin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TinTuyenDung model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tin = GetTinThuocEmployer(model.TinTuyenDungId, false);
            if (tin == null)
            {
                return HttpNotFound();
            }

            if (!TrangThaiTinTuyenDung.CoTheSuaBoiEmployer(tin.TrangThaiTin))
            {
                TempData["Error"] = "Không thể chỉnh sửa tin ở trạng thái hiện tại.";
                return RedirectToAction("Details", new { id = model.TinTuyenDungId });
            }

            ValidateTinTuyenDung(model);
            if (!ModelState.IsValid)
            {
                model.TrangThaiTin = tin.TrangThaiTin;
                PopulateDanhMucSelections(model);
                return View(model);
            }

            tin.TieuDe = model.TieuDe?.Trim();
            tin.NganhNgheId = model.NganhNgheId;
            tin.DiaDiemId = model.DiaDiemId;
            tin.LoaiHinhLamViecId = model.LoaiHinhLamViecId;
            tin.CapDoKinhNghiemId = model.CapDoKinhNghiemId;
            tin.MoTaCongViec = model.MoTaCongViec?.Trim();
            tin.YeuCau = NullIfWhiteSpace(model.YeuCau);
            tin.QuyenLoi = NullIfWhiteSpace(model.QuyenLoi);
            tin.SoLuongTuyen = model.SoLuongTuyen;
            tin.LuongToiThieu = model.LuongToiThieu;
            tin.LuongToiDa = model.LuongToiDa;
            tin.HanNopHoSo = model.HanNopHoSo;
            tin.NgayCapNhat = DateTime.Now;

            db.SaveChanges();

            TempData["Success"] = "Đã cập nhật tin tuyển dụng.";
            return RedirectToAction("Details", new { id = tin.TinTuyenDungId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuiDuyet(int id)
        {
            var tin = GetTinThuocEmployer(id, false);
            if (tin == null)
            {
                return HttpNotFound();
            }

            if (!TrangThaiTinTuyenDung.CoTheGuiDuyet(tin.TrangThaiTin))
            {
                TempData["Error"] = "Chỉ có thể gửi duyệt tin đang ở trạng thái nháp hoặc bị từ chối.";
                return RedirectToAction("Details", new { id });
            }

            tin.TrangThaiTin = TrangThaiTinTuyenDung.ChoDuyet;
            tin.NgayCapNhat = DateTime.Now;
            db.NhatKyDuyetTins.Add(new NhatKyDuyetTin
            {
                TinTuyenDungId = tin.TinTuyenDungId,
                ApplicationUserId = User.Identity.GetUserId(),
                HanhDong = HanhDongDuyetTin.GuiDuyet,
                ThoiGianXuLy = DateTime.Now
            });

            db.SaveChanges();

            TempData["Success"] = "Đã gửi tin sang trạng thái chờ duyệt.";
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DongTin(int id)
        {
            var tin = GetTinThuocEmployer(id, false);
            if (tin == null)
            {
                return HttpNotFound();
            }

            if (!TrangThaiTinTuyenDung.CoTheDong(tin.TrangThaiTin))
            {
                TempData["Error"] = "Chỉ có thể đóng tin đã được duyệt.";
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

        private HoSoCongTy GetHoSoCongTyHienTai()
        {
            var userId = User.Identity.GetUserId();
            return db.HoSoCongTys.FirstOrDefault(x => x.ApplicationUserId == userId);
        }

        private TinTuyenDung GetTinThuocEmployer(int id, bool includeDetails)
        {
            var hoSoCongTy = GetHoSoCongTyHienTai();
            if (hoSoCongTy == null)
            {
                return null;
            }

            IQueryable<TinTuyenDung> query = db.TinTuyenDungs.Where(x => x.HoSoCongTyId == hoSoCongTy.HoSoCongTyId);
            if (includeDetails)
            {
                query = query
                    .Include(x => x.HoSoCongTy)
                    .Include(x => x.NganhNghe)
                    .Include(x => x.DiaDiem)
                    .Include(x => x.LoaiHinhLamViec)
                    .Include(x => x.CapDoKinhNghiem)
                    .Include(x => x.DonUngTuyens)
                    .Include(x => x.NhatKyDuyetTins.Select(y => y.ApplicationUser));
            }

            return query.FirstOrDefault(x => x.TinTuyenDungId == id);
        }

        private void PopulateDanhMucSelections(TinTuyenDung model)
        {
            ViewBag.NganhNgheId = new SelectList(
                db.NganhNghes.Where(x => x.TrangThai).OrderBy(x => x.TenNganhNghe).ToList(),
                "NganhNgheId",
                "TenNganhNghe",
                model?.NganhNgheId);

            ViewBag.DiaDiemId = new SelectList(
                db.DiaDiems.Where(x => x.TrangThai).OrderBy(x => x.TenDiaDiem).ToList(),
                "DiaDiemId",
                "TenDiaDiem",
                model?.DiaDiemId);

            ViewBag.LoaiHinhLamViecId = new SelectList(
                db.LoaiHinhLamViecs.Where(x => x.TrangThai).OrderBy(x => x.TenLoaiHinhLamViec).ToList(),
                "LoaiHinhLamViecId",
                "TenLoaiHinhLamViec",
                model?.LoaiHinhLamViecId);

            ViewBag.CapDoKinhNghiemId = new SelectList(
                db.CapDoKinhNghiems.Where(x => x.TrangThai).OrderBy(x => x.TenCapDoKinhNghiem).ToList(),
                "CapDoKinhNghiemId",
                "TenCapDoKinhNghiem",
                model?.CapDoKinhNghiemId);
        }

        private void ValidateTinTuyenDung(TinTuyenDung model)
        {
            model.TieuDe = model.TieuDe?.Trim();
            model.MoTaCongViec = model.MoTaCongViec?.Trim();
            model.YeuCau = model.YeuCau?.Trim();
            model.QuyenLoi = model.QuyenLoi?.Trim();

            if (string.IsNullOrWhiteSpace(model.TieuDe))
            {
                ModelState.AddModelError(nameof(model.TieuDe), "Vui lòng nhập tiêu đề tin tuyển dụng.");
            }
            else if (model.TieuDe.Length > 200)
            {
                ModelState.AddModelError(nameof(model.TieuDe), "Tiêu đề tin tuyển dụng tối đa 200 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(model.MoTaCongViec))
            {
                ModelState.AddModelError(nameof(model.MoTaCongViec), "Vui lòng nhập mô tả công việc.");
            }
            else if (model.MoTaCongViec.Length > 8000)
            {
                ModelState.AddModelError(nameof(model.MoTaCongViec), "Mô tả công việc tối đa 8000 ký tự.");
            }

            if (!string.IsNullOrWhiteSpace(model.YeuCau) && model.YeuCau.Length > 5000)
            {
                ModelState.AddModelError(nameof(model.YeuCau), "Yêu cầu ứng viên tối đa 5000 ký tự.");
            }

            if (!string.IsNullOrWhiteSpace(model.QuyenLoi) && model.QuyenLoi.Length > 5000)
            {
                ModelState.AddModelError(nameof(model.QuyenLoi), "Quyền lợi tối đa 5000 ký tự.");
            }

            if (model.SoLuongTuyen <= 0)
            {
                ModelState.AddModelError(nameof(model.SoLuongTuyen), "Số lượng tuyển phải lớn hơn 0.");
            }
            else if (model.SoLuongTuyen > 1000)
            {
                ModelState.AddModelError(nameof(model.SoLuongTuyen), "Số lượng tuyển không nên vượt quá 1000 người.");
            }

            if (model.HanNopHoSo.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(model.HanNopHoSo), "Hạn nộp hồ sơ không được ở trong quá khứ.");
            }
            else if (model.HanNopHoSo.Date > DateTime.Today.AddYears(2))
            {
                ModelState.AddModelError(nameof(model.HanNopHoSo), "Hạn nộp hồ sơ không nên vượt quá 2 năm.");
            }

            if (model.LuongToiThieu.HasValue && model.LuongToiThieu.Value < 0)
            {
                ModelState.AddModelError(nameof(model.LuongToiThieu), "Lương tối thiểu không được âm.");
            }
            else if (model.LuongToiThieu.HasValue && model.LuongToiThieu.Value > 1000000000m)
            {
                ModelState.AddModelError(nameof(model.LuongToiThieu), "Lương tối thiểu không nên vượt quá 1 tỷ VNĐ.");
            }

            if (model.LuongToiDa.HasValue && model.LuongToiDa.Value < 0)
            {
                ModelState.AddModelError(nameof(model.LuongToiDa), "Lương tối đa không được âm.");
            }
            else if (model.LuongToiDa.HasValue && model.LuongToiDa.Value > 1000000000m)
            {
                ModelState.AddModelError(nameof(model.LuongToiDa), "Lương tối đa không nên vượt quá 1 tỷ VNĐ.");
            }

            if (model.LuongToiThieu.HasValue && model.LuongToiDa.HasValue && model.LuongToiThieu.Value > model.LuongToiDa.Value)
            {
                ModelState.AddModelError(nameof(model.LuongToiDa), "Lương tối đa phải lớn hơn hoặc bằng lương tối thiểu.");
            }

            if (!db.NganhNghes.Any(x => x.NganhNgheId == model.NganhNgheId && x.TrangThai))
            {
                ModelState.AddModelError(nameof(model.NganhNgheId), "Vui lòng chọn ngành nghề hợp lệ.");
            }

            if (!db.DiaDiems.Any(x => x.DiaDiemId == model.DiaDiemId && x.TrangThai))
            {
                ModelState.AddModelError(nameof(model.DiaDiemId), "Vui lòng chọn địa điểm hợp lệ.");
            }

            if (!db.LoaiHinhLamViecs.Any(x => x.LoaiHinhLamViecId == model.LoaiHinhLamViecId && x.TrangThai))
            {
                ModelState.AddModelError(nameof(model.LoaiHinhLamViecId), "Vui lòng chọn loại hình làm việc hợp lệ.");
            }

            if (!db.CapDoKinhNghiems.Any(x => x.CapDoKinhNghiemId == model.CapDoKinhNghiemId && x.TrangThai))
            {
                ModelState.AddModelError(nameof(model.CapDoKinhNghiemId), "Vui lòng chọn cấp độ kinh nghiệm hợp lệ.");
            }
        }

        private static string NullIfWhiteSpace(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
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
