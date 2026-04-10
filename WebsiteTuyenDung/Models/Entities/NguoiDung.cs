using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteTuyenDung.Models.Entities
{
    public class NguoiDung
    {
        public int NguoiDungId { get; set; }
        public string TenDangNhap { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }

        public int VaiTroId { get; set; }
        public bool TrangThaiHoatDong { get; set; }

        public virtual VaiTro VaiTro { get; set; }

        public virtual HoSoCty HoSoCongTy { get; set; }
        public virtual HoSoCaNhan HoSoCaNhan { get; set; }
    }
}