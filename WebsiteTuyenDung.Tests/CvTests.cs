using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class CvTests : WebsitePageTest
{
    [TestMethod]
    public async Task CV_01_ChuaCoHoSo_ThiNhacCapNhatHoSo()
    {
        await RegisterCandidateAsync(TestConfig.NewEmail("cv"));

        await Page.GotoAsync($"{TestConfig.BaseUrl}/UngVien/CV");

        await Expect(Page).ToHaveURLAsync(new Regex(@"/UngVien/CV"));
        await Expect(Page.Locator("a.btn-clean-dark[href*='/UngVien/HoSo']").First).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CV_02_TaoCVNhanhThanhCong()
    {
        var cvName = $"CV Playwright {DateTime.UtcNow:HHmmssfff}";

        await RegisterCandidateAsync(TestConfig.NewEmail("cv"));
        await SaveCandidateProfileAsync("Ung vien tao CV");
        await CreateCandidateCvAsync(cvName);

        await Expect(Page).ToHaveURLAsync(new Regex(@"/UngVien/CV"));
        await Expect(Page.Locator("body")).ToContainTextAsync(cvName);
    }

    [TestMethod]
    public async Task CV_03_TaiLenCVKhongChonFile_ThiBaoLoi()
    {
        await RegisterCandidateAsync(TestConfig.NewEmail("cv"));
        await SaveCandidateProfileAsync("Ung vien upload CV");
        await Page.GotoAsync($"{TestConfig.BaseUrl}/UngVien/CV");

        await Page.Locator("form[action*='TaiLenCV'] button[type='submit']").ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/UngVien/CV"));
        await Expect(Page.Locator("body")).ToContainTextAsync("Vui lòng chọn tệp CV");
    }
}
