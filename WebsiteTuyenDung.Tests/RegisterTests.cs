using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class RegisterTests : WebsitePageTest
{
    [TestMethod]
    public async Task DK_01_BoTrongThongTinDangKy_ThiBaoLoi()
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/Account/Register");

        await Page.ClickAsync("button[type='submit']");

        await Expect(Page).ToHaveURLAsync(new Regex(@"/Account/Register"));
        await Expect(Page.Locator("body")).ToContainTextAsync("Vui lòng nhập");
        await AssertNotAuthenticatedAsync();
    }

    [TestMethod]
    public async Task DK_02_XacNhanMatKhauKhongKhop_ThiBaoLoi()
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/Account/Register");

        await Page.SelectOptionAsync("#VaiTro", "UngVien");
        await Page.FillAsync("#Email", TestConfig.NewEmail("ungvien"));
        await Page.FillAsync("#Password", TestConfig.DemoPassword);
        await Page.FillAsync("#ConfirmPassword", "SaiMatKhau123");
        await Page.ClickAsync("button[type='submit']");

        await Expect(Page).ToHaveURLAsync(new Regex(@"/Account/Register"));
        await Expect(Page.Locator("body")).ToContainTextAsync("không khớp");
        await AssertNotAuthenticatedAsync();
    }

    [TestMethod]
    public async Task DK_03_DangKyUngVienThanhCong()
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/Account/Register");

        await Page.SelectOptionAsync("#VaiTro", "UngVien");
        await Page.FillAsync("#Email", TestConfig.NewEmail("ungvien"));
        await Page.FillAsync("#Password", TestConfig.DemoPassword);
        await Page.FillAsync("#ConfirmPassword", TestConfig.DemoPassword);
        await Page.ClickAsync("button[type='submit']");

        Assert.IsFalse(
            Page.Url.Contains("/Account/Register"),
            $"Dang ky thanh cong khong duoc o lai trang dang ky. URL hien tai: {Page.Url}");
        await AssertAuthenticatedAsync();
    }
}
