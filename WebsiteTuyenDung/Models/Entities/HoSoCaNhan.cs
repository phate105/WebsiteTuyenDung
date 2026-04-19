using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebsiteTuyenDung.Models;

namespace WebsiteTuyenDung.Models.Entities
{
    public class HoSoCaNhan
    {
        [Key]
        public int HoSoCaNhanId { get; set; }

        [Required]
        [StringLength(128)]
        [Index("IX_HoSoCaNhan_ApplicationUserId", IsUnique = true)]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [StringLength(150)]
        public string HoTen { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(20)]
        public string GioiTinh { get; set; }

        [StringLength(20)]
        public string SoDienThoai { get; set; }

        [StringLength(300)]
        public string DiaChi { get; set; }

        public string MucTieuNgheNghiep { get; set; }
        public string HocVan { get; set; }
        public string TomTatKinhNghiem { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public virtual ICollection<CVUngVien> CVUngViens { get; set; }
        public virtual ICollection<DonUngTuyen> DonUngTuyens { get; set; }

        public HoSoCaNhan()
        {
            CVUngViens = new HashSet<CVUngVien>();
            DonUngTuyens = new HashSet<DonUngTuyen>();
        }
    }
}
