using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class HoSoCaNhan
    {
        [Key, ForeignKey("NguoiDung")]
        public int NguoiDungId { get; set; }

        [Required]
        public string HoTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string GioiTinh { get; set; }

        [StringLength(15)]
        public string SoDienThoai { get; set; }

        public string DiaChi { get; set; }

        public string MucTieuNgheNghiep { get; set; }

        public string HocVan { get; set; }

        public string TomTatKinhNghiem { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public virtual NguoiDung NguoiDung { get; set; }

        public virtual ICollection<CVUngVien> CVUngViens { get; set; }

        public HoSoCaNhan()
        {
            CVUngViens = new HashSet<CVUngVien>();
        }
    }
}