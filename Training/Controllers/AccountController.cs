using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Crypto.Generators;
using BCrypt.Net;
using Training.Models;
using Training.Service;
using System.Security.Principal;
using Training.Data;

namespace Training.Controllers
{
    public class AccountController : Controller
    {
        private AccountService accountService;
        private readonly TrainingDbContext _context;
        public AccountController(AccountService accountService, TrainingDbContext context)
        {
            this.accountService = accountService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Account/SignUp")]
        public IActionResult SignUp()
        {
            return View("SignUp", new Infor());
        }

        [HttpPost]
        [Route("Account/SignUp")]
        public IActionResult SignUp(Infor account)
        {
            if (!ModelState.IsValid)
            {
                return View(account);
            }
            if (account.Password != account.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                return View(account);
            }

            //account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            account.AccountId = Guid.NewGuid().ToString();

            try
            {
                if (accountService.Create(account))
                {
                    return RedirectToAction("SignIn");
                }
                else
                {
                    TempData["Msg"] = "Đăng kí thất bại do lỗi hệ thống";
                    return RedirectToAction("SignUp");
                }
            }
            catch (Exception ex)
            {
                TempData["Msg"] = ex.Message; 
                return View(account);
            }
        }

        [HttpGet]
        [Route("Account/SignIn")]
        public IActionResult SignIn()
        {
            return View("SignIn");
        }

        [HttpPost]
        [Route("Account/SignIn")]
        public IActionResult SignIn(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập và mật khẩu không được để trống.");
                return View("SignIn");
            }

          
            var account = _context.Accounts.FirstOrDefault(a => a.Username == username);

           
            if (account == null ||account.Password != password)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                return View("SignIn");
            }

          
            HttpContext.Session.SetString("UserRole", account.Role); 
            HttpContext.Session.SetString("AccountId", account.AccountId);
            HttpContext.Session.SetString("UserId", account.AccountId);
            return RedirectToAction("IndexUser", "Home");
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


    }
}
