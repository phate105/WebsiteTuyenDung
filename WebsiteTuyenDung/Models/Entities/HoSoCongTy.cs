using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class HoSoCongTy
    {
        [Key, ForeignKey("NguoiDung")]
        public int NguoiDungId { get; set; }

        [Required]
        public string TenCongTy { get; set; }

        [StringLength(20)]
        public string MaSoThue { get; set; }

        public string MoTa { get; set; }

        public string DiaChi { get; set; }

        public string Website { get; set; }

        public string Logo { get; set; }

        [EmailAddress]
        public string EmailLienHe { get; set; }

        [StringLength(15)]
        public string SoDienThoaiLienHe { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public virtual NguoiDung NguoiDung { get; set; }

        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }

        public HoSoCongTy()
        {
            TinTuyenDungs = new HashSet<TinTuyenDung>();
        }
    }
}