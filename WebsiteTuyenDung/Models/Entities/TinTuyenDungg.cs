using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteTuyenDung.Models.Enums;

namespace WebsiteTuyenDung.Models.Entities
{
    public class TinTuyenDungg
    {
        public int TinTuyenDungId { get; set; }

        public int HoSoCongTyId { get; set; }

        public string TieuDe { get; set; }

        public DateTime HanNopHoSo { get; set; }

        public TrangThaiTin TrangThaiTin { get; set; }

        public virtual HoSoCty HoSoCongTy { get; set; }
    }
}