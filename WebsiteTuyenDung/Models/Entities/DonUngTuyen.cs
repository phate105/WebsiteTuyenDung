using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class DonUngTuyen
    {
        [Key]
        public int DonUngTuyenId { get; set; }

        [Index("IX_UniqueApply", 2, IsUnique = true)]
        public int HoSoCaNhanId { get; set; }
        [Index("IX_UniqueApply", 1, IsUnique = true)]
        public int TinTuyenDungId { get; set; }
        [Required]
        public int CVUngVienId { get; set; }

        public DateTime NgayNop { get; set; }
        [Required]
        public string TrangThaiDon { get; set; } // Đang xử lý, Chấp nhận, Từ chối

        [ForeignKey("TinTuyenDungId")]
        public virtual TinTuyenDung TinTuyenDung { get; set; }
    }
}