using System;

namespace WebsiteTuyenDung.Models.Constants
{
    public static class HanhDongDuyetTin
    {
        public const string GuiDuyet = "GuiDuyet";
        public const string Duyet = "Duyet";
        public const string TuChoi = "TuChoi";
        public const string DongTin = "DongTin";

        public static string ToDisplayText(string hanhDong)
        {
            switch (hanhDong)
            {
                case GuiDuyet:
                    return "Nhà tuyển dụng gửi duyệt";
                case Duyet:
                    return "Admin duyệt tin";
                case TuChoi:
                    return "Admin từ chối tin";
                case DongTin:
                    return "Đóng tin";
                default:
                    return string.IsNullOrWhiteSpace(hanhDong) ? "Cập nhật trạng thái" : hanhDong;
            }
        }
    }
}
