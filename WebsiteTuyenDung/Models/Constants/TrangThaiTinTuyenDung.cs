using System;

namespace WebsiteTuyenDung.Models.Constants
{
    public static class TrangThaiTinTuyenDung
    {
        public const string Nhap = "Nhap";
        public const string ChoDuyet = "ChoDuyet";
        public const string DaDuyet = "DaDuyet";
        public const string BiTuChoi = "BiTuChoi";
        public const string DaDong = "DaDong";
        public const string HetHan = "HetHan";

        public static readonly string[] EditableByEmployer = { Nhap, BiTuChoi };

        public static bool CoTheSuaBoiEmployer(string trangThaiTin)
        {
            return string.Equals(trangThaiTin, Nhap, StringComparison.OrdinalIgnoreCase)
                || string.Equals(trangThaiTin, BiTuChoi, StringComparison.OrdinalIgnoreCase);
        }

        public static bool CoTheGuiDuyet(string trangThaiTin)
        {
            return CoTheSuaBoiEmployer(trangThaiTin);
        }

        public static bool CoTheDong(string trangThaiTin)
        {
            return string.Equals(trangThaiTin, DaDuyet, StringComparison.OrdinalIgnoreCase);
        }

        public static string ToDisplayText(string trangThaiTin)
        {
            switch (trangThaiTin)
            {
                case Nhap:
                    return "Nháp";
                case ChoDuyet:
                    return "Chờ duyệt";
                case DaDuyet:
                    return "Đã duyệt";
                case BiTuChoi:
                    return "Bị từ chối";
                case DaDong:
                    return "Đã đóng";
                case HetHan:
                    return "Hết hạn";
                default:
                    return string.IsNullOrWhiteSpace(trangThaiTin) ? "Chưa cập nhật" : trangThaiTin;
            }
        }

        public static string ToLabelCss(string trangThaiTin)
        {
            switch (trangThaiTin)
            {
                case ChoDuyet:
                    return "label-warning";
                case DaDuyet:
                    return "label-success";
                case BiTuChoi:
                    return "label-danger";
                case DaDong:
                    return "label-primary";
                case HetHan:
                    return "label-default";
                default:
                    return "label-info";
            }
        }
    }
}
