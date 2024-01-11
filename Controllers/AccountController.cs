using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IdentityManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Net;
using System.Configuration;
using System.Net.Mail;
using System.Data.SqlClient;

public class AccountController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration configuration;
    public AccountController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5184/");
        this.configuration=configuration;
    }

    [HttpGet]
public IActionResult Signup()
{
    var viewModel = new SignupViewModel();
    return View("Signup", viewModel);
}



   [HttpPost]
public async Task<IActionResult> Signup(SignupViewModel signupViewModel)
{
    try{
    if (!ModelState.IsValid)
    {
        // There are validation errors, create a new SignupViewModel object
        var newSignupViewModel = new SignupViewModel
        {
            userID = signupViewModel.userID,
            password = signupViewModel.password,
            confirmpassword = signupViewModel.confirmpassword
        };
        return View(newSignupViewModel);
    }

   using( var signupResponse = await _httpClient.PostAsJsonAsync("/signup", signupViewModel)){

    if (signupResponse.IsSuccessStatusCode)
    {
        // Signup was successful, redirect to the login page
        return RedirectToAction("Profile","Admin");
    }
    else
    {
        // Signup failed, display an error message to the user
        var errorMessage = await signupResponse.Content.ReadAsStringAsync();
        ModelState.AddModelError("", errorMessage);
        return View(signupViewModel);
    }
    }
    }
    catch(HttpRequestException ex){
        Console.WriteLine("The Error was Occured in while Connecting to Web Api: " + ex.Message);
         return RedirectToAction("WebApi","Error");

    }
}

    
}