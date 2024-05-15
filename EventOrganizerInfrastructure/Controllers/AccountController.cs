using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EventOrganizerInfrastructure.ViewModel;
using EventOrganizerDomain.Model;


namespace EventOrganizerInfrastructure.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model.Role == "Participant")
            {
                // Удаляем ошибки валидации для полей организации и номера телефона
                ModelState.Remove("OrganizationName");
                ModelState.Remove("PhoneNumber");
            }

           
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Email,
                    RegistrationDate = DateOnly.FromDateTime(DateTime.Today)
                };

                // додаємо користувача
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.Role == "Organizer")
                    {
                        user.OrganizationName = model.OrganizationName;
                        user.PhoneNumber = model.PhoneNumber;
                        await _userManager.AddToRoleAsync(user, "Organizer");
                    }
                    else if (model.Role == "Participant")
                    {
                        await _userManager.AddToRoleAsync(user, "Participant");
                    }

                    // установка кукі
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
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
            public IActionResult Login(string returnUrl = null)
        {
            // Якщо returnUrl пустий, то пробуємо отримати з заголовку Referer
            if (string.IsNullOrEmpty(returnUrl) && Request.Headers.ContainsKey("Referer"))
            {
                returnUrl = Request.Headers["Referer"];
            }

            // Передаем returnUrl в представление
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }                  
                    return RedirectToAction("Index", "Home");
                    
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Неправильний логін або пароль");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // видаляємо автентифікаційні куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}