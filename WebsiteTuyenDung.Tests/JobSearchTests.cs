using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class JobSearchTests : WebsitePageTest
{
    [TestMethod]
    public async Task TK_01_TimKiemViecLamTheoTuKhoa()
    {
        const string keyword = "Developer";

        await Page.GotoAsync($"{TestConfig.BaseUrl}/Job/Search");
        await Page.FillAsync("input[name='keyword']", keyword);
        await Page.ClickAsync("button[type='submit']");

        await Expect(Page).ToHaveURLAsync(new Regex(@"/Job.*keyword=Developer"));
        await Expect(Page.Locator("input[name='keyword']")).ToHaveValueAsync(keyword);
    }

    [TestMethod]
    public async Task TK_02_TimKiemDiaDiemKhongKhop_ThiGopVaoTuKhoa()
    {
        const string location = "DiaDiemKhongTonTai";

        await Page.GotoAsync($"{TestConfig.BaseUrl}/Job/Search?location={location}");

        await Expect(Page).ToHaveURLAsync(new Regex(@"/Job.*keyword=DiaDiemKhongTonTai"));
        await Expect(Page.Locator("input[name='keyword']")).ToHaveValueAsync(location);
    }

    [TestMethod]
    public async Task TK_03_LocViecLamTheoNganhNghe()
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/Job");

        await SelectFirstNonEmptyOptionAsync("select[name='nganhNgheId']");
        await Page.ClickAsync("button[type='submit']");

        await Expect(Page).ToHaveURLAsync(new Regex(@"/Job.*nganhNgheId=\d+"));
    }
}
