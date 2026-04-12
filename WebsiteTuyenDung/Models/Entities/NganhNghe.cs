using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class NganhNghe
    {
        [Key]
        public int NganhNgheId { get; set; }

        [Required]
        [StringLength(100)]
        public string TenNganhNghe { get; set; }

        public bool TrangThai { get; set; }

        // Navigation
        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }

        public NganhNghe()
        {
            TinTuyenDungs = new HashSet<TinTuyenDung>();
        }
    }
}