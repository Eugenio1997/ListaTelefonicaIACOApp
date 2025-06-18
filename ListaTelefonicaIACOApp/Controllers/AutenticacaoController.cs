using ListaTelefonicaIACOApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ListaTelefonicaIACOApp.Controllers
{
    public class AutenticacaoController : Controller
    {
        private readonly string _domain = "YOUR_DOMAIN"; // e.g., yourcompany.local

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool isValid = ValidateUser(model.Nome, model.Senha);

            if (isValid)
            {
                // Authenticate user manually (e.g., using cookie auth)
                // Redirect to secure area
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password.";
            return View(model);
        }

        private bool ValidateUser(string nome, string senha)
        {
            /*
            using (var context = new PrincipalContext(ContextType.Domain, _domain))
            {
                return context.ValidateCredentials(username, password);
            }
            */
            return true;
        }

    }
}
