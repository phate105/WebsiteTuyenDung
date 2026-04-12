using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class LichSuTrangThaiDon
    {
        [Key]
        public int LichSuTrangThaiDonId { get; set; }

        public int DonUngTuyenId { get; set; }
        public int NguoiDungId { get; set; }

        public string TrangThaiCu { get; set; }
        public string TrangThaiMoi { get; set; }
        public string GhiChu { get; set; }
        public DateTime ThoiGianThayDoi { get; set; }

        [ForeignKey("DonUngTuyenId")]
        public virtual DonUngTuyen DonUngTuyen { get; set; }

        [ForeignKey("NguoiDungId")]
        public virtual NguoiDung NguoiDung { get; set; }
    }
}