using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTuyenDung.Models
{
    public class CongTy
    {
        [Key]
        public int ID { get; set; }

        public int TaiKhoanID { get; set; }

        [Required]
        public string TenCongTy { get; set; }

        public string DiaChi { get; set; }

        public string Logo { get; set; }

        public virtual TaiKhoan TaiKhoan { get; set; }

        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }
    }
}