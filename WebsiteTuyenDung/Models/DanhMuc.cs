using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTuyenDung.Models
{
    public class DanhMuc
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string TenDanhMuc { get; set; }

        public virtual ICollection<TinTuyenDung> TinTuyenDungs { get; set; }
    }
}