using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using WebsiteTuyenDung.Models.ViewModels;

namespace WebsiteTuyenDung.Tests;

[TestClass]
public class ViewModelValidationUnitTests
{
    [TestMethod]
    public void LoginViewModel_HopLe_ThiKhongCoLoiValidation()
    {
        var model = new LoginViewModel
        {
            Email = "ungvien@test.com",
            Password = "123456Aa@"
        };

        var results = Validate(model);

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void LoginViewModel_ThieuEmail_ThiCoLoiValidation()
    {
        var model = new LoginViewModel
        {
            Email = "",
            Password = "123456Aa@"
        };

        var results = Validate(model);

        AssertHasMemberError(results, nameof(LoginViewModel.Email));
    }

    [TestMethod]
    public void LoginViewModel_EmailSaiDinhDang_ThiCoLoiValidation()
    {
        var model = new LoginViewModel
        {
            Email = "email-sai",
            Password = "123456Aa@"
        };

        var results = Validate(model);

        AssertHasMemberError(results, nameof(LoginViewModel.Email));
    }

    [TestMethod]
    public void LoginViewModel_ThieuMatKhau_ThiCoLoiValidation()
    {
        var model = new LoginViewModel
        {
            Email = "ungvien@test.com",
            Password = ""
        };

        var results = Validate(model);

        AssertHasMemberError(results, nameof(LoginViewModel.Password));
    }

    [TestMethod]
    public void RegisterViewModel_HopLe_ThiKhongCoLoiValidation()
    {
        var model = new RegisterViewModel
        {
            VaiTro = "UngVien",
            Email = "ungvien_moi@test.com",
            Password = "123456Aa@",
            ConfirmPassword = "123456Aa@"
        };

        var results = Validate(model);

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void RegisterViewModel_ThieuVaiTro_ThiCoLoiValidation()
    {
        var model = new RegisterViewModel
        {
            VaiTro = "",
            Email = "ungvien_moi@test.com",
            Password = "123456Aa@",
            ConfirmPassword = "123456Aa@"
        };

        var results = Validate(model);

        AssertHasMemberError(results, nameof(RegisterViewModel.VaiTro));
    }

    [TestMethod]
    public void RegisterViewModel_MatKhauQuaNgan_ThiCoLoiValidation()
    {
        var model = new RegisterViewModel
        {
            VaiTro = "UngVien",
            Email = "ungvien_moi@test.com",
            Password = "123",
            ConfirmPassword = "123"
        };

        var results = Validate(model);

        AssertHasMemberError(results, nameof(RegisterViewModel.Password));
    }

    [TestMethod]
    public void RegisterViewModel_XacNhanMatKhauKhongKhop_ThiCoLoiValidation()
    {
        var model = new RegisterViewModel
        {
            VaiTro = "UngVien",
            Email = "ungvien_moi@test.com",
            Password = "123456Aa@",
            ConfirmPassword = "SaiMatKhau123"
        };

        var results = Validate(model);

        AssertHasMemberError(results, nameof(RegisterViewModel.ConfirmPassword));
    }

    [TestMethod]
    public void RegisterViewModel_EmailSaiDinhDang_ThiCoLoiValidation()
    {
        var model = new RegisterViewModel
        {
            VaiTro = "UngVien",
            Email = "email-sai",
            Password = "123456Aa@",
            ConfirmPassword = "123456Aa@"
        };

        var results = Validate(model);

        AssertHasMemberError(results, nameof(RegisterViewModel.Email));
    }

    private static List<ValidationResult> Validate(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, validateAllProperties: true);

        return results;
    }

    private static void AssertHasMemberError(IEnumerable<ValidationResult> results, string memberName)
    {
        Assert.IsTrue(
            results.Any(result => result.MemberNames.Contains(memberName)),
            $"Expected validation error for member '{memberName}'.");
    }
}
