using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTuyenDung.Models.Entities
{
    public class LoaiHinhLamViec
    {
        [Key]
        public int LoaiHinhLamViecId { get; set; }

        public string TenLoaiHinhLamViec { get; set; }
        public bool TrangThai { get; set; }

        // Navigation
        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }
    }
}