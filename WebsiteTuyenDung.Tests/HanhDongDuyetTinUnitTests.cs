using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebsiteTuyenDung.Models.Constants;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class HanhDongDuyetTinUnitTests
{
    [DataTestMethod]
    [DataRow(HanhDongDuyetTin.GuiDuyet, "Nhà tuyển dụng gửi duyệt")]
    [DataRow(HanhDongDuyetTin.Duyet, "Admin duyệt tin")]
    [DataRow(HanhDongDuyetTin.TuChoi, "Admin từ chối tin")]
    [DataRow(HanhDongDuyetTin.DongTin, "Đóng tin")]
    public void ToDisplayText_HanhDongHopLe_TraVeDungText(string action, string expected)
    {
        var result = HanhDongDuyetTin.ToDisplayText(action);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ToDisplayText_HanhDongRong_TraVeCapNhatTrangThai()
    {
        var result = HanhDongDuyetTin.ToDisplayText("");

        Assert.AreEqual("Cập nhật trạng thái", result);
    }

    [TestMethod]
    public void ToDisplayText_HanhDongLaGiaTriLa_TraVeChinhGiaTriDo()
    {
        var result = HanhDongDuyetTin.ToDisplayText("XuLyKhac");

        Assert.AreEqual("XuLyKhac", result);
    }
}
