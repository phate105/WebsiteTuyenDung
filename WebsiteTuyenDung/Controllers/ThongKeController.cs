using System.Linq;
using System.Web.Mvc;
using WebsiteTuyenDung.Models;
using WebsiteTuyenDung.Models.Constants;
using WebsiteTuyenDung.Models.ViewModels;

namespace WebsiteTuyenDung.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var nhaTuyenDungRole = db.Roles.FirstOrDefault(x => x.Name == ApplicationRoles.NhaTuyenDung);
            var ungVienRole = db.Roles.FirstOrDefault(x => x.Name == ApplicationRoles.UngVien);

            var model = new AdminThongKeViewModel
            {
                TongTaiKhoan = db.Users.Count(),
                TongUngVien = ungVienRole == null ? 0 : db.Users.Count(x => x.Roles.Any(r => r.RoleId == ungVienRole.Id)),
                TongNhaTuyenDung = nhaTuyenDungRole == null ? 0 : db.Users.Count(x => x.Roles.Any(r => r.RoleId == nhaTuyenDungRole.Id)),
                TongHoSoCongTy = db.HoSoCongTys.Count(),
                TongTinTuyenDung = db.TinTuyenDungs.Count(),
                TongDonUngTuyen = db.DonUngTuyens.Count(),
                ThongKeTinTheoTrangThai = new[]
                {
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiTinTuyenDung.Nhap, TenHienThi = TrangThaiTinTuyenDung.ToDisplayText(TrangThaiTinTuyenDung.Nhap), SoLuong = db.TinTuyenDungs.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.Nhap) },
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiTinTuyenDung.ChoDuyet, TenHienThi = TrangThaiTinTuyenDung.ToDisplayText(TrangThaiTinTuyenDung.ChoDuyet), SoLuong = db.TinTuyenDungs.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.ChoDuyet) },
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiTinTuyenDung.DaDuyet, TenHienThi = TrangThaiTinTuyenDung.ToDisplayText(TrangThaiTinTuyenDung.DaDuyet), SoLuong = db.TinTuyenDungs.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.DaDuyet) },
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiTinTuyenDung.BiTuChoi, TenHienThi = TrangThaiTinTuyenDung.ToDisplayText(TrangThaiTinTuyenDung.BiTuChoi), SoLuong = db.TinTuyenDungs.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.BiTuChoi) },
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiTinTuyenDung.DaDong, TenHienThi = TrangThaiTinTuyenDung.ToDisplayText(TrangThaiTinTuyenDung.DaDong), SoLuong = db.TinTuyenDungs.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.DaDong) },
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiTinTuyenDung.HetHan, TenHienThi = TrangThaiTinTuyenDung.ToDisplayText(TrangThaiTinTuyenDung.HetHan), SoLuong = db.TinTuyenDungs.Count(x => x.TrangThaiTin == TrangThaiTinTuyenDung.HetHan) }
                },
                ThongKeDonTheoTrangThai = new[]
                {
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiDonUngTuyen.DaNop, TenHienThi = TrangThaiDonUngTuyen.ToDisplayText(TrangThaiDonUngTuyen.DaNop), SoLuong = db.DonUngTuyens.Count(x => x.TrangThaiDon == TrangThaiDonUngTuyen.DaNop) },
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiDonUngTuyen.DaXem, TenHienThi = TrangThaiDonUngTuyen.ToDisplayText(TrangThaiDonUngTuyen.DaXem), SoLuong = db.DonUngTuyens.Count(x => x.TrangThaiDon == TrangThaiDonUngTuyen.DaXem) },
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiDonUngTuyen.VaoDanhSachNgan, TenHienThi = TrangThaiDonUngTuyen.ToDisplayText(TrangThaiDonUngTuyen.VaoDanhSachNgan), SoLuong = db.DonUngTuyens.Count(x => x.TrangThaiDon == TrangThaiDonUngTuyen.VaoDanhSachNgan) },
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiDonUngTuyen.BiTuChoi, TenHienThi = TrangThaiDonUngTuyen.ToDisplayText(TrangThaiDonUngTuyen.BiTuChoi), SoLuong = db.DonUngTuyens.Count(x => x.TrangThaiDon == TrangThaiDonUngTuyen.BiTuChoi) },
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiDonUngTuyen.DuocChapNhan, TenHienThi = TrangThaiDonUngTuyen.ToDisplayText(TrangThaiDonUngTuyen.DuocChapNhan), SoLuong = db.DonUngTuyens.Count(x => x.TrangThaiDon == TrangThaiDonUngTuyen.DuocChapNhan) },
                    new AdminThongKeTrangThaiItemViewModel { TrangThai = TrangThaiDonUngTuyen.DaRut, TenHienThi = TrangThaiDonUngTuyen.ToDisplayText(TrangThaiDonUngTuyen.DaRut), SoLuong = db.DonUngTuyens.Count(x => x.TrangThaiDon == TrangThaiDonUngTuyen.DaRut) }
                },
                TopCongTyTheoHoatDong = db.HoSoCongTys
                    .Select(x => new AdminThongKeCongTyItemViewModel
                    {
                        TenCongTy = x.TenCongTy,
                        SoTinTuyenDung = x.TinTuyenDungs.Count(),
                        SoDonUngTuyen = x.TinTuyenDungs.SelectMany(t => t.DonUngTuyens).Count()
                    })
                    .OrderByDescending(x => x.SoDonUngTuyen)
                    .ThenByDescending(x => x.SoTinTuyenDung)
                    .ThenBy(x => x.TenCongTy)
                    .Take(5)
                    .ToList()
            };

            return View(model);
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
