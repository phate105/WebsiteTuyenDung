using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class DonUngTuyen
    {
        [Key]
        public int DonUngTuyenId { get; set; }

        [Index("IX_DonUngTuyen_TinTuyenDung_HoSoCaNhan", 2, IsUnique = true)]
        [Required]
        public int HoSoCaNhanId { get; set; }

        [Index("IX_DonUngTuyen_TinTuyenDung_HoSoCaNhan", 1, IsUnique = true)]
        [Required]
        public int TinTuyenDungId { get; set; }

        [Required]
        public int CVUngVienId { get; set; }

        public DateTime NgayNop { get; set; } = DateTime.Now;

        [Required]
        [StringLength(30)]
        public string TrangThaiDon { get; set; } = "DaNop";

        public string ThuGioiThieu { get; set; }
        public string GhiChuXuLy { get; set; }

        [ForeignKey("HoSoCaNhanId")]
        public virtual HoSoCaNhan HoSoCaNhan { get; set; }

        [ForeignKey("TinTuyenDungId")]
        public virtual TinTuyenDung TinTuyenDung { get; set; }

        [ForeignKey("CVUngVienId")]
        public virtual CVUngVien CVUngVien { get; set; }

        public virtual ICollection<LichSuTrangThaiDon> LichSuTrangThaiDons { get; set; }

        public DonUngTuyen()
        {
            LichSuTrangThaiDons = new HashSet<LichSuTrangThaiDon>();
        }
    }
}
