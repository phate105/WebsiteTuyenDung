using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class CVUngVien
    {
        [Key]
        public int CVUngVienId { get; set; }

        [Required]
        public int HoSoCaNhanId { get; set; }

        [Required]
        [StringLength(200)]
        public string TenCV { get; set; }

        [Required]
        [StringLength(255)]
        public string DuongDanFile { get; set; }

        public DateTime NgayTaiLen { get; set; } = DateTime.Now;

        public bool TrangThaiSuDung { get; set; }

        [ForeignKey("HoSoCaNhanId")]
        public virtual HoSoCaNhan HoSoCaNhan { get; set; }

        public virtual ICollection<DonUngTuyen> DonUngTuyens { get; set; }

        public CVUngVien()
        {
            DonUngTuyens = new HashSet<DonUngTuyen>();
        }
    }
}