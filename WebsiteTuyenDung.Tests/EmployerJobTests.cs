using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class EmployerJobTests : WebsitePageTest
{
    [TestMethod]
    public async Task NTD_01_BoTrongTenCongTy_ThiBaoLoi()
    {
        await RegisterEmployerAsync(TestConfig.NewEmail("employer"));
        await Page.GotoAsync($"{TestConfig.BaseUrl}/HoSoCongTy/Create");

        await Page.Locator("form[action*='HoSoCongTy'] button[type='submit']").ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/HoSoCongTy/Create"));
        await Expect(Page.Locator("body")).ToContainTextAsync("Vui lòng nhập");
    }

    [TestMethod]
    public async Task NTD_02_TaoHoSoCongTyThanhCong()
    {
        var companyName = $"Cong ty Playwright {DateTime.UtcNow:HHmmssfff}";

        await RegisterEmployerAsync(TestConfig.NewEmail("employer"));
        await CreateEmployerProfileAsync(companyName);

        await Expect(Page).ToHaveURLAsync(new Regex(@"/HoSoCongTy"));
        await Expect(Page.Locator("body")).ToContainTextAsync(companyName);
    }

    [TestMethod]
    public async Task NTD_03_TaoTinTuyenDungVaGuiDuyetThanhCong()
    {
        var companyName = $"Cong ty Playwright {DateTime.UtcNow:HHmmssfff}";
        var jobTitle = $"Tin tuyen dung Playwright {DateTime.UtcNow:yyyyMMddHHmmssfff}";

        await RegisterEmployerAsync(TestConfig.NewEmail("employer"));
        await CreateEmployerProfileAsync(companyName);
        await CreateJobDraftAsync(jobTitle);
        await OpenEmployerJobDetailsAsync(jobTitle);
        await Page.Locator("form[action*='GuiDuyet'] button[type='submit']").ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/TinTuyenDung/Details"));
        await Expect(Page.Locator("body")).ToContainTextAsync("Chờ duyệt");
    }
}
