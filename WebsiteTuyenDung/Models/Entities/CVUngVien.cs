using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteTuyenDung.Models.Entities
{
    public class CVUngVien
    {
        public int CVUngVienId { get; set; }
        public int HoSoCaNhanId { get; set; }

        public string TenCV { get; set; }
        public string DuongDanFile { get; set; }

        public DateTime NgayTaiLen { get; set; }

        public virtual HoSoCaNhan HoSoCaNhan { get; set; }
    }
}