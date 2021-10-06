using Microsoft.AspNetCore.Identity;
using System;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using csharp_lend_pc.Models;
using Microsoft.EntityFrameworkCore;
using csharp_lend_pc.Db;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;

namespace csharp_lend_pc.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly AppDbContext _context;
        public UserController(AppDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        static readonly SHA256CryptoServiceProvider hashProvider = new SHA256CryptoServiceProvider();
        public static string GetSHA256HashedString(string value)
        => string.Join("", hashProvider.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(x => $"{x:x2}"));

        // public async Task<IActionResult> Index()
        // {
        //     // using Microsoft.EntityFrameworkCore;
        //     var users = await _userManager.Users.
        //                       OrderBy(user => user.UserName).ToListAsync();
        //     return View(users);
        // }

        // GET: Account/Login/ReturnUrl
        // Model は Models/UserViewModel.cs の LoginViewModel
        [AllowAnonymous]
        [Route("/login")]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/login")]
        public async Task<IActionResult> Login(string returnUrl,
            LoginViewModel model)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            const string badUserNameOrPasswordMessage = "Username or password is incorrect.";

            string hashedPassword = GetSHA256HashedString(model.Password);

            UserEntity result = await _context.users
            .Where(u => u.UserName == model.UserName && u.PasswordHash == hashedPassword)
            .FirstOrDefaultAsync();

            if (result == null)
            {
                return BadRequest(badUserNameOrPasswordMessage);
            }
            else
            {                
                var identity = new ClaimsIdentity("MyCookieAuthenticationScheme");
                identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));                
                // クッキー認証スキームと、上の数行で作成されたIDから作成された新しい ClaimsPrincipal を渡します。
                await HttpContext.SignInAsync("MyCookieAuthenticationScheme", new ClaimsPrincipal(identity));
                return LocalRedirect(returnUrl);
            }
        }

        // POST: Account/Logout
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuthenticationScheme");

            return RedirectToAction("Login", "User");
        }

        // GET: Account/Register
        // Model は Models/UserViewModel.cs の RegisterViewModel
        [AllowAnonymous]
        [HttpGet]
        [Route("/register")]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/register")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            const string badUserNameOrPasswordMessage = "Username or password is incorrect.";

            if (model == null || model.Password != model.ConfirmPassword)
            {
                return BadRequest(badUserNameOrPasswordMessage);
            }
            else
            {
                var now = DateTime.Now;
                var user = new UserEntity();
                user.UserName = model.UserName;
                user.PasswordHash = GetSHA256HashedString(model.Password);
                user.IsDeleted = false;
                user.CreatedAt = now;
                user.UpdatedAt = now;
                _context.users.Add(user);
                await _context.SaveChangesAsync();
                return LocalRedirect("/login");
            }
        }
    }
}