using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class LoaiHinhLamViec
    {
        [Key]
        public int LoaiHinhLamViecId { get; set; }

        [Required]
        [StringLength(100)]
        [Index("IX_LoaiHinhLamViec_TenLoaiHinhLamViec", IsUnique = true)]
        public string TenLoaiHinhLamViec { get; set; }

        public bool TrangThai { get; set; }

        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }

        public LoaiHinhLamViec()
        {
            TinTuyenDungs = new HashSet<TinTuyenDung>();
        }
    }
}
