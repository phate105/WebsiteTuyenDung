using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class NguoiDung
    {
        [Key]
        public int NguoiDungId { get; set; }

        [Required]
        [StringLength(50)]
        [Index("IX_TenDangNhap", IsUnique = true)]
        public string TenDangNhap { get; set; }
        [Required]
        [StringLength(100)]
        [Index("IX_Email",IsUnique = true)]
        public string Email { get; set; }
        [Required]
        [StringLength(50)]
        public string MatKhau { get; set; }

        public bool TrangThaiHoatDong { get; set; } = true;

        [Required]
        public int VaiTroId { get; set; }


        // Navigation
        [ForeignKey("VaiTroId")]
        public virtual VaiTro VaiTro { get; set; }

        public virtual HoSoCongTy HoSoCongTy { get; set; }
        public virtual HoSoCaNhan HoSoCaNhan { get; set; }
    }
}