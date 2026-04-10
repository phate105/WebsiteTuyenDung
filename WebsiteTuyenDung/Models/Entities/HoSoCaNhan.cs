using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteTuyenDung.Models.Entities
{
    public class HoSoCaNhan
    {
        public int HoSoCaNhanId { get; set; }
        public int NguoiDungId { get; set; }

        public string HoTen { get; set; }

        public virtual NguoiDung NguoiDung { get; set; }
        public virtual ICollection<CVUngVien> CVUngViens { get; set; }
    }
}