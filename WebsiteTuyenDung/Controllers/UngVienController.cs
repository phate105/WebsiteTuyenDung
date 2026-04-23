using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebsiteTuyenDung.Models;
using WebsiteTuyenDung.Models.Constants;
using WebsiteTuyenDung.Models.Entities;
using WebsiteTuyenDung.Models.ViewModels;

namespace WebsiteTuyenDung.Controllers
{
    [Authorize(Roles = ApplicationRoles.UngVien)]
    public class UngVienController : Controller
    {
        private static readonly string[] AllowedCvExtensions = { ".pdf", ".doc", ".docx" };
        private static readonly string[] AllowedCvContentTypes =
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/octet-stream"
        };
        private const int CvMaxSizeInBytes = 5 * 1024 * 1024;

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult HoSo()
        {
            var hoSoCaNhan = GetHoSoCaNhanHienTai();
            var model = hoSoCaNhan == null ? new UngVienHoSoViewModel() : MapToUngVienHoSo(hoSoCaNhan);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HoSo(UngVienHoSoViewModel model)
        {
            model = model ?? new UngVienHoSoViewModel();
            TrimHoSoModel(model);
            if (model.NgaySinh.HasValue && model.NgaySinh.Value.Date > DateTime.Today)
            {
                ModelState.AddModelError(nameof(model.NgaySinh), "Ngày sinh không được ở tương lai.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.Identity.GetUserId();
            var entity = GetHoSoCaNhanHienTai();
            if (entity == null)
            {
                entity = new HoSoCaNhan
                {
                    ApplicationUserId = userId
                };
                db.HoSoCaNhans.Add(entity);
            }

            entity.HoTen = model.HoTen;
            entity.NgaySinh = model.NgaySinh;
            entity.GioiTinh = NullIfWhiteSpace(model.GioiTinh);
            entity.SoDienThoai = NullIfWhiteSpace(model.SoDienThoai);
            entity.DiaChi = NullIfWhiteSpace(model.DiaChi);
            entity.MucTieuNgheNghiep = NullIfWhiteSpace(model.MucTieuNgheNghiep);
            entity.HocVan = NullIfWhiteSpace(model.HocVan);
            entity.TomTatKinhNghiem = NullIfWhiteSpace(model.TomTatKinhNghiem);
            entity.NgayCapNhat = DateTime.Now;

            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã cập nhật hồ sơ cá nhân.";
            return RedirectToAction("HoSo");
        }

        public ActionResult CV()
        {
            var model = BuildCvPageViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaiLenCV(UploadCvViewModel model, HttpPostedFileBase cvFile)
        {
            model = model ?? new UploadCvViewModel();
            var userId = User.Identity.GetUserId();
            var hoSoCaNhan = db.HoSoCaNhans
                .Include(x => x.CVUngViens)
                .FirstOrDefault(x => x.ApplicationUserId == userId);

            if (hoSoCaNhan == null)
            {
                TempData["ErrorMessage"] = "Bạn cần cập nhật hồ sơ cá nhân trước khi tải lên CV.";
                return RedirectToAction("HoSo");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? "Vui lòng kiểm tra lại thông tin CV.";
                return RedirectToAction("CV");
            }

            var validationError = ValidateCvFile(cvFile);
            if (!string.IsNullOrWhiteSpace(validationError))
            {
                TempData["ErrorMessage"] = validationError;
                return RedirectToAction("CV");
            }

            var tenCv = string.IsNullOrWhiteSpace(model.TenCV)
                ? Path.GetFileNameWithoutExtension(cvFile.FileName)
                : model.TenCV.Trim();

            if (tenCv.Length > 200)
            {
                tenCv = tenCv.Substring(0, 200);
            }

            var duongDanFile = SaveCvFile(cvFile);
            var entity = new CVUngVien
            {
                HoSoCaNhanId = hoSoCaNhan.HoSoCaNhanId,
                TenCV = tenCv,
                DuongDanFile = duongDanFile,
                NgayTaiLen = DateTime.Now,
                TrangThaiSuDung = !hoSoCaNhan.CVUngViens.Any()
            };

            db.CVUngViens.Add(entity);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã tải lên CV mới.";
            return Redirect(Url.Action("CV") + "#cv-list");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoCV(TaoCvViewModel model)
        {
            model = model ?? new TaoCvViewModel();
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? "Vui lòng kiểm tra lại nội dung tạo CV.";
                return RedirectToAction("CV");
            }

            var userId = User.Identity.GetUserId();
            var hoSoCaNhan = db.HoSoCaNhans
                .Include(x => x.ApplicationUser)
                .Include(x => x.CVUngViens)
                .FirstOrDefault(x => x.ApplicationUserId == userId);

            if (hoSoCaNhan == null)
            {
                TempData["ErrorMessage"] = "Bạn cần cập nhật hồ sơ cá nhân trước khi tạo CV.";
                return RedirectToAction("HoSo");
            }

            var tenCv = string.IsNullOrWhiteSpace(model.TenCV)
                ? "CV " + hoSoCaNhan.HoTen
                : model.TenCV.Trim();

            if (tenCv.Length > 200)
            {
                tenCv = tenCv.Substring(0, 200);
            }

            var duongDanFile = SaveGeneratedCvFile(hoSoCaNhan, model, tenCv);
            var entity = new CVUngVien
            {
                HoSoCaNhanId = hoSoCaNhan.HoSoCaNhanId,
                TenCV = tenCv,
                DuongDanFile = duongDanFile,
                NgayTaiLen = DateTime.Now,
                TrangThaiSuDung = !hoSoCaNhan.CVUngViens.Any()
            };

            db.CVUngViens.Add(entity);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã tạo CV mới. Bạn có thể mở hoặc tải xuống trong danh sách CV.";
            return Redirect(Url.Action("CV") + "#cv-list");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatTenCV(int id, string tenCV)
        {
            var cvUngVien = GetCvHienTai(id);
            if (cvUngVien == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            tenCV = (tenCV ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(tenCV))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập tên CV.";
                return RedirectToAction("CV");
            }

            if (tenCV.Length > 200)
            {
                tenCV = tenCV.Substring(0, 200);
            }

            cvUngVien.TenCV = tenCV;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã cập nhật tên CV.";
            return RedirectToAction("CV");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaCV(int id)
        {
            var userId = User.Identity.GetUserId();
            var cvUngVien = db.CVUngViens
                .Include(x => x.DonUngTuyens)
                .Include(x => x.HoSoCaNhan)
                .FirstOrDefault(x => x.CVUngVienId == id && x.HoSoCaNhan.ApplicationUserId == userId);

            if (cvUngVien == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (cvUngVien.DonUngTuyens != null && cvUngVien.DonUngTuyens.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa CV đã được dùng để nộp hồ sơ.";
                return RedirectToAction("CV");
            }

            DeleteLocalCvFile(cvUngVien.DuongDanFile);
            db.CVUngViens.Remove(cvUngVien);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã xóa CV.";
            return RedirectToAction("CV");
        }

        public ActionResult XemCVFile(int id)
        {
            return ReturnCvFile(id, false);
        }

        public ActionResult TaiCVFile(int id)
        {
            return ReturnCvFile(id, true);
        }

        public ActionResult DonUngTuyen()
        {
            var hoSoCaNhan = GetHoSoCaNhanHienTai();
            var model = new UngVienDonUngTuyenPageViewModel
            {
                DonUngTuyens = Enumerable.Empty<UngVienDonUngTuyenItemViewModel>()
            };

            if (hoSoCaNhan == null)
            {
                return View(model);
            }

            model.DonUngTuyens = db.DonUngTuyens
                .Include(x => x.TinTuyenDung.HoSoCongTy)
                .Include(x => x.CVUngVien)
                .Where(x => x.HoSoCaNhanId == hoSoCaNhan.HoSoCaNhanId)
                .OrderByDescending(x => x.NgayNop)
                .ToList()
                .Select(x => new UngVienDonUngTuyenItemViewModel
                {
                    DonUngTuyenId = x.DonUngTuyenId,
                    TieuDeTin = x.TinTuyenDung != null ? x.TinTuyenDung.TieuDe : "Tin tuyển dụng",
                    TenCongTy = x.TinTuyenDung != null && x.TinTuyenDung.HoSoCongTy != null ? x.TinTuyenDung.HoSoCongTy.TenCongTy : "Doanh nghiệp",
                    NgayNop = x.NgayNop,
                    TrangThaiDon = x.TrangThaiDon,
                    TenCV = x.CVUngVien != null ? x.CVUngVien.TenCV : "Chưa cập nhật",
                    CoTheRutDon = CoTheRutDon(x.TrangThaiDon)
                })
                .ToList();

            return View(model);
        }

        public ActionResult ChiTietDon(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var donUngTuyen = GetDonUngTuyenHienTaiQuery().FirstOrDefault(x => x.DonUngTuyenId == id.Value);
            if (donUngTuyen == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var lichSu = db.LichSuTrangThaiDons
                .Include(x => x.ApplicationUser)
                .Where(x => x.DonUngTuyenId == donUngTuyen.DonUngTuyenId)
                .OrderByDescending(x => x.ThoiGianThayDoi)
                .ToList();

            var model = new UngVienChiTietDonViewModel
            {
                DonUngTuyen = donUngTuyen,
                LichSuTrangThaiDons = lichSu,
                CoTheRutDon = CoTheRutDon(donUngTuyen.TrangThaiDon)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RutDon(int id, string returnUrl = null)
        {
            var donUngTuyen = GetDonUngTuyenHienTaiQuery().FirstOrDefault(x => x.DonUngTuyenId == id);
            if (donUngTuyen == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (!CoTheRutDon(donUngTuyen.TrangThaiDon))
            {
                TempData["ErrorMessage"] = "Đơn ứng tuyển này không thể rút ở trạng thái hiện tại.";
                return RedirectToSafeReturnUrl(returnUrl, donUngTuyen.DonUngTuyenId);
            }

            var trangThaiCu = donUngTuyen.TrangThaiDon;
            donUngTuyen.TrangThaiDon = TrangThaiDonUngTuyen.DaRut;

            db.LichSuTrangThaiDons.Add(new LichSuTrangThaiDon
            {
                DonUngTuyenId = donUngTuyen.DonUngTuyenId,
                ApplicationUserId = User.Identity.GetUserId(),
                TrangThaiCu = trangThaiCu,
                TrangThaiMoi = TrangThaiDonUngTuyen.DaRut,
                GhiChu = "Ứng viên rút đơn ứng tuyển.",
                ThoiGianThayDoi = DateTime.Now
            });

            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã rút đơn ứng tuyển.";
            return RedirectToSafeReturnUrl(returnUrl, donUngTuyen.DonUngTuyenId);
        }

        private ActionResult ReturnCvFile(int id, bool download)
        {
            var cvUngVien = GetCvHienTai(id);
            if (cvUngVien == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (string.IsNullOrWhiteSpace(cvUngVien.DuongDanFile))
            {
                return HttpNotFound();
            }

            var physicalPath = Server.MapPath("~" + cvUngVien.DuongDanFile);
            if (!System.IO.File.Exists(physicalPath))
            {
                return HttpNotFound();
            }

            var contentType = MimeMapping.GetMimeMapping(physicalPath);
            if (download)
            {
                var extension = Path.GetExtension(physicalPath);
                return File(physicalPath, contentType, cvUngVien.TenCV + extension);
            }

            return File(physicalPath, contentType);
        }

        private UngVienCvPageViewModel BuildCvPageViewModel(UploadCvViewModel uploadForm = null)
        {
            var userId = User.Identity.GetUserId();
            var hoSoCaNhan = db.HoSoCaNhans
                .Include(x => x.CVUngViens.Select(c => c.DonUngTuyens))
                .FirstOrDefault(x => x.ApplicationUserId == userId);

            return new UngVienCvPageViewModel
            {
                HasProfile = hoSoCaNhan != null,
                UploadForm = uploadForm ?? new UploadCvViewModel(),
                TaoCvForm = BuildDefaultTaoCvForm(hoSoCaNhan),
                CvItems = hoSoCaNhan == null
                    ? Enumerable.Empty<UngVienCvItemViewModel>()
                    : hoSoCaNhan.CVUngViens
                        .OrderByDescending(x => x.NgayTaiLen)
                        .Select(x => new UngVienCvItemViewModel
                        {
                            CVUngVienId = x.CVUngVienId,
                            TenCV = x.TenCV,
                            DuongDanFile = x.DuongDanFile,
                            NgayTaiLen = x.NgayTaiLen,
                            TrangThaiSuDung = x.TrangThaiSuDung,
                            SoDonUngTuyen = x.DonUngTuyens != null ? x.DonUngTuyens.Count : 0
                        })
                        .ToList()
            };
        }

        private IQueryable<DonUngTuyen> GetDonUngTuyenHienTaiQuery()
        {
            var userId = User.Identity.GetUserId();

            return db.DonUngTuyens
                .Include(x => x.TinTuyenDung.HoSoCongTy)
                .Include(x => x.TinTuyenDung.NganhNghe)
                .Include(x => x.TinTuyenDung.DiaDiem)
                .Include(x => x.TinTuyenDung.LoaiHinhLamViec)
                .Include(x => x.TinTuyenDung.CapDoKinhNghiem)
                .Include(x => x.CVUngVien)
                .Include(x => x.HoSoCaNhan)
                .Where(x => x.HoSoCaNhan.ApplicationUserId == userId);
        }

        private HoSoCaNhan GetHoSoCaNhanHienTai()
        {
            var userId = User.Identity.GetUserId();
            return db.HoSoCaNhans.FirstOrDefault(x => x.ApplicationUserId == userId);
        }

        private CVUngVien GetCvHienTai(int id)
        {
            var userId = User.Identity.GetUserId();
            return db.CVUngViens
                .Include(x => x.HoSoCaNhan)
                .FirstOrDefault(x => x.CVUngVienId == id && x.HoSoCaNhan.ApplicationUserId == userId);
        }

        private static UngVienHoSoViewModel MapToUngVienHoSo(HoSoCaNhan hoSoCaNhan)
        {
            return new UngVienHoSoViewModel
            {
                HoSoCaNhanId = hoSoCaNhan.HoSoCaNhanId,
                HoTen = hoSoCaNhan.HoTen,
                NgaySinh = hoSoCaNhan.NgaySinh,
                GioiTinh = hoSoCaNhan.GioiTinh,
                SoDienThoai = hoSoCaNhan.SoDienThoai,
                DiaChi = hoSoCaNhan.DiaChi,
                MucTieuNgheNghiep = hoSoCaNhan.MucTieuNgheNghiep,
                HocVan = hoSoCaNhan.HocVan,
                TomTatKinhNghiem = hoSoCaNhan.TomTatKinhNghiem
            };
        }

        private static bool CoTheRutDon(string trangThaiDon)
        {
            return TrangThaiDonUngTuyen.DangXuLy.Contains(trangThaiDon);
        }

        private static void TrimHoSoModel(UngVienHoSoViewModel model)
        {
            model.HoTen = (model.HoTen ?? string.Empty).Trim();
            model.GioiTinh = NullIfWhiteSpace(model.GioiTinh);
            model.SoDienThoai = NullIfWhiteSpace(model.SoDienThoai);
            model.DiaChi = NullIfWhiteSpace(model.DiaChi);
            model.MucTieuNgheNghiep = NullIfWhiteSpace(model.MucTieuNgheNghiep);
            model.HocVan = NullIfWhiteSpace(model.HocVan);
            model.TomTatKinhNghiem = NullIfWhiteSpace(model.TomTatKinhNghiem);
        }

        private static string ValidateCvFile(HttpPostedFileBase cvFile)
        {
            if (cvFile == null || cvFile.ContentLength <= 0)
            {
                return "Vui lòng chọn tệp CV để tải lên.";
            }

            var extension = Path.GetExtension(cvFile.FileName);
            if (!AllowedCvExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return "CV chỉ hỗ trợ định dạng PDF, DOC hoặc DOCX.";
            }

            if (cvFile.ContentLength > CvMaxSizeInBytes)
            {
                return "Dung lượng CV tối đa là 5MB.";
            }

            if (!string.IsNullOrWhiteSpace(cvFile.ContentType) &&
                !AllowedCvContentTypes.Contains(cvFile.ContentType, StringComparer.OrdinalIgnoreCase))
            {
                return "Nội dung tệp CV không đúng định dạng PDF, DOC hoặc DOCX.";
            }

            return null;
        }

        private string SaveCvFile(HttpPostedFileBase cvFile)
        {
            var extension = Path.GetExtension(cvFile.FileName);
            var fileName = string.Format("{0:N}{1}", Guid.NewGuid(), extension);
            var cvFolder = Server.MapPath("~/Uploads/CV");
            Directory.CreateDirectory(cvFolder);
            var filePath = Path.Combine(cvFolder, fileName);
            cvFile.SaveAs(filePath);
            return "/Uploads/CV/" + fileName;
        }

        private string SaveGeneratedCvFile(HoSoCaNhan hoSoCaNhan, TaoCvViewModel model, string tenCv)
        {
            var safeName = MakeSafeFileName(tenCv);
            var fileName = string.Format("{0}-{1:N}.doc", safeName, Guid.NewGuid());
            var cvFolder = Server.MapPath("~/Uploads/CV");
            Directory.CreateDirectory(cvFolder);
            var filePath = Path.Combine(cvFolder, fileName);
            System.IO.File.WriteAllText(filePath, BuildGeneratedCvHtml(hoSoCaNhan, model), Encoding.UTF8);
            return "/Uploads/CV/" + fileName;
        }

        private static TaoCvViewModel BuildDefaultTaoCvForm(HoSoCaNhan hoSoCaNhan)
        {
            if (hoSoCaNhan == null)
            {
                return new TaoCvViewModel();
            }

            return new TaoCvViewModel
            {
                TenCV = "CV " + hoSoCaNhan.HoTen,
                KinhNghiem = hoSoCaNhan.TomTatKinhNghiem
            };
        }

        private static string BuildGeneratedCvHtml(HoSoCaNhan hoSoCaNhan, TaoCvViewModel model)
        {
            var html = new StringBuilder();
            html.AppendLine("<!doctype html>");
            html.AppendLine("<html><head><meta charset=\"utf-8\" />");
            html.AppendLine("<style>");
            html.AppendLine("body{font-family:Arial,Helvetica,sans-serif;color:#18212a;line-height:1.5;margin:42px;}h1{font-size:28px;margin:0 0 6px;}h2{font-size:15px;margin:24px 0 8px;padding-bottom:6px;border-bottom:1px solid #d9dee3;text-transform:uppercase;letter-spacing:.05em;}p{margin:0 0 8px}.muted{color:#68727d}.contact{margin:10px 0 22px}.section{margin-bottom:14px}.target{font-size:16px;font-weight:bold;color:#2d3339}.generated{margin-top:28px;font-size:11px;color:#8a939c}");
            html.AppendLine("</style></head><body>");
            html.AppendFormat("<h1>{0}</h1>", Encode(hoSoCaNhan.HoTen));
            html.AppendFormat("<p class=\"target\">{0}</p>", Encode(FirstValue(model.ViTriMongMuon, "Ứng viên tìm việc")));
            html.Append("<div class=\"contact muted\">");
            html.Append(Encode(JoinNonEmpty(" | ", hoSoCaNhan.SoDienThoai, hoSoCaNhan.ApplicationUser != null ? hoSoCaNhan.ApplicationUser.Email : null, hoSoCaNhan.DiaChi)));
            html.AppendLine("</div>");

            AppendSection(html, "Mục tiêu nghề nghiệp", FirstValue(hoSoCaNhan.MucTieuNgheNghiep, "Mong muốn tìm kiếm công việc phù hợp để phát triển chuyên môn và đóng góp hiệu quả cho doanh nghiệp."));
            AppendSection(html, "Kỹ năng chính", model.KyNang);
            AppendSection(html, "Kinh nghiệm", FirstValue(model.KinhNghiem, hoSoCaNhan.TomTatKinhNghiem));
            AppendSection(html, "Học vấn", hoSoCaNhan.HocVan);
            AppendSection(html, "Dự án / thành tích", model.DuAnThanhTich);
            AppendSection(html, "Chứng chỉ / hoạt động", model.ChungChiHoatDong);

            html.AppendFormat("<p class=\"generated\">CV được tạo từ VietJob ngày {0:dd/MM/yyyy HH:mm}</p>", DateTime.Now);
            html.AppendLine("</body></html>");
            return html.ToString();
        }

        private static void AppendSection(StringBuilder html, string title, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            html.AppendFormat("<div class=\"section\"><h2>{0}</h2>{1}</div>", Encode(title), ToHtmlParagraphs(content));
        }

        private static string ToHtmlParagraphs(string content)
        {
            var lines = content
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => "<p>" + Encode(x) + "</p>");

            return string.Join(Environment.NewLine, lines);
        }

        private static string JoinNonEmpty(string separator, params string[] values)
        {
            return string.Join(separator, values.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
        }

        private static string FirstValue(params string[] values)
        {
            return values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        }

        private static string Encode(string value)
        {
            return HttpUtility.HtmlEncode(value ?? string.Empty);
        }

        private static string MakeSafeFileName(string value)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var safe = new string((value ?? "cv").Select(ch => invalidChars.Contains(ch) ? '-' : ch).ToArray()).Trim();
            return string.IsNullOrWhiteSpace(safe) ? "cv" : safe;
        }

        private void DeleteLocalCvFile(string currentPath)
        {
            if (string.IsNullOrWhiteSpace(currentPath) || !currentPath.StartsWith("/Uploads/CV/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var physicalPath = Server.MapPath("~" + currentPath);
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }

        private ActionResult RedirectToSafeReturnUrl(string returnUrl, int donUngTuyenId)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("ChiTietDon", new { id = donUngTuyenId });
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
