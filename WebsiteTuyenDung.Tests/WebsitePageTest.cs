using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebsiteTuyenDung.Tests;

public abstract class WebsitePageTest : PageTest
{
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        };
    }

    protected Task LoginAsAdminAsync() =>
        LoginAsync(TestConfig.AdminEmail, TestConfig.DemoPassword);

    protected Task LoginAsEmployerAsync() =>
        LoginAsync(TestConfig.EmployerEmail, TestConfig.DemoPassword);

    protected Task LoginAsCandidateAsync() =>
        LoginAsync(TestConfig.CandidateEmail, TestConfig.DemoPassword);

    protected async Task LoginAsync(string email, string password)
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/Account/Login");
        await Page.FillAsync("#Email", email);
        await Page.FillAsync("#Password", password);
        await Page.ClickAsync("button[type='submit']");
    }

    protected Task RegisterCandidateAsync(string email) =>
        RegisterAsync(email, "UngVien");

    protected Task RegisterEmployerAsync(string email) =>
        RegisterAsync(email, "NhaTuyenDung");

    protected async Task RegisterAsync(string email, string role)
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/Account/Register");
        await Page.SelectOptionAsync("#VaiTro", role);
        await Page.FillAsync("#Email", email);
        await Page.FillAsync("#Password", TestConfig.DemoPassword);
        await Page.FillAsync("#ConfirmPassword", TestConfig.DemoPassword);
        await Page.ClickAsync("button[type='submit']");
        await AssertAuthenticatedAsync();
    }

    protected async Task AssertAuthenticatedAsync()
    {
        var cookies = await Page.Context.CookiesAsync(new[] { TestConfig.BaseUrl });

        Assert.IsTrue(
            cookies.Any(cookie => cookie.Name.Contains("ApplicationCookie")),
            "Dang nhap thanh cong phai tao authentication cookie.");
    }

    protected async Task AssertNotAuthenticatedAsync()
    {
        var cookies = await Page.Context.CookiesAsync(new[] { TestConfig.BaseUrl });

        Assert.IsFalse(
            cookies.Any(cookie => cookie.Name.Contains("ApplicationCookie")),
            "Dang nhap that bai khong duoc tao authentication cookie.");
    }

    protected async Task SelectFirstNonEmptyOptionAsync(string selector)
    {
        var value = await Page.Locator(selector).EvaluateAsync<string?>(
            "select => Array.from(select.options).map(option => option.value).find(value => value && value.trim().length > 0) || null");

        Assert.IsFalse(string.IsNullOrWhiteSpace(value), $"Khong tim thay option hop le cho selector: {selector}");

        await Page.SelectOptionAsync(selector, value);
    }

    protected async Task<string?> FirstHrefAsync(string selector)
    {
        return await Page.Locator(selector).First.EvaluateAsync<string?>(
            "element => element.getAttribute('href')");
    }

    protected string ToAbsoluteUrl(string href)
    {
        if (href.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            href.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return href;
        }

        return $"{TestConfig.BaseUrl}/{href.TrimStart('/')}";
    }

    protected async Task SaveCandidateProfileAsync(string fullName)
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/UngVien/HoSo");
        await Page.FillAsync("#HoTen", fullName);
        await Page.FillAsync("#NgaySinh", "1998-01-15");
        await Page.SelectOptionAsync("#GioiTinh", "Nam");
        await Page.FillAsync("#SoDienThoai", "0901234567");
        await Page.FillAsync("#DiaChi", "Ha Noi");
        await Page.FillAsync("#MucTieuNgheNghiep", "Tim cong viec phu hop va phat trien lau dai.");
        await Page.FillAsync("#HocVan", "Dai hoc Cong nghe thong tin.");
        await Page.FillAsync("#TomTatKinhNghiem", "Co kinh nghiem lam viec nhom va xay dung san pham web.");
        await Page.Locator("form[action*='HoSo'] button[type='submit']").ClickAsync();

        await Expect(Page.Locator("#HoTen")).ToHaveValueAsync(fullName);
    }

    protected async Task CreateCandidateCvAsync(string cvName)
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/UngVien/CV");
        await Page.Locator("form[action*='TaoCV'] input[name='TenCV']").FillAsync(cvName);
        await Page.Locator("form[action*='TaoCV'] input[name='ViTriMongMuon']").FillAsync("Nhan vien phan mem");
        await Page.Locator("form[action*='TaoCV'] textarea[name='KyNang']").FillAsync("C#\nASP.NET MVC\nSQL Server");
        await Page.Locator("form[action*='TaoCV'] textarea[name='KinhNghiem']").FillAsync("Da thuc hien cac bai tap lon ve website tuyen dung.");
        await Page.Locator("form[action*='TaoCV'] button[type='submit']").ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync(cvName);
    }

    protected async Task CreateEmployerProfileAsync(string companyName)
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/HoSoCongTy/Create");
        await Page.FillAsync("#TenCongTy", companyName);
        await Page.FillAsync("#MaSoThue", "0101234567");
        await Page.FillAsync("#EmailLienHe", "hr@example.com");
        await Page.FillAsync("#SoDienThoaiLienHe", "0901234567");
        await Page.FillAsync("#DiaChi", "Ha Noi");
        await Page.FillAsync("#Website", "example.com");
        await Page.FillAsync("#MoTa", "Cong ty test dung cho Playwright.");
        await Page.Locator("form[action*='HoSoCongTy'] button[type='submit']").ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync(companyName);
    }

    protected async Task CreateJobDraftAsync(string title)
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/TinTuyenDung/Create");
        await Page.FillAsync("#TieuDe", title);
        await SelectFirstNonEmptyOptionAsync("#NganhNgheId");
        await SelectFirstNonEmptyOptionAsync("#DiaDiemId");
        await SelectFirstNonEmptyOptionAsync("#LoaiHinhLamViecId");
        await SelectFirstNonEmptyOptionAsync("#CapDoKinhNghiemId");
        await Page.FillAsync("#MoTaCongViec", "Mo ta cong viec test duoc tao boi Playwright.");
        await Page.FillAsync("#YeuCau", "Ung vien co tinh than hoc hoi.");
        await Page.FillAsync("#QuyenLoi", "Moi truong lam viec than thien.");
        await Page.FillAsync("#SoLuongTuyen", "2");
        await Page.FillAsync("#LuongToiThieu", "10000000");
        await Page.FillAsync("#LuongToiDa", "20000000");
        await Page.FillAsync("#HanNopHoSo", DateTime.Today.AddDays(30).ToString("yyyy-MM-dd"));
        await Page.Locator("form[action*='TinTuyenDung'] button[type='submit']").ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync(title);
    }

    protected async Task OpenEmployerJobDetailsAsync(string title)
    {
        await Page.Locator($"tr:has-text(\"{title}\") a[href*='/TinTuyenDung/Details']").First.ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync(title);
    }

    protected async Task<string> CreatePendingJobAsNewEmployerAsync()
    {
        var companyName = $"Cong ty Playwright {DateTime.UtcNow:HHmmssfff}";
        var jobTitle = $"Tin tuyen dung Playwright {DateTime.UtcNow:yyyyMMddHHmmssfff}";

        await RegisterEmployerAsync(TestConfig.NewEmail("employer"));
        await CreateEmployerProfileAsync(companyName);
        await CreateJobDraftAsync(jobTitle);
        await OpenEmployerJobDetailsAsync(jobTitle);
        await Page.Locator("form[action*='GuiDuyet'] button[type='submit']").ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("Chờ duyệt");

        return jobTitle;
    }

    protected async Task OpenFirstPublicJobDetailsAsync()
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/Job");
        var href = await FirstHrefAsync("a.job-detail-action");

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(href),
            "Can co it nhat mot tin tuyen dung da duyet de chay test ung tuyen.");

        await Page.GotoAsync(ToAbsoluteUrl(href!));
    }
}
