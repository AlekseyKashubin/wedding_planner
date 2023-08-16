using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using wedding_planner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace wedding_planner.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;

    private MyContext db;

    public UserController(ILogger<UserController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

// ===============================root route=================================
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    // ===========================Create user Method==================================

    [HttpPost("user/create")]
    public IActionResult Register(User newUser)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }
        PasswordHasher<User> hashBrowns = new PasswordHasher<User>();
        newUser.Password = hashBrowns.HashPassword(newUser, newUser.Password);
        db.Users.Add(newUser);
        db.SaveChanges();
        HttpContext.Session.SetInt32("UUID", newUser.UserId);
        HttpContext.Session.SetString("UName", newUser.FirstName);
        return RedirectToAction("Index", "Wedding");
    }


    [HttpGet("success")]
    public IActionResult Success()
    {
        if (HttpContext.Session.GetInt32("UUID") == null)
        {
            return RedirectToAction("Index");
        }
        return View("Index", "Wedding");
    }


//======================login===============================
    [HttpPost("users/login")]
    public IActionResult Login(LoginUser loginUser)
    {
        if (ModelState.IsValid)
        {
            User? userInDb = db.Users.FirstOrDefault(u => u.Email == loginUser.LoginEmail);
            if (userInDb == null)
            {
                ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                return View("Index");
            }
            PasswordHasher<LoginUser> hashBrown = new PasswordHasher<LoginUser>();
            var result = hashBrown.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.LoginPassword);
            if (result == 0)
            {
                ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                return View("Index");
            }
            else
            {
                HttpContext.Session.SetInt32("UUID", userInDb.UserId);
                HttpContext.Session.SetString("UName", userInDb.FirstName);
                return RedirectToAction("Index", "Wedding");
            }
        }
        else
        {
            return View("Index");
        }
    }




// ==========logout=============
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Wedding");
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


