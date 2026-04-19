using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebsiteTuyenDung.Models;
using WebsiteTuyenDung.Models.Constants;
using WebsiteTuyenDung.Models.ViewModels;

namespace WebsiteTuyenDung.Controllers
{
    public class AccountController : Controller
    {
        private static readonly string[] PublicRegisterRoles =
        {
            ApplicationRoles.UngVien,
            ApplicationRoles.NhaTuyenDung
        };

        private static readonly DemoAccountInfo[] DemoAccounts =
        {
            new DemoAccountInfo("admin", "Admin", "admin@test.com", "123456Aa@", "Tài khoản quản trị hệ thống"),
            new DemoAccountInfo("employer", "Nhà tuyển dụng", "employer@test.com", "123456Aa@", "Tài khoản quản lý tin tuyển dụng"),
            new DemoAccountInfo("candidate", "Ứng viên", "ungvien@test.com", "123456Aa@", "Tài khoản ứng tuyển và theo dõi hồ sơ")
        };

        private UserManager<ApplicationUser> _userManager;

        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                if (_userManager != null)
                {
                    return _userManager;
                }

                _userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(new ApplicationDbContext()));

                _userManager.UserValidator = new UserValidator<ApplicationUser>(_userManager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };

                return _userManager;
            }
            private set => _userManager = value;
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            PopulateDemoAccounts();
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            PopulateDemoAccounts();

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var email = (model.Email ?? string.Empty).Trim();
            var user = await UserManager.FindByEmailAsync(email);

            if (user == null || !await UserManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            if (await UserManager.IsLockedOutAsync(user.Id))
            {
                ModelState.AddModelError(string.Empty, "Tài khoản đang bị khóa.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var identity = await UserManager.CreateIdentityAsync(
                user,
                DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            }, identity);

            return await RedirectToDefaultDestinationAsync(user, returnUrl);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterViewModel
            {
                VaiTro = "UngVien"
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!PublicRegisterRoles.Contains(model.VaiTro))
            {
                ModelState.AddModelError(nameof(model.VaiTro), "Vai trò đăng ký không hợp lệ.");
                return View(model);
            }

            var email = (model.Email ?? string.Empty).Trim();
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }

            await UserManager.AddToRoleAsync(user.Id, model.VaiTro);

            var identity = await UserManager.CreateIdentityAsync(
                user,
                DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, identity);

            return await RedirectToDefaultDestinationAsync(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userManager?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void PopulateDemoAccounts()
        {
            ViewBag.DemoAccounts = DemoAccounts;
        }

        private async Task<ActionResult> RedirectToDefaultDestinationAsync(ApplicationUser user, string returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (await UserManager.IsInRoleAsync(user.Id, ApplicationRoles.Admin))
            {
                return RedirectToAction("AdminDashboard", "Home");
            }

            if (await UserManager.IsInRoleAsync(user.Id, ApplicationRoles.NhaTuyenDung))
            {
                return RedirectToAction("EmployerDashboard", "Home");
            }

            return RedirectToAction("Index", "Home");
        }
    }

}
