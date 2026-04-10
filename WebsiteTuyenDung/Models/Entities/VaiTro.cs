using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteTuyenDung.Models.Entities
{
    public class VaiTro
    {
        public int VaiTroId { get; set; }
        public string TenVaiTro { get; set; }

        public virtual ICollection<NguoiDung> NguoiDungs { get; set; }
    }
}