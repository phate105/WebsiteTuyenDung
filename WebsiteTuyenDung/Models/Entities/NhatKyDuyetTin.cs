using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebsiteTuyenDung.Models;

namespace WebsiteTuyenDung.Models.Entities
{
    public class NhatKyDuyetTin
    {
        [Key]
        public int NhatKyDuyetTinId { get; set; }

        [Required]
        public int TinTuyenDungId { get; set; }

        [Required]
        [StringLength(128)]
        public string ApplicationUserId { get; set; }

        [Required]
        [StringLength(30)]
        public string HanhDong { get; set; }

        public string LyDoTuChoi { get; set; }

        public DateTime ThoiGianXuLy { get; set; } = DateTime.Now;

        [ForeignKey("TinTuyenDungId")]
        public virtual TinTuyenDung TinTuyenDung { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
