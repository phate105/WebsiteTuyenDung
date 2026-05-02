using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class CandidateProfileTests : WebsitePageTest
{
    [TestMethod]
    public async Task HSUV_01_CapNhatHoSoCaNhanThanhCong()
    {
        var fullName = $"Ung vien Playwright {DateTime.UtcNow:HHmmssfff}";

        await RegisterCandidateAsync(TestConfig.NewEmail("profile"));
        await SaveCandidateProfileAsync(fullName);

        await Expect(Page).ToHaveURLAsync(new Regex(@"/UngVien/HoSo"));
        await Expect(Page.Locator("#HoTen")).ToHaveValueAsync(fullName);
        await Expect(Page.Locator("#SoDienThoai")).ToHaveValueAsync("0901234567");
    }

    [TestMethod]
    public async Task HSUV_02_BoTrongHoTen_ThiBaoLoi()
    {
        await RegisterCandidateAsync(TestConfig.NewEmail("profile"));
        await Page.GotoAsync($"{TestConfig.BaseUrl}/UngVien/HoSo");

        await Page.FillAsync("#HoTen", "");
        await Page.Locator("form[action*='HoSo'] button[type='submit']").ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/UngVien/HoSo"));
        await Expect(Page.Locator("[data-valmsg-for='HoTen'].field-validation-error")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task HSUV_03_NgaySinhTrongTuongLai_ThiBaoLoi()
    {
        await RegisterCandidateAsync(TestConfig.NewEmail("profile"));
        await Page.GotoAsync($"{TestConfig.BaseUrl}/UngVien/HoSo");

        await Page.FillAsync("#HoTen", "Ung vien ngay sinh sai");
        await Page.FillAsync("#NgaySinh", DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));
        await Page.Locator("form[action*='HoSo'] button[type='submit']").ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/UngVien/HoSo"));
        await Expect(Page.Locator("[data-valmsg-for='NgaySinh'].field-validation-error")).ToBeVisibleAsync();
    }
}
