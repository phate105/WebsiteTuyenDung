using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class NhatKyDuyetTin
    {
        [Key]
        public int NhatKyDuyetTinId { get; set; }

        public int TinTuyenDungId { get; set; }
        public int NguoiDungId { get; set; }

        public string HanhDong { get; set; }
        public string LyDoTuChoi { get; set; }
        public DateTime ThoiGianXuLy { get; set; }

        [ForeignKey("TinTuyenDungId")]
        public virtual TinTuyenDung TinTuyenDung { get; set; }

        [ForeignKey("NguoiDungId")]
        public virtual NguoiDung NguoiDung { get; set; }
    }
}