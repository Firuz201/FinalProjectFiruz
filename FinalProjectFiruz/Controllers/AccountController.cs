using FinalProjectFiruz.Abstraction;
using FinalProjectFiruz.Models;
using FinalProjectFiruz.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FinalProjectFiruz.Controllers;

public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager, IEmailService _emailService) : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm)
    {
        if(!ModelState.IsValid)
            return View(vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);

        if(user is null)
        {
            ModelState.AddModelError("", "Email or password is wrong");
            return View(vm);
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, vm.Password, false, true);

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError("", "Email or password is wrong");
            return View(vm);
        }

        if(!user.EmailConfirmed)
        {
            ModelState.AddModelError("", "Please confirm your email address");
            await SendConfirmationEmailAsync(user); 
            return View(vm);
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var existUser = await _userManager.FindByNameAsync(vm.Username);

        if(existUser is { })
        {
            ModelState.AddModelError("Username", "This username is already exist");
            return View(vm);
        }

        existUser = await _userManager.FindByEmailAsync(vm.Email);

        if( existUser is { })
        {
            ModelState.AddModelError("Email", "This email is already exist");
            return View(vm);
        }

        ////_userManager.Users.FirstOrDefault(x => x.Id == "1");

        AppUser user = new()
        {
            Fullname = vm.Fullname,
            UserName = vm.Username,
            Email = vm.Email
        };

        var result = await _userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);

            }

            return View(vm);
        }

        await _userManager.AddToRoleAsync(user, "Member");

        //await _signInManager.SignInAsync(user, false);

        await SendConfirmationEmailAsync(user);

        return RedirectToAction("Login");
    }

    //public async Task <IActionResult> CreateRoles()
    //{
    //    await _roleManager.CreateAsync(new() { Name = "Admin" });
    //    await _roleManager.CreateAsync(new() { Name = "Member" });
    //    await _roleManager.CreateAsync(new() { Name = "Moderator" });
    //    await _roleManager.CreateAsync(new() { Name = "Attention" });

    //    return Ok("Roles was created");
    //}

    //public async Task <IActionResult> CreateAdminAndModerator()
    //{
    //    AppUser adminUser = new()
    //    {

    //    };
    //}

    private async Task SendConfirmationEmailAsync(AppUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //await _userManager.ConfirmEmailAsync(user, token);

        //string url = @$"https://localhost:7257/Account/ConfirmEmail?token={token}&userId={user.Id}";
        string url = Url.Action("ConfirmEmail", "Account", new { token = token, userId=user.Id }, Request.Scheme) ?? string.Empty;

        string emailBody = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
  <title>Email Confirmed</title>
</head>
<body style=""margin:0;padding:0;background-color:#0a0a0f;font-family:'Georgia',serif;min-height:100vh;display:flex;align-items:center;justify-content:center;"">

  <!-- Outer wrapper -->
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""background-color:#0a0a0f;min-height:100vh;"">
    <tr>
      <td align=""center"" valign=""middle"" style=""padding:40px 20px;"">

        <!-- Card -->
        <table width=""520"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""background-color:#13131a;border:1px solid #2a2a3d;border-radius:4px;overflow:hidden;max-width:520px;"">

          <!-- Top accent line -->
          <tr>
            <td style=""height:3px;background:linear-gradient(90deg,#c8a96e,#e8c98e,#c8a96e);""></td>
          </tr>

          <!-- Header -->
          <tr>
            <td align=""center"" style=""padding:52px 48px 36px;"">

              <!-- Seal / Icon -->
              <table cellpadding=""0"" cellspacing=""0"" border=""0"">
                <tr>
                  <td align=""center"">
                    <div style=""width:72px;height:72px;border-radius:50%;background-color:#1c1c28;border:1px solid #2e2e45;display:inline-flex;align-items:center;justify-content:center;margin-bottom:0;"">
                      <!-- Checkmark SVG -->
                      <svg width=""72"" height=""72"" viewBox=""0 0 72 72"" xmlns=""http://www.w3.org/2000/svg"">
                        <circle cx=""36"" cy=""36"" r=""35"" fill=""#1c1c28"" stroke=""#2e2e45"" stroke-width=""1""/>
                        <circle cx=""36"" cy=""36"" r=""28"" fill=""none"" stroke=""#c8a96e"" stroke-width=""0.5"" stroke-dasharray=""3 4""/>
                        <polyline points=""23,36 32,45 50,27"" fill=""none"" stroke=""#c8a96e"" stroke-width=""2.5"" stroke-linecap=""round"" stroke-linejoin=""round""/>
                      </svg>
                    </div>
                  </td>
                </tr>
              </table>

              <!-- Wordmark -->
              <p style=""margin:28px 0 6px;font-size:11px;letter-spacing:5px;color:#c8a96e;text-transform:uppercase;font-family:'Georgia',serif;"">Verified</p>
              <h1 style=""margin:0 0 12px;font-size:28px;font-weight:400;color:#f0ece4;letter-spacing:1px;font-family:'Georgia',serif;line-height:1.2;"">Email Confirmed</h1>
              <p style=""margin:0;font-size:14px;color:#6b6b8a;letter-spacing:0.5px;line-height:1.7;font-family:'Georgia',serif;"">Your identity has been verified.<br>Welcome to the fold.</p>

            </td>
          </tr>

          <!-- Divider -->
          <tr>
            <td style=""padding:0 48px;"">
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                <tr>
                  <td style=""height:1px;background-color:#1f1f30;""></td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Details block -->
          <tr>
            <td style=""padding:32px 48px;"">
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">

                <tr>
                  <td style=""padding-bottom:16px;"">
                    <p style=""margin:0 0 4px;font-size:10px;letter-spacing:3px;color:#4a4a6a;text-transform:uppercase;font-family:'Georgia',serif;"">Account</p>
                    <p style=""margin:0;font-size:15px;color:#c8c8e0;font-family:'Georgia',serif;"">{user.Email}</p>
                  </td>
                </tr>

                

              </table>
            </td>
          </tr>

          <!-- Divider -->
          <tr>
            <td style=""padding:0 48px;"">
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                <tr>
                  <td style=""height:1px;background-color:#1f1f30;""></td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- CTA -->
          <tr>
            <td align=""center"" style=""padding:36px 48px 44px;"">
              <a href=""{url}"" style=""display:inline-block;padding:13px 42px;background-color:transparent;border:1px solid #c8a96e;color:#c8a96e;text-decoration:none;font-size:11px;letter-spacing:4px;text-transform:uppercase;font-family:'Georgia',serif;border-radius:2px;"">Enter Your Account</a>
              
            </td>
          </tr>

          <!-- Bottom accent -->
          <tr>
            <td style=""height:3px;background:linear-gradient(90deg,#c8a96e,#e8c98e,#c8a96e);""></td>
          </tr>

        </table>
        <!-- /Card -->

        <!-- Footer note -->
        <p style=""margin:28px 0 0;font-size:11px;color:#2e2e45;letter-spacing:1px;font-family:'Georgia',serif;"">© 2026 &nbsp;·&nbsp; All rights reserved</p>

      </td>
    </tr>
  </table>

</body>
</html>";

        await _emailService.SendEmailAsync(user.Email!, "Confirm your email", emailBody);
    }

    public async Task<IActionResult> ConfirmEmail(string token, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return BadRequest();
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            return BadRequest();
        }

        await _signInManager.SignInAsync(user, false);

        return RedirectToAction("Index", "Home");
    }
}
