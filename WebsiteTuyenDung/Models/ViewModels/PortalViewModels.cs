using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebsiteTuyenDung.Models.Entities;

namespace WebsiteTuyenDung.Models.ViewModels
{
    public class HoSoCongTyFormViewModel
    {
        public int? HoSoCongTyId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên công ty.")]
        [StringLength(200)]
        [Display(Name = "Tên công ty")]
        public string TenCongTy { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã số thuế")]
        public string MaSoThue { get; set; }

        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [StringLength(300)]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [StringLength(255)]
        [Display(Name = "Website")]
        public string Website { get; set; }

        [EmailAddress(ErrorMessage = "Email liên hệ không hợp lệ.")]
        [StringLength(256)]
        [Display(Name = "Email liên hệ")]
        public string EmailLienHe { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại liên hệ")]
        public string SoDienThoaiLienHe { get; set; }

        public string CurrentLogo { get; set; }
    }

    public class EmployerDashboardViewModel
    {
        public bool HasCompanyProfile { get; set; }
        public string CompanyName { get; set; }
        public int SoTinNhap { get; set; }
        public int SoTinChoDuyet { get; set; }
        public int SoTinDaDuyet { get; set; }
        public int SoTinBiTuChoi { get; set; }
        public int SoHoSoChoXuLy { get; set; }
        public int TongHoSoUngTuyen { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int SoTinChoDuyet { get; set; }
        public int SoTinDaDuyet { get; set; }
        public int SoTinBiTuChoi { get; set; }
        public int SoTinDaDong { get; set; }
        public int TongTinTuyenDung { get; set; }
        public int TongDonUngTuyen { get; set; }
        public int TongTaiKhoan { get; set; }
        public int SoCongTy { get; set; }
        public int SoNhaTuyenDung { get; set; }
        public int SoUngVien { get; set; }
    }

    public class AdminTaiKhoanItemViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string VaiTro { get; set; }
        public string ThongTinLienKet { get; set; }
        public bool BiKhoa { get; set; }
        public DateTime? KhoaDenLuc { get; set; }
        public int SoTinTuyenDung { get; set; }
        public int SoDonUngTuyen { get; set; }
        public bool LaTaiKhoanHienTai { get; set; }
    }

    public class AdminTaiKhoanIndexViewModel
    {
        public IEnumerable<AdminTaiKhoanItemViewModel> TaiKhoans { get; set; }
        public string VaiTro { get; set; }
        public string TrangThai { get; set; }
        public int TongTaiKhoan { get; set; }
        public int TongDangHoatDong { get; set; }
        public int TongBiKhoa { get; set; }
    }

    public class AdminThongKeTrangThaiItemViewModel
    {
        public string TrangThai { get; set; }
        public string TenHienThi { get; set; }
        public int SoLuong { get; set; }
    }

    public class AdminThongKeCongTyItemViewModel
    {
        public string TenCongTy { get; set; }
        public int SoTinTuyenDung { get; set; }
        public int SoDonUngTuyen { get; set; }
    }

    public class AdminThongKeViewModel
    {
        public int TongTaiKhoan { get; set; }
        public int TongUngVien { get; set; }
        public int TongNhaTuyenDung { get; set; }
        public int TongHoSoCongTy { get; set; }
        public int TongTinTuyenDung { get; set; }
        public int TongDonUngTuyen { get; set; }
        public IEnumerable<AdminThongKeTrangThaiItemViewModel> ThongKeTinTheoTrangThai { get; set; }
        public IEnumerable<AdminThongKeTrangThaiItemViewModel> ThongKeDonTheoTrangThai { get; set; }
        public IEnumerable<AdminThongKeCongTyItemViewModel> TopCongTyTheoHoatDong { get; set; }
    }

    public class HoSoUngTuyenIndexViewModel
    {
        public IEnumerable<DonUngTuyen> DonUngTuyens { get; set; }
        public IEnumerable<SelectListItem> TinTuyenDungOptions { get; set; }
        public int? TinTuyenDungId { get; set; }
        public string TrangThai { get; set; }
    }

    public class DonUngTuyenDetailsViewModel
    {
        public DonUngTuyen DonUngTuyen { get; set; }
        public IEnumerable<LichSuTrangThaiDon> LichSuTrangThaiDons { get; set; }
        public IEnumerable<SelectListItem> TrangThaiOptions { get; set; }
        public bool CoTheCapNhat { get; set; }
    }
}
