using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class ApplyJobTests : WebsitePageTest
{
    [TestMethod]
    public async Task UT_01_ChuaDangNhap_ThiYeuCauDangNhap()
    {
        await OpenFirstPublicJobDetailsAsync();

        await Expect(Page.Locator("#apply-box a[href*='/Account/Login']")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task UT_02_UngVienChuaCoHoSo_ThiDuocNhacTaoHoSo()
    {
        await RegisterCandidateAsync(TestConfig.NewEmail("apply"));

        await OpenFirstPublicJobDetailsAsync();

        await Expect(Page.Locator("#apply-box a[href*='/UngVien/HoSo']")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task UT_03_UngVienCoHoSoVaCV_NopHoSoThanhCong()
    {
        var cvName = $"CV ung tuyen {DateTime.UtcNow:HHmmssfff}";

        await RegisterCandidateAsync(TestConfig.NewEmail("apply"));
        await SaveCandidateProfileAsync("Ung vien ung tuyen");
        await CreateCandidateCvAsync(cvName);

        await OpenFirstPublicJobDetailsAsync();
        await SelectFirstNonEmptyOptionAsync("select[name='ApplyForm.CVUngVienId']");
        await Page.FillAsync("textarea[name='ApplyForm.ThuGioiThieu']", "Toi mong muon ung tuyen vi tri nay.");
        await Page.Locator("form[action*='Apply'] button[type='submit']").ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/UngVien/ChiTietDon"));
        await Expect(Page.Locator("body")).ToContainTextAsync(cvName);
    }
}
