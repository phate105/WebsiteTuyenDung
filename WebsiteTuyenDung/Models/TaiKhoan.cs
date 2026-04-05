using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTuyenDung.Models
{
    public class TaiKhoan
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string MatKhau { get; set; }

        public string VaiTro { get; set; } 

        public bool TrangThai { get; set; } 
    }
}