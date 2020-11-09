using System.Threading.Tasks;
using Velusia.Server.Models;
using Velusia.Server.Services;
using Velusia.Server.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Velusia.Server.ViewModels.Shared;

namespace Velusia.Server.Controllers
{
    public class AccountController : Controller
    {
        private const string DummyPassword = "6d2cfde64418|49b2-a355@a873623f66f3!950E79DF-90ED43E1A9A5#D4EB8DF93336"; // JK
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _applicationDbContext;
        private static bool _databaseChecked;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationDbContext = applicationDbContext;
        }

        //
        // GET: /Account/LinkSent
        [HttpGet]
        public IActionResult LinkSent()
        {
            return View();
        }

        //
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            EnsureDatabaseCreated(_applicationDbContext);
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    // Setting two factor as enabled for all new users /JK
                    var newUser = new ApplicationUser { UserName = model.Email, Email = model.Email, EmailConfirmed = false };
                    var result = await _userManager.CreateAsync(newUser, DummyPassword);

                    if (!result.Succeeded)
                    {
                        AddErrors(result);
                    }
                    else
                    {
                        user = await _userManager.FindByEmailAsync(model.Email);
                    }
                }

                if (user == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        Error = "Error creating user.",
                    });
                }

                var token = await _userManager.GenerateUserTokenAsync(user, "Default", "passwordless-auth");
                var linkForEmail = Url.Action("ConfirmEmail", "Account", new { userid = user.Id, code = token, returnurl = returnUrl }, this.Request.Scheme);

                await _emailSender.SendLinkEmailAsync(
                    model.Email,
                    "Sign-in link",
                    linkForEmail);

                return RedirectToAction("LinkSent");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, string returnUrl = null)
        {
            if (userId == null || code == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Error = "Missing parameters.",
                });
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Error = "No user found.",
                });
            }

            var isValid = await _userManager.VerifyUserTokenAsync(user, "Default", "passwordless-auth", code);

            if (isValid)
            {
                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    var resultUpdatingEmailConfirmed = await _userManager.UpdateAsync(user);

                    if (!resultUpdatingEmailConfirmed.Succeeded)
                    {
                        return View("Error", new ErrorViewModel
                        {
                            Error = "Error updating confirmed e-mail for user.",
                        });
                    }
                }

                await _userManager.UpdateSecurityStampAsync(user);

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(user.Email, DummyPassword, true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return View("Error", new ErrorViewModel
                    {
                        Error = "This account has been locked out, please try again later.",
                    });
                }
            }

            return View("Error", new ErrorViewModel
            {
                Error = "Error with signin link.",
            });
        }

        // The following code creates the database and schema if they don't exist.
        // This is a temporary workaround since deploying database through EF migrations is
        // not yet supported in this release.
        // Please see this http://go.microsoft.com/fwlink/?LinkID=615859 for more information on how to do deploy the database
        // when publishing your application.
        private static void EnsureDatabaseCreated(ApplicationDbContext context)
        {
            if (!_databaseChecked)
            {
                _databaseChecked = true;
                context.Database.EnsureCreated();
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
