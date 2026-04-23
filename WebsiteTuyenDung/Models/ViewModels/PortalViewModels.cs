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

        [StringLength(2000, ErrorMessage = "Mô tả công ty tối đa 2000 ký tự.")]
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
        [RegularExpression(@"^[0-9+\s().-]{8,20}$", ErrorMessage = "Số điện thoại liên hệ không hợp lệ.")]
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

    public class AdminTaiKhoanEditViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(256)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập username.")]
        [StringLength(256)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò.")]
        public string VaiTro { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }

        public bool BiKhoa { get; set; }
        public bool LaTaiKhoanHienTai { get; set; }
        public bool CoHoSoCaNhan { get; set; }
        public bool CoHoSoCongTy { get; set; }

        [StringLength(150)]
        public string HoTen { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^[0-9+\s().-]{8,20}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SoDienThoai { get; set; }

        [StringLength(300)]
        public string DiaChi { get; set; }

        [StringLength(200)]
        public string TenCongTy { get; set; }

        [StringLength(255)]
        public string Website { get; set; }

        [EmailAddress(ErrorMessage = "Email liên hệ không hợp lệ.")]
        [StringLength(256)]
        public string EmailLienHe { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^[0-9+\s().-]{8,20}$", ErrorMessage = "Số điện thoại liên hệ không hợp lệ.")]
        public string SoDienThoaiLienHe { get; set; }

        [StringLength(300)]
        public string DiaChiCongTy { get; set; }
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

    public class PublicJobCardViewModel
    {
        public int TinTuyenDungId { get; set; }
        public int? HoSoCongTyId { get; set; }
        public string TieuDe { get; set; }
        public string TenCongTy { get; set; }
        public string LogoCongTy { get; set; }
        public string TenDiaDiem { get; set; }
        public string TenLoaiHinhLamViec { get; set; }
        public string TenNganhNghe { get; set; }
        public string TenCapDoKinhNghiem { get; set; }
        public string MoTaNgan { get; set; }
        public string LuongHienThi { get; set; }
        public DateTime? NgayDang { get; set; }
        public DateTime HanNopHoSo { get; set; }
        public bool CoTheUngTuyen { get; set; }
    }

    public class PublicHomeCategoryViewModel
    {
        public int NganhNgheId { get; set; }
        public string TenNganhNghe { get; set; }
        public int SoLuongTin { get; set; }
    }

    public class FeaturedEmployerViewModel
    {
        public int HoSoCongTyId { get; set; }
        public string TenCongTy { get; set; }
        public string LogoCongTy { get; set; }
        public string DiaChi { get; set; }
        public string Website { get; set; }
        public int SoLuongTinDangTuyen { get; set; }
    }

    public class HomeIndexViewModel
    {
        public string Keyword { get; set; }
        public int? NganhNgheId { get; set; }
        public int? DiaDiemId { get; set; }
        public IEnumerable<SelectListItem> NganhNgheOptions { get; set; }
        public IEnumerable<SelectListItem> DiaDiemOptions { get; set; }
        public IEnumerable<PublicHomeCategoryViewModel> NganhNgheNoiBat { get; set; }
        public IEnumerable<PublicJobCardViewModel> TinTuyenDungMoiNhat { get; set; }
        public IEnumerable<FeaturedEmployerViewModel> NhaTuyenDungTieuBieu { get; set; }
    }

    public class JobIndexViewModel
    {
        public string Keyword { get; set; }
        public int? NganhNgheId { get; set; }
        public int? DiaDiemId { get; set; }
        public int? LoaiHinhLamViecId { get; set; }
        public int? CapDoKinhNghiemId { get; set; }
        public IEnumerable<SelectListItem> NganhNgheOptions { get; set; }
        public IEnumerable<SelectListItem> DiaDiemOptions { get; set; }
        public IEnumerable<SelectListItem> LoaiHinhLamViecOptions { get; set; }
        public IEnumerable<SelectListItem> CapDoKinhNghiemOptions { get; set; }
        public IEnumerable<PublicJobCardViewModel> Jobs { get; set; }
        public int TongKetQua { get; set; }
        public int TrangHienTai { get; set; }
        public int TongTrang { get; set; }
        public int KichThuocTrang { get; set; }
    }

    public class ApplyForJobViewModel
    {
        [Required]
        public int TinTuyenDungId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn CV để ứng tuyển.")]
        public int? CVUngVienId { get; set; }

        [StringLength(1000, ErrorMessage = "Thư giới thiệu tối đa 1000 ký tự.")]
        [Display(Name = "Thư giới thiệu")]
        public string ThuGioiThieu { get; set; }
    }

    public class JobDetailsViewModel
    {
        public TinTuyenDung TinTuyenDung { get; set; }
        public IEnumerable<SelectListItem> CvOptions { get; set; }
        public ApplyForJobViewModel ApplyForm { get; set; }
        public IEnumerable<PublicJobCardViewModel> RelatedJobs { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IsCandidate { get; set; }
        public bool HasProfile { get; set; }
        public bool HasCv { get; set; }
        public bool AlreadyApplied { get; set; }
        public bool CoTheUngTuyen { get; set; }
        public string ApplyMessage { get; set; }
    }

    public class CompanyDetailsViewModel
    {
        public HoSoCongTy CongTy { get; set; }
        public IEnumerable<PublicJobCardViewModel> Jobs { get; set; }
        public int SoViecDangTuyen { get; set; }
    }

    public class UngVienHoSoViewModel
    {
        public int? HoSoCaNhanId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(150)]
        public string HoTen { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [StringLength(20)]
        public string GioiTinh { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^[0-9+\s().-]{8,20}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SoDienThoai { get; set; }

        [StringLength(300)]
        public string DiaChi { get; set; }

        [Display(Name = "Mục tiêu nghề nghiệp")]
        [StringLength(1500, ErrorMessage = "Mục tiêu nghề nghiệp tối đa 1500 ký tự.")]
        public string MucTieuNgheNghiep { get; set; }

        [StringLength(3000, ErrorMessage = "Học vấn tối đa 3000 ký tự.")]
        public string HocVan { get; set; }

        [StringLength(3000, ErrorMessage = "Kinh nghiệm tối đa 3000 ký tự.")]
        public string TomTatKinhNghiem { get; set; }
    }

    public class UngVienCvItemViewModel
    {
        public int CVUngVienId { get; set; }
        public string TenCV { get; set; }
        public string DuongDanFile { get; set; }
        public DateTime NgayTaiLen { get; set; }
        public bool TrangThaiSuDung { get; set; }
        public int SoDonUngTuyen { get; set; }
    }

    public class UploadCvViewModel
    {
        [StringLength(200)]
        [Display(Name = "Tên CV")]
        public string TenCV { get; set; }
    }

    public class TaoCvViewModel
    {
        [StringLength(200)]
        [Display(Name = "Tên CV")]
        public string TenCV { get; set; }

        [Display(Name = "Vị trí mong muốn")]
        [StringLength(150, ErrorMessage = "Vị trí mong muốn tối đa 150 ký tự.")]
        public string ViTriMongMuon { get; set; }

        [Display(Name = "Kỹ năng chính")]
        [StringLength(1500, ErrorMessage = "Kỹ năng chính tối đa 1500 ký tự.")]
        public string KyNang { get; set; }

        [Display(Name = "Kinh nghiệm nổi bật")]
        [StringLength(2500, ErrorMessage = "Kinh nghiệm nổi bật tối đa 2500 ký tự.")]
        public string KinhNghiem { get; set; }

        [Display(Name = "Dự án / thành tích")]
        [StringLength(2000, ErrorMessage = "Dự án / thành tích tối đa 2000 ký tự.")]
        public string DuAnThanhTich { get; set; }

        [Display(Name = "Chứng chỉ / hoạt động")]
        [StringLength(1500, ErrorMessage = "Chứng chỉ / hoạt động tối đa 1500 ký tự.")]
        public string ChungChiHoatDong { get; set; }
    }

    public class UngVienCvPageViewModel
    {
        public bool HasProfile { get; set; }
        public IEnumerable<UngVienCvItemViewModel> CvItems { get; set; }
        public UploadCvViewModel UploadForm { get; set; }
        public TaoCvViewModel TaoCvForm { get; set; }
    }

    public class UngVienDonUngTuyenItemViewModel
    {
        public int DonUngTuyenId { get; set; }
        public string TieuDeTin { get; set; }
        public string TenCongTy { get; set; }
        public DateTime NgayNop { get; set; }
        public string TrangThaiDon { get; set; }
        public string TenCV { get; set; }
        public bool CoTheRutDon { get; set; }
    }

    public class UngVienDonUngTuyenPageViewModel
    {
        public IEnumerable<UngVienDonUngTuyenItemViewModel> DonUngTuyens { get; set; }
    }

    public class UngVienChiTietDonViewModel
    {
        public DonUngTuyen DonUngTuyen { get; set; }
        public IEnumerable<LichSuTrangThaiDon> LichSuTrangThaiDons { get; set; }
        public bool CoTheRutDon { get; set; }
    }
}
