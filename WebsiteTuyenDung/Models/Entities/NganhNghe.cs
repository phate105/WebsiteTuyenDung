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
        [Index("IX_NganhNghe_TenNganhNghe", IsUnique = true)]
        public string TenNganhNghe { get; set; }

        public bool TrangThai { get; set; }

        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }

        public NganhNghe()
        {
            TinTuyenDungs = new HashSet<TinTuyenDung>();
        }
    }
}
