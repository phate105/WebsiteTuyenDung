namespace WebsiteTuyenDung.Tests;

internal static class TestConfig
{
    public const string AdminEmail = "admin@test.com";
    public const string EmployerEmail = "employer@test.com";
    public const string CandidateEmail = "ungvien@test.com";
    public const string DemoPassword = "123456Aa@";

    public static string BaseUrl =>
        (Environment.GetEnvironmentVariable("BASE_URL") ?? "https://localhost:44359").TrimEnd('/');

    public static string NewEmail(string prefix) =>
        $"{prefix}_{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}@test.com";
}
