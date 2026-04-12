using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTuyenDung.Models.Entities
{
    public class DiaDiem
    {
        [Key]
        public int DiaDiemId { get; set; }

        [Required]
        [StringLength(100)]
        public string TenDiaDiem { get; set; }

        public bool TrangThai { get; set; }

        // Navigation
        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }

        public DiaDiem()
        {
            TinTuyenDungs = new HashSet<TinTuyenDung>();
        }
    }
}