using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebsiteTuyenDung.Models.Constants;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class TrangThaiTinTuyenDungUnitTests
{
    [TestMethod]
    public void CoTheSuaBoiEmployer_TinNhap_TraVeTrue()
    {
        var result = TrangThaiTinTuyenDung.CoTheSuaBoiEmployer(TrangThaiTinTuyenDung.Nhap);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CoTheSuaBoiEmployer_TinBiTuChoi_TraVeTrue()
    {
        var result = TrangThaiTinTuyenDung.CoTheSuaBoiEmployer(TrangThaiTinTuyenDung.BiTuChoi);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CoTheSuaBoiEmployer_TinDaDuyet_TraVeFalse()
    {
        var result = TrangThaiTinTuyenDung.CoTheSuaBoiEmployer(TrangThaiTinTuyenDung.DaDuyet);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CoTheGuiDuyet_TinNhap_TraVeTrue()
    {
        var result = TrangThaiTinTuyenDung.CoTheGuiDuyet(TrangThaiTinTuyenDung.Nhap);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CoTheDong_TinDaDuyet_TraVeTrue()
    {
        var result = TrangThaiTinTuyenDung.CoTheDong(TrangThaiTinTuyenDung.DaDuyet);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CoTheDong_TinChoDuyet_TraVeFalse()
    {
        var result = TrangThaiTinTuyenDung.CoTheDong(TrangThaiTinTuyenDung.ChoDuyet);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow(TrangThaiTinTuyenDung.Nhap, "Nháp")]
    [DataRow(TrangThaiTinTuyenDung.ChoDuyet, "Chờ duyệt")]
    [DataRow(TrangThaiTinTuyenDung.DaDuyet, "Đã duyệt")]
    [DataRow(TrangThaiTinTuyenDung.BiTuChoi, "Bị từ chối")]
    [DataRow(TrangThaiTinTuyenDung.DaDong, "Đã đóng")]
    [DataRow(TrangThaiTinTuyenDung.HetHan, "Hết hạn")]
    public void ToDisplayText_TrangThaiHopLe_TraVeDungText(string status, string expected)
    {
        var result = TrangThaiTinTuyenDung.ToDisplayText(status);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ToDisplayText_TrangThaiRong_TraVeChuaCapNhat()
    {
        var result = TrangThaiTinTuyenDung.ToDisplayText("");

        Assert.AreEqual("Chưa cập nhật", result);
    }

    [DataTestMethod]
    [DataRow(TrangThaiTinTuyenDung.ChoDuyet, "label-warning")]
    [DataRow(TrangThaiTinTuyenDung.DaDuyet, "label-success")]
    [DataRow(TrangThaiTinTuyenDung.BiTuChoi, "label-danger")]
    [DataRow(TrangThaiTinTuyenDung.DaDong, "label-primary")]
    [DataRow(TrangThaiTinTuyenDung.HetHan, "label-default")]
    [DataRow(TrangThaiTinTuyenDung.Nhap, "label-info")]
    public void ToLabelCss_TrangThaiHopLe_TraVeDungClass(string status, string expected)
    {
        var result = TrangThaiTinTuyenDung.ToLabelCss(status);

        Assert.AreEqual(expected, result);
    }
}
