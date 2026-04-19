using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class TinTuyenDung
    {
        [Key]
        public int TinTuyenDungId { get; set; }

        [Required]
        public int HoSoCongTyId { get; set; }

        [Required]
        public int NganhNgheId { get; set; }

        [Required]
        public int DiaDiemId { get; set; }

        [Required]
        public int LoaiHinhLamViecId { get; set; }

        [Required]
        public int CapDoKinhNghiemId { get; set; }

        [Required]
        [StringLength(200)]
        public string TieuDe { get; set; }

        [Required]
        public string MoTaCongViec { get; set; }

        public string YeuCau { get; set; }
        public string QuyenLoi { get; set; }

        public int SoLuongTuyen { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? LuongToiThieu { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? LuongToiDa { get; set; }

        public DateTime? NgayDang { get; set; }

        [Required]
        public DateTime HanNopHoSo { get; set; }

        [Required]
        [StringLength(30)]
        public string TrangThaiTin { get; set; } = "Nhap";

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [ForeignKey("HoSoCongTyId")]
        public virtual HoSoCongTy HoSoCongTy { get; set; }

        [ForeignKey("NganhNgheId")]
        public virtual NganhNghe NganhNghe { get; set; }

        [ForeignKey("DiaDiemId")]
        public virtual DiaDiem DiaDiem { get; set; }

        [ForeignKey("LoaiHinhLamViecId")]
        public virtual LoaiHinhLamViec LoaiHinhLamViec { get; set; }

        [ForeignKey("CapDoKinhNghiemId")]
        public virtual CapDoKinhNghiem CapDoKinhNghiem { get; set; }

        public virtual ICollection<DonUngTuyen> DonUngTuyens { get; set; }
        public virtual ICollection<NhatKyDuyetTin> NhatKyDuyetTins { get; set; }

        public TinTuyenDung()
        {
            DonUngTuyens = new HashSet<DonUngTuyen>();
            NhatKyDuyetTins = new HashSet<NhatKyDuyetTin>();
        }
    }
}
