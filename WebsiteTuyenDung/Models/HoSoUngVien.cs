using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTuyenDung.Models
{
    public class HoSoUngVien
    {
        [Key]
        public int ID { get; set; }

        public int TaiKhoanID { get; set; }

        [Required]
        public string HoTen { get; set; }

        public string SoDienThoai { get; set; }

        public string FileCV { get; set; } 

        public virtual TaiKhoan TaiKhoan { get; set; }
    }
}