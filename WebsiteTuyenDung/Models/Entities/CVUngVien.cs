using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class CVUngVien
    {
        [Key]
        public int CVUngVienId { get; set; }
        
        public int HoSoCaNhanId { get; set; }

        public string TenCV { get; set; }
        public string DuongDanFile { get; set; }

        public DateTime NgayTaiLen { get; set; }
        public bool TrangThaiSuDung { get; set; } 

        [ForeignKey("HoSoCaNhanId")]
        public virtual HoSoCaNhan HoSoCaNhan { get; set; }
    }
}