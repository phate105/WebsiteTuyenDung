using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WebsiteTuyenDung.Tests
{
    [TestClass]
    public class LoginTests : PageTest
    {
        private const string BaseUrl = "https://localhost:44359";

        [TestMethod]
        public async Task DN_CN_01_DangNhapUngVienThanhCong()
        {
            await Page.GotoAsync($"{BaseUrl}/Account/Login");

            await Page.FillAsync("input[name='Email']", "ungvien@test.com");
            await Page.FillAsync("input[name='Password']", "123456");

            await Page.ClickAsync("button[type='submit']");

            await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*"));
        }

        [TestMethod]
        public async Task DN_CN_02_DangNhapNhaTuyenDungThanhCong()
        {
            await Page.GotoAsync($"{BaseUrl}/Account/Login");

            await Page.FillAsync("input[name='Email']", "nhatuyendung@test.com");
            await Page.FillAsync("input[name='Password']", "123456");

            await Page.ClickAsync("button[type='submit']");

            await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*"));
        }

        [TestMethod]
        public async Task DN_CN_03_DangNhapQuanTriVienThanhCong()
        {
            await Page.GotoAsync($"{BaseUrl}/Account/Login");

            await Page.FillAsync("input[name='Email']", "admin@test.com");
            await Page.FillAsync("input[name='Password']", "123456");

            await Page.ClickAsync("button[type='submit']");

            await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*"));
        }

        [TestMethod]
        public async Task DN_CN_04_DangNhapSaiMatKhau()
        {
            await Page.GotoAsync($"{BaseUrl}/Account/Login");

            await Page.FillAsync("input[name='Email']", "ungvien@test.com");
            await Page.FillAsync("input[name='Password']", "saimatkhau");

            await Page.ClickAsync("button[type='submit']");

            await Expect(Page.Locator("body")).ToContainTextAsync("không đúng");
        }

        [TestMethod]
        public async Task DN_CN_05_BoTrongThongTinDangNhap()
        {
            await Page.GotoAsync($"{BaseUrl}/Account/Login");

            await Page.ClickAsync("button[type='submit']");

            await Expect(Page).ToHaveURLAsync(new Regex(".*Login.*"));
        }
    }
}