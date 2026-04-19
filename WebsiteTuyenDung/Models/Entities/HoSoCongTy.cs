using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebsiteTuyenDung.Models;

namespace WebsiteTuyenDung.Models.Entities
{
    public class HoSoCongTy
    {
        [Key]
        public int HoSoCongTyId { get; set; }

        [Required]
        [StringLength(128)]
        [Index("IX_HoSoCongTy_ApplicationUserId", IsUnique = true)]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [StringLength(200)]
        public string TenCongTy { get; set; }

        [StringLength(50)]
        public string MaSoThue { get; set; }

        public string MoTa { get; set; }

        [StringLength(300)]
        public string DiaChi { get; set; }

        [StringLength(255)]
        public string Website { get; set; }

        [StringLength(255)]
        public string Logo { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public string EmailLienHe { get; set; }

        [StringLength(20)]
        public string SoDienThoaiLienHe { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }

        public HoSoCongTy()
        {
            TinTuyenDungs = new HashSet<TinTuyenDung>();
        }
    }
}
