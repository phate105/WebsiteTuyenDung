using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebsiteTuyenDung.Models;

namespace WebsiteTuyenDung.Models.Entities
{
    public class LichSuTrangThaiDon
    {
        [Key]
        public int LichSuTrangThaiDonId { get; set; }

        [Required]
        public int DonUngTuyenId { get; set; }

        [Required]
        [StringLength(128)]
        public string ApplicationUserId { get; set; }

        [StringLength(30)]
        public string TrangThaiCu { get; set; }

        [StringLength(30)]
        public string TrangThaiMoi { get; set; }

        public string GhiChu { get; set; }

        public DateTime ThoiGianThayDoi { get; set; } = DateTime.Now;

        [ForeignKey("DonUngTuyenId")]
        public virtual DonUngTuyen DonUngTuyen { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
