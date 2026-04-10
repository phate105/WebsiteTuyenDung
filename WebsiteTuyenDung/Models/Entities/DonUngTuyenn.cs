using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteTuyenDung.Models.Enums;

namespace WebsiteTuyenDung.Models.Entities
{
    public class DonUngTuyenn
    {
        public int DonUngTuyenId { get; set; }

        public int TinTuyenDungId { get; set; }
        public int HoSoCaNhanId { get; set; }

        public DateTime NgayNop { get; set; }

        public TrangThaiDon TrangThaiDon { get; set; }

        public virtual TinTuyenDung TinTuyenDung { get; set; }
        public virtual HoSoCaNhan HoSoCaNhan { get; set; }
    }
}