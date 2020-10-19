using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using exam.Models;
using exam.Models.Account;
using Microsoft.AspNetCore.Authorization;
using EmailApp;
using exam.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace exam.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { Email = model.Email, UserName = model.Email };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // генерация токена для пользователя
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: HttpContext.Request.Scheme);
                    EmailService emailService = new EmailService();
                    await emailService.SendEmailAsync(model.Email, "Confirm your account",
                        $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");

                    return Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(model.Email);
                var password = await _userManager.CheckPasswordAsync(user, model.Password);

                if (password)
                {
                    await _signInManager.SignInAsync(user, false);

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ChangePassword(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);
            
            if (user == null)
            {
                return NotFound();
            }
            MyAccountViewModel model = new MyAccountViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(MyAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }
        [Authorize(Roles = "client")]
        public async Task<IActionResult> MyAccount(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }
            MyAccountViewModel model = new MyAccountViewModel { Name = user.UserName,Id = user.Id, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> MyAccount(MyAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByIdAsync(model.Id);
                var password = await _userManager.CheckPasswordAsync(user, model.OldPassword);

                if (password)
                {
                    var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.Email);
                    var resetLink = Url.Action("ChangeEmailToken", "Account", new { token = token, oldEmail = user.Email, newEmail = model.NewEmail }, protocol: HttpContext.Request.Scheme);
                    EmailService emailService = new EmailService();
                    await emailService.SendEmailAsync(model.Email, "Reset pass", resetLink);


                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ChangeEmailToken([FromQuery] string token, [FromQuery] string oldEmail, [FromQuery] string newEmail)
        {
            AppUser user = await _userManager.FindByEmailAsync(oldEmail);
            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction("Index");
        }


        [Authorize(Roles = "client")]
        public async Task<IActionResult> SubscribeTrainings(int id)
        {
            var training = _context.Trainings.Where(w => w.TTypeId == id).Include(t => t.Hall).Include(t => t.TType).Include(t => t.Trainer);

            return View(await training.ToListAsync());
        }
        [Authorize(Roles = "client")]
        public async Task<IActionResult> SubscribePost(int trainingId, string userName)
        {
            AppUser user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                Training tr = _context.Trainings.Where(t => t.Id == trainingId).FirstOrDefault();

                if (tr != null)
                {
                    var hall = _context.Halls.Where(hall => hall.Id == tr.HallId).FirstOrDefault();
                    if (hall.Capacity > 0)
                    {
                        tr.Clients.Add(new ClientTraining() { Client = user });
                        hall.Capacity--;
                        await _context.SaveChangesAsync();
                    }
        
                }

            }

            return RedirectToAction("Index","Home");
        }
        [AllowAnonymous]
        public ActionResult TimeTable()
        {
            return View();
        }

        public JsonResult GetTimeTable()
        {
            var applicationDbContext = _context.Trainings.Include(t => t.Hall).Include(t => t.TType).Include(t => t.Trainer);
            return Json(applicationDbContext.ToList());
        }

    }

}