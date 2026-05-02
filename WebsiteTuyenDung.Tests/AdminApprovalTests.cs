using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class AdminApprovalTests : WebsitePageTest
{
    [TestMethod]
    public async Task QTV_01_DuyetTinChoDuyetThanhCong()
    {
        var jobTitle = await CreatePendingJobAsNewEmployerAsync();

        await Page.Context.ClearCookiesAsync();
        await LoginAsAdminAsync();
        await OpenAdminApprovalDetailsAsync(jobTitle);
        await Page.Locator("form[action$='/Duyet'] button[type='submit']").ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/KiemDuyetTinTuyenDung/Details"));
        await Expect(Page.Locator("body")).ToContainTextAsync("Đã duyệt");
    }

    [TestMethod]
    public async Task QTV_02_TuChoiTinChoDuyetKhiCoLyDo()
    {
        var jobTitle = await CreatePendingJobAsNewEmployerAsync();
        const string rejectReason = "Can bo sung mo ta cong viec ro hon.";

        await Page.Context.ClearCookiesAsync();
        await LoginAsAdminAsync();
        await OpenAdminApprovalDetailsAsync(jobTitle);
        await Page.Locator("form[action$='/TuChoi'] textarea[name='lyDoTuChoi']").FillAsync(rejectReason);
        await Page.Locator("form[action$='/TuChoi'] button[type='submit']").ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/KiemDuyetTinTuyenDung/Details"));
        await Expect(Page.Locator("body")).ToContainTextAsync("Bị từ chối");
        await Expect(Page.Locator("body")).ToContainTextAsync(rejectReason);
    }

    [TestMethod]
    public async Task QTV_03_TuChoiTinKhongNhapLyDo_ThiBaoLoi()
    {
        var jobTitle = await CreatePendingJobAsNewEmployerAsync();

        await Page.Context.ClearCookiesAsync();
        await LoginAsAdminAsync();
        await OpenAdminApprovalDetailsAsync(jobTitle);
        await Page.Locator("form[action$='/TuChoi'] button[type='submit']").ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/KiemDuyetTinTuyenDung/Details"));
        await Expect(Page.Locator("body")).ToContainTextAsync("Vui lòng nhập lý do từ chối");
    }

    private async Task OpenAdminApprovalDetailsAsync(string jobTitle)
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/KiemDuyetTinTuyenDung?trangThai=ChoDuyet");
        await Page.Locator($"tr:has-text(\"{jobTitle}\") a[href*='/KiemDuyetTinTuyenDung/Details']").First.ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync(jobTitle);
    }
}
