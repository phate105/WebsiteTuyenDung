using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class LoginTests : WebsitePageTest
{
    private const string WrongPassword = "saimatkhau";

    [TestMethod]
    public async Task DN_CN_01_DangNhapUngVienThanhCong()
    {
        await LoginAsCandidateAsync();

        Assert.IsFalse(
            Page.Url.Contains("/Account/Login"),
            $"Dang nhap thanh cong khong duoc o lai trang login. URL hien tai: {Page.Url}");
        await AssertAuthenticatedAsync();
    }

    [TestMethod]
    public async Task DN_CN_02_DangNhapNhaTuyenDungThanhCong()
    {
        await LoginAsEmployerAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/Home/EmployerDashboard/?$"));
        await AssertAuthenticatedAsync();
    }

    [TestMethod]
    public async Task DN_CN_03_DangNhapQuanTriVienThanhCong()
    {
        await LoginAsAdminAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/Home/AdminDashboard/?$"));
        await AssertAuthenticatedAsync();
    }

    [TestMethod]
    public async Task DN_CN_04_DangNhapSaiMatKhau_ThiBaoLoi()
    {
        await LoginAsync(TestConfig.CandidateEmail, WrongPassword);

        await Expect(Page).ToHaveURLAsync(new Regex(@"/Account/Login"));
        await Expect(Page.Locator("body")).ToContainTextAsync("Email hoặc mật khẩu không đúng");
        await AssertNotAuthenticatedAsync();
    }

    [TestMethod]
    public async Task DN_CN_05_BoTrongThongTinDangNhap_ThiBaoLoi()
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/Account/Login");

        await Page.ClickAsync("button[type='submit']");

        await Expect(Page).ToHaveURLAsync(new Regex(@"/Account/Login"));
        await Expect(Page.Locator("body")).ToContainTextAsync("Vui lòng nhập");
        await AssertNotAuthenticatedAsync();
    }
}
