using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTuyenDung.Models
{
    public class TinTuyenDung
    {
        [Key]
        public int ID { get; set; }

        public int CongTyID { get; set; }

        public int DanhMucID { get; set; }

        [Required]
        public string TieuDe { get; set; }

        public string MoTa { get; set; }

        public string TrangThai { get; set; } 

        public DateTime NgayDang { get; set; } = DateTime.Now;

        public virtual CongTy CongTy { get; set; }

        public virtual DanhMuc DanhMuc { get; set; }
    }
}