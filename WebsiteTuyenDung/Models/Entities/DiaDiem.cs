using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class DiaDiem
    {
        [Key]
        public int DiaDiemId { get; set; }

        [Required]
        [StringLength(100)]
        [Index("IX_DiaDiem_TenDiaDiem", IsUnique = true)]
        public string TenDiaDiem { get; set; }

        public bool TrangThai { get; set; }

        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }

        public DiaDiem()
        {
            TinTuyenDungs = new HashSet<TinTuyenDung>();
        }
    }
}
