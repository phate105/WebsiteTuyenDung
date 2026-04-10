using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace WebsiteTuyenDung.Models.Entities
{
    public class HoSoCty
    {
        [Key]
        public int HoSoCongTyId { get; set; }
        public int NguoiDungId { get; set; }
        public string TenCongTy { get; set; }
        public string DiaChi { get; set; }

        public virtual NguoiDung NguoiDung { get; set; }
        public virtual ICollection<Models.TinTuyenDung> TinTuyenDungs { get; set; }
    }
}