using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuyenDung.Models.Entities
{
    public class TinTuyenDung
    {
        [Key]
        public int TinTuyenDungId { get; set; }

        public int HoSoCongTyId { get; set; }
        public int NganhNgheId { get; set; }
        public int DiaDiemId { get; set; }
        public int LoaiHinhLamViecId { get; set; }
        public int CapDoKinhNghiemId { get; set; }

        public string TieuDe { get; set; }
        public string MoTaCongViec { get; set; }

        public DateTime NgayDang { get; set; }
        public DateTime HanNopHoSo { get; set; }

        [ForeignKey("HoSoCongTyId")]
        public virtual HoSoCongTy HoSoCongTy { get; set; }

        [ForeignKey("LoaiHinhLamViecId")]
        public virtual LoaiHinhLamViec LoaiHinhLamViec { get; set; }

        [ForeignKey("CapDoKinhNghiemId")]
        public virtual CapDoKinhNghiem CapDoKinhNghiem { get; set; }

    }
}