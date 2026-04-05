using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTuyenDung.Models
{
    public class DonUngTuyen
    {
        [Key]
        public int ID { get; set; }

        public int TinID { get; set; }

        public int HoSoID { get; set; }

        public DateTime NgayNop { get; set; } = DateTime.Now;

        public string TrangThaiDon { get; set; } 

        public virtual TinTuyenDung TinTuyenDung { get; set; }

        public virtual HoSoUngVien HoSoUngVien { get; set; }
    }
}