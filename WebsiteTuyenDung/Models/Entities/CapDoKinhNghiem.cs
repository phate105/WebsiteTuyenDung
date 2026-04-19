using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class CapDoKinhNghiem
    {
        [Key]
        public int CapDoKinhNghiemId { get; set; }

        [Required]
        [StringLength(100)]
        [Index("IX_CapDoKinhNghiem_TenCapDoKinhNghiem", IsUnique = true)]
        public string TenCapDoKinhNghiem { get; set; }

        public bool TrangThai { get; set; }

        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }

        public CapDoKinhNghiem()
        {
            TinTuyenDungs = new HashSet<TinTuyenDung>();
        }
    }
}
