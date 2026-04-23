using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebsiteTuyenDung.Models;
using WebsiteTuyenDung.Models.Constants;
using WebsiteTuyenDung.Models.Entities;
using WebsiteTuyenDung.Models.ViewModels;

namespace WebsiteTuyenDung.Controllers
{
    public class JobController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string keyword = null, int? nganhNgheId = null, int? diaDiemId = null, int? loaiHinhLamViecId = null, int? capDoKinhNghiemId = null, int page = 1)
        {
            const int pageSize = 20;
            var query = GetCongViecDangMoQuery();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                if (keyword.Length > 100)
                {
                    keyword = keyword.Substring(0, 100);
                }

                query = query.Where(x =>
                    x.TieuDe.Contains(keyword) ||
                    x.MoTaCongViec.Contains(keyword) ||
                    x.HoSoCongTy.TenCongTy.Contains(keyword) ||
                    x.NganhNghe.TenNganhNghe.Contains(keyword));
            }

            if (nganhNgheId.HasValue)
            {
                query = query.Where(x => x.NganhNgheId == nganhNgheId.Value);
            }

            if (diaDiemId.HasValue)
            {
                query = query.Where(x => x.DiaDiemId == diaDiemId.Value);
            }

            if (loaiHinhLamViecId.HasValue)
            {
                query = query.Where(x => x.LoaiHinhLamViecId == loaiHinhLamViecId.Value);
            }

            if (capDoKinhNghiemId.HasValue)
            {
                query = query.Where(x => x.CapDoKinhNghiemId == capDoKinhNghiemId.Value);
            }

            if (page < 1)
            {
                page = 1;
            }

            var totalJobs = query.Count();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalJobs / (double)pageSize));
            if (page > totalPages)
            {
                page = totalPages;
            }

            var jobs = query
                .OrderByDescending(x => x.NgayDang ?? x.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(MapToJobCard)
                .ToList();

            var model = new JobIndexViewModel
            {
                Keyword = keyword,
                NganhNgheId = nganhNgheId,
                DiaDiemId = diaDiemId,
                LoaiHinhLamViecId = loaiHinhLamViecId,
                CapDoKinhNghiemId = capDoKinhNghiemId,
                NganhNgheOptions = new SelectList(db.NganhNghes.Where(x => x.TrangThai).OrderBy(x => x.TenNganhNghe), "NganhNgheId", "TenNganhNghe", nganhNgheId),
                DiaDiemOptions = new SelectList(db.DiaDiems.Where(x => x.TrangThai).OrderBy(x => x.TenDiaDiem), "DiaDiemId", "TenDiaDiem", diaDiemId),
                LoaiHinhLamViecOptions = new SelectList(db.LoaiHinhLamViecs.Where(x => x.TrangThai).OrderBy(x => x.TenLoaiHinhLamViec), "LoaiHinhLamViecId", "TenLoaiHinhLamViec", loaiHinhLamViecId),
                CapDoKinhNghiemOptions = new SelectList(db.CapDoKinhNghiems.Where(x => x.TrangThai).OrderBy(x => x.TenCapDoKinhNghiem), "CapDoKinhNghiemId", "TenCapDoKinhNghiem", capDoKinhNghiemId),
                Jobs = jobs,
                TongKetQua = totalJobs,
                TrangHienTai = page,
                TongTrang = totalPages,
                KichThuocTrang = pageSize
            };

            return View(model);
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var tinTuyenDung = GetCongViecCongKhaiChiTietQuery().FirstOrDefault(x => x.TinTuyenDungId == id.Value);
            if (tinTuyenDung == null)
            {
                return HttpNotFound();
            }

            var model = BuildJobDetailsViewModel(tinTuyenDung);
            return View(model);
        }

        public ActionResult Company(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var company = db.HoSoCongTys.FirstOrDefault(x => x.HoSoCongTyId == id.Value);
            if (company == null)
            {
                return HttpNotFound();
            }

            var jobsQuery = GetCongViecDangMoQuery()
                .Where(x => x.HoSoCongTyId == company.HoSoCongTyId);
            var model = new CompanyDetailsViewModel
            {
                CongTy = company,
                SoViecDangTuyen = jobsQuery.Count(),
                Jobs = jobsQuery
                    .OrderByDescending(x => x.NgayDang ?? x.NgayTao)
                    .Take(12)
                    .ToList()
                    .Select(MapToJobCard)
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.UngVien)]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(ApplyForJobViewModel model)
        {
            model = model ?? new ApplyForJobViewModel();
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? "Vui lòng kiểm tra lại thông tin ứng tuyển.";
                return RedirectToAction("Details", new { id = model.TinTuyenDungId });
            }

            var tinTuyenDung = GetCongViecDangMoQuery().FirstOrDefault(x => x.TinTuyenDungId == model.TinTuyenDungId);
            if (tinTuyenDung == null)
            {
                TempData["ErrorMessage"] = "Tin tuyển dụng không tồn tại hoặc đã ngừng nhận hồ sơ.";
                return RedirectToAction("Index");
            }

            var userId = User.Identity.GetUserId();
            var hoSoCaNhan = db.HoSoCaNhans
                .Include(x => x.CVUngViens)
                .FirstOrDefault(x => x.ApplicationUserId == userId);

            if (hoSoCaNhan == null)
            {
                TempData["ErrorMessage"] = "Bạn cần cập nhật hồ sơ cá nhân trước khi ứng tuyển.";
                return RedirectToAction("HoSo", "UngVien");
            }

            if (hoSoCaNhan.CVUngViens == null || !hoSoCaNhan.CVUngViens.Any())
            {
                TempData["ErrorMessage"] = "Bạn cần tải lên ít nhất một CV trước khi ứng tuyển.";
                return RedirectToAction("CV", "UngVien");
            }

            if (db.DonUngTuyens.Any(x => x.HoSoCaNhanId == hoSoCaNhan.HoSoCaNhanId && x.TinTuyenDungId == tinTuyenDung.TinTuyenDungId))
            {
                TempData["ErrorMessage"] = "Bạn đã nộp hồ sơ vào tin tuyển dụng này rồi.";
                return RedirectToAction("Details", new { id = tinTuyenDung.TinTuyenDungId });
            }

            var cvUngVien = hoSoCaNhan.CVUngViens.FirstOrDefault(x => x.CVUngVienId == model.CVUngVienId.Value);
            if (cvUngVien == null)
            {
                TempData["ErrorMessage"] = "CV được chọn không thuộc về tài khoản hiện tại.";
                return RedirectToAction("Details", new { id = tinTuyenDung.TinTuyenDungId });
            }

            var donUngTuyen = new DonUngTuyen
            {
                HoSoCaNhanId = hoSoCaNhan.HoSoCaNhanId,
                TinTuyenDungId = tinTuyenDung.TinTuyenDungId,
                CVUngVienId = cvUngVien.CVUngVienId,
                NgayNop = DateTime.Now,
                TrangThaiDon = TrangThaiDonUngTuyen.DaNop,
                ThuGioiThieu = NullIfWhiteSpace(model.ThuGioiThieu)
            };

            donUngTuyen.LichSuTrangThaiDons.Add(new LichSuTrangThaiDon
            {
                ApplicationUserId = userId,
                TrangThaiMoi = TrangThaiDonUngTuyen.DaNop,
                GhiChu = "Ứng viên nộp hồ sơ.",
                ThoiGianThayDoi = DateTime.Now
            });

            db.DonUngTuyens.Add(donUngTuyen);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã nộp hồ sơ ứng tuyển thành công.";
            return RedirectToAction("ChiTietDon", "UngVien", new { id = donUngTuyen.DonUngTuyenId });
        }

        public ActionResult Search(string keyword = "", string location = "")
        {
            return RedirectToAction("Index", new { keyword = keyword });
        }

        private JobDetailsViewModel BuildJobDetailsViewModel(TinTuyenDung tinTuyenDung)
        {
            var model = new JobDetailsViewModel
            {
                TinTuyenDung = tinTuyenDung,
                ApplyForm = new ApplyForJobViewModel
                {
                    TinTuyenDungId = tinTuyenDung.TinTuyenDungId
                },
                RelatedJobs = GetCongViecDangMoQuery()
                    .Where(x => x.TinTuyenDungId != tinTuyenDung.TinTuyenDungId && x.NganhNgheId == tinTuyenDung.NganhNgheId)
                    .OrderByDescending(x => x.NgayDang ?? x.NgayTao)
                    .Take(3)
                    .ToList()
                    .Select(MapToJobCard)
                    .ToList(),
                IsAuthenticated = Request.IsAuthenticated,
                IsCandidate = Request.IsAuthenticated && User.IsInRole(ApplicationRoles.UngVien),
                CoTheUngTuyen = CoTheUngTuyen(tinTuyenDung),
                CvOptions = Enumerable.Empty<SelectListItem>()
            };

            if (!model.IsAuthenticated)
            {
                model.ApplyMessage = "Bạn cần đăng nhập tài khoản ứng viên để nộp hồ sơ.";
                return model;
            }

            if (!model.IsCandidate)
            {
                model.ApplyMessage = "Chỉ tài khoản ứng viên mới có thể nộp hồ sơ vào tin tuyển dụng.";
                return model;
            }

            var userId = User.Identity.GetUserId();
            var hoSoCaNhan = db.HoSoCaNhans
                .Include(x => x.CVUngViens)
                .FirstOrDefault(x => x.ApplicationUserId == userId);

            model.HasProfile = hoSoCaNhan != null;
            model.HasCv = hoSoCaNhan != null && hoSoCaNhan.CVUngViens.Any();
            model.AlreadyApplied = hoSoCaNhan != null &&
                                  db.DonUngTuyens.Any(x => x.HoSoCaNhanId == hoSoCaNhan.HoSoCaNhanId && x.TinTuyenDungId == tinTuyenDung.TinTuyenDungId);

            if (!model.CoTheUngTuyen)
            {
                model.ApplyMessage = "Tin tuyển dụng này hiện không còn nhận hồ sơ.";
            }
            else if (!model.HasProfile)
            {
                model.ApplyMessage = "Bạn cần cập nhật hồ sơ cá nhân trước khi ứng tuyển.";
            }
            else if (!model.HasCv)
            {
                model.ApplyMessage = "Bạn cần tải lên ít nhất một CV trước khi ứng tuyển.";
            }
            else if (model.AlreadyApplied)
            {
                model.ApplyMessage = "Bạn đã nộp hồ sơ vào công việc này.";
            }

            if (hoSoCaNhan != null)
            {
                model.CvOptions = hoSoCaNhan.CVUngViens
                    .OrderByDescending(x => x.NgayTaiLen)
                    .Select(x => new SelectListItem
                    {
                        Value = x.CVUngVienId.ToString(),
                        Text = x.TenCV
                    })
                    .ToList();
            }

            return model;
        }

        private IQueryable<TinTuyenDung> GetCongViecDangMoQuery()
        {
            var homNay = DateTime.Today;

            return db.TinTuyenDungs
                .Include(x => x.HoSoCongTy)
                .Include(x => x.NganhNghe)
                .Include(x => x.DiaDiem)
                .Include(x => x.LoaiHinhLamViec)
                .Include(x => x.CapDoKinhNghiem)
                .Where(x => x.TrangThaiTin == TrangThaiTinTuyenDung.DaDuyet && x.HanNopHoSo >= homNay);
        }

        private IQueryable<TinTuyenDung> GetCongViecCongKhaiChiTietQuery()
        {
            return db.TinTuyenDungs
                .Include(x => x.HoSoCongTy)
                .Include(x => x.NganhNghe)
                .Include(x => x.DiaDiem)
                .Include(x => x.LoaiHinhLamViec)
                .Include(x => x.CapDoKinhNghiem)
                .Where(x =>
                    x.TrangThaiTin == TrangThaiTinTuyenDung.DaDuyet ||
                    x.TrangThaiTin == TrangThaiTinTuyenDung.DaDong ||
                    x.TrangThaiTin == TrangThaiTinTuyenDung.HetHan);
        }

        private static PublicJobCardViewModel MapToJobCard(TinTuyenDung tinTuyenDung)
        {
            return new PublicJobCardViewModel
            {
                TinTuyenDungId = tinTuyenDung.TinTuyenDungId,
                HoSoCongTyId = tinTuyenDung.HoSoCongTyId,
                TieuDe = tinTuyenDung.TieuDe,
                TenCongTy = tinTuyenDung.HoSoCongTy != null ? tinTuyenDung.HoSoCongTy.TenCongTy : "Doanh nghiệp",
                LogoCongTy = NormalizeUrl(tinTuyenDung.HoSoCongTy != null ? tinTuyenDung.HoSoCongTy.Logo : null),
                TenDiaDiem = tinTuyenDung.DiaDiem != null ? tinTuyenDung.DiaDiem.TenDiaDiem : "Chưa cập nhật",
                TenLoaiHinhLamViec = tinTuyenDung.LoaiHinhLamViec != null ? tinTuyenDung.LoaiHinhLamViec.TenLoaiHinhLamViec : "Chưa cập nhật",
                TenNganhNghe = tinTuyenDung.NganhNghe != null ? tinTuyenDung.NganhNghe.TenNganhNghe : "Chưa cập nhật",
                TenCapDoKinhNghiem = tinTuyenDung.CapDoKinhNghiem != null ? tinTuyenDung.CapDoKinhNghiem.TenCapDoKinhNghiem : "Chưa cập nhật",
                MoTaNgan = BuildSummaryText(tinTuyenDung.MoTaCongViec),
                LuongHienThi = BuildSalaryText(tinTuyenDung.LuongToiThieu, tinTuyenDung.LuongToiDa),
                NgayDang = tinTuyenDung.NgayDang,
                HanNopHoSo = tinTuyenDung.HanNopHoSo,
                CoTheUngTuyen = CoTheUngTuyen(tinTuyenDung)
            };
        }

        private static bool CoTheUngTuyen(TinTuyenDung tinTuyenDung)
        {
            return tinTuyenDung != null &&
                   tinTuyenDung.TrangThaiTin == TrangThaiTinTuyenDung.DaDuyet &&
                   tinTuyenDung.HanNopHoSo.Date >= DateTime.Today;
        }

        private static string BuildSalaryText(decimal? luongToiThieu, decimal? luongToiDa)
        {
            if (!luongToiThieu.HasValue && !luongToiDa.HasValue)
            {
                return "Thỏa thuận";
            }

            if (luongToiThieu.HasValue && luongToiDa.HasValue)
            {
                return string.Format("{0:#,##0} - {1:#,##0} VNĐ", luongToiThieu.Value, luongToiDa.Value);
            }

            if (luongToiThieu.HasValue)
            {
                return string.Format("Từ {0:#,##0} VNĐ", luongToiThieu.Value);
            }

            return string.Format("Đến {0:#,##0} VNĐ", luongToiDa.Value);
        }

        private static string BuildSummaryText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Thông tin công việc sẽ được nhà tuyển dụng trao đổi thêm khi phỏng vấn.";
            }

            var text = value.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim();
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            return text.Length <= 230 ? text : text.Substring(0, 230).TrimEnd() + "...";
        }

        private static string NormalizeUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (value.StartsWith("http", StringComparison.OrdinalIgnoreCase) || value.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }

            return "/" + value.TrimStart('/');
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
