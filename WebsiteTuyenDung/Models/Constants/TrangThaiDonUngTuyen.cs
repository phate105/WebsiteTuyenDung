using System;

namespace WebsiteTuyenDung.Models.Constants
{
    public static class TrangThaiDonUngTuyen
    {
        public const string DaNop = "DaNop";
        public const string DaXem = "DaXem";
        public const string VaoDanhSachNgan = "VaoDanhSachNgan";
        public const string BiTuChoi = "BiTuChoi";
        public const string DuocChapNhan = "DuocChapNhan";
        public const string DaRut = "DaRut";

        public static readonly string[] EmployerCapNhatHopLe =
        {
            DaNop,
            DaXem,
            VaoDanhSachNgan,
            BiTuChoi,
            DuocChapNhan
        };

        public static readonly string[] DangXuLy =
        {
            DaNop,
            DaXem,
            VaoDanhSachNgan
        };

        public static string ToDisplayText(string trangThaiDon)
        {
            switch (trangThaiDon)
            {
                case DaNop:
                    return "Đã nộp";
                case DaXem:
                    return "Đã xem";
                case VaoDanhSachNgan:
                    return "Vào danh sách ngắn";
                case BiTuChoi:
                    return "Bị từ chối";
                case DuocChapNhan:
                    return "Được chấp nhận";
                case DaRut:
                    return "Đã rút";
                default:
                    return string.IsNullOrWhiteSpace(trangThaiDon) ? "Chưa cập nhật" : trangThaiDon;
            }
        }
    }
}
