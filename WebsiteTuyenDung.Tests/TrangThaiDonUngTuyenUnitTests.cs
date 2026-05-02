using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebsiteTuyenDung.Models.Constants;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class TrangThaiDonUngTuyenUnitTests
{
    [TestMethod]
    public void DangXuLy_ChuaCacTrangThaiDangXuLy()
    {
        CollectionAssert.AreEquivalent(
            new[]
            {
                TrangThaiDonUngTuyen.DaNop,
                TrangThaiDonUngTuyen.DaXem,
                TrangThaiDonUngTuyen.VaoDanhSachNgan
            },
            TrangThaiDonUngTuyen.DangXuLy);
    }

    [TestMethod]
    public void EmployerCapNhatHopLe_KhongChoCapNhatVeDaRut()
    {
        CollectionAssert.DoesNotContain(
            TrangThaiDonUngTuyen.EmployerCapNhatHopLe,
            TrangThaiDonUngTuyen.DaRut);
    }

    [DataTestMethod]
    [DataRow(TrangThaiDonUngTuyen.DaNop, "Đã nộp")]
    [DataRow(TrangThaiDonUngTuyen.DaXem, "Đã xem")]
    [DataRow(TrangThaiDonUngTuyen.VaoDanhSachNgan, "Vào danh sách ngắn")]
    [DataRow(TrangThaiDonUngTuyen.BiTuChoi, "Bị từ chối")]
    [DataRow(TrangThaiDonUngTuyen.DuocChapNhan, "Được chấp nhận")]
    [DataRow(TrangThaiDonUngTuyen.DaRut, "Đã rút")]
    public void ToDisplayText_TrangThaiHopLe_TraVeDungText(string status, string expected)
    {
        var result = TrangThaiDonUngTuyen.ToDisplayText(status);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ToDisplayText_TrangThaiRong_TraVeChuaCapNhat()
    {
        var result = TrangThaiDonUngTuyen.ToDisplayText("");

        Assert.AreEqual("Chưa cập nhật", result);
    }

    [TestMethod]
    public void ToDisplayText_TrangThaiLaGiaTriLa_TraVeChinhGiaTriDo()
    {
        var result = TrangThaiDonUngTuyen.ToDisplayText("TrangThaiMoi");

        Assert.AreEqual("TrangThaiMoi", result);
    }
}
