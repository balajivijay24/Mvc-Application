using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IdentityManagement.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Web;
using static System.Console;
using Microsoft.AspNetCore.Http;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace IdentityManagement.Controllers;

public class AdminController : Controller
{
  

    public static string? password { get; private set; }
    public static string? Mail { get; private set; }

    [Authorize]
       public IActionResult Profile()
    {
       
        return View();
    }
    
     [HttpGet]
    public IActionResult AdminLogin()
    {
        ClaimsPrincipal claimUser=HttpContext.User;
        if(claimUser.Identity.IsAuthenticated){

            return RedirectToAction("Profile");
        }
        TempData["pass"]=password;
        return View();
    }
    
    [HttpPost]

     public async Task<IActionResult> AdminLoginAsync(User user , Database database)
    {
       string result= Database.AdminLogin(user);
       WriteLine(result);
       try{
       if(result=="Admin")
       {
        List<Claim> claims=new List<Claim>(){
            new Claim(ClaimTypes.NameIdentifier,user.EmailId)
        };
        ClaimsIdentity claimsIdentity=new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
        AuthenticationProperties properties=new AuthenticationProperties(){
            AllowRefresh=true,
            IsPersistent=database.KeepLoggedIn
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),properties);
         Log.Information("Employee Login Triggered");

          HttpContext.Session.SetString("EmailId",Database.display(user.EmailId));
          return View("Profile",user);
       }

      else {
        TempData["msg"]="Login Id or password is incorrect";
       return View("AdminLogin",user);
      }

    }

    catch(Exception exception)
        {
         Console.WriteLine("The Error was Occured: " + exception.Message);
         return RedirectToAction("Index","Error");
        }
        
    }



///forget password///
    [HttpGet]
    public IActionResult forgotPassword()
    {
         return View();
    }


    [HttpPost]
    public IActionResult forgotPassword(User employee)
    {   
      string mail=Database.sendEmail(employee.EmailId,employee);
      Mail=employee.EmailId;
        if (mail=="sent")
        {
            ViewBag.OTP="Verification Code sent successfuly \n Enter otp to continue";
            return RedirectToAction("checkCode","Admin",employee);
        }
        else
        {
            ViewBag.Message="Your are not a registered user";
            return View("forgotPassword");
        }
    }

    [HttpGet]
    public IActionResult checkCode()
    {
         return View();
    }

    public IActionResult checkCode(Database database)
    {  
      string code=Database.verifyCode(database);
        if (code=="code sent")
        { 
            password=Database.sendPassword(Mail);
            return RedirectToAction("AdminLogin","Admin");
        }
        else
        {
            ViewBag.OTP="Incorrect Code";
            return View("checkCode",database);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
