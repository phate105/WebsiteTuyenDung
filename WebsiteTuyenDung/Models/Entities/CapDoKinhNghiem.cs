using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTuyenDung.Models.Entities
{
    public class CapDoKinhNghiem
    {
        [Key]
        public int CapDoKinhNghiemId { get; set; }

        public string TenCapDoKinhNghiem { get; set; }
        public bool TrangThai { get; set; }

        // Navigation
        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }
    }
}