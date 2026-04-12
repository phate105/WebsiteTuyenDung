using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace   WebsiteTuyenDung.Models.Entities
{
    public class VaiTro
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TenVaiTro { get; set; } // Admin, Candidate, Employer

        // Navigation
        public virtual ICollection<NguoiDung> TaiKhoans { get; set; }

        public VaiTro()
        {
            TaiKhoans = new HashSet<NguoiDung>();
        }
    }
}