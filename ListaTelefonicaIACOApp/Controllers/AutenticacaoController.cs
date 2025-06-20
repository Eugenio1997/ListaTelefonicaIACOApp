using ListaTelefonicaIACOApp.Services.Ldap;
using ListaTelefonicaIACOApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ListaTelefonicaIACOApp.Controllers
{
    public class AutenticacaoController : Controller
    {
        private readonly LdapService _ldapService;

        public AutenticacaoController(LdapService ldapService)
        {
            _ldapService = ldapService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            bool isValid = await _ldapService.AuthenticateAgainstLdap(model.Nome, model.Senha);

            if (!isValid)
                return Unauthorized("Usuário ou senha inválidos.");

            // Cria as Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, model.Nome),
                new Claim(ClaimTypes.Name, model.Nome)
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            return Ok(new { sucesso = true, mensagem = "Login realizado com sucesso." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return Ok(new { sucesso = true, mensagem = "Logout efetuado com sucesso." });
        }

        [HttpGet("autenticado")]
        public IActionResult Autenticado()
        {
            if (User.Identity?.IsAuthenticated == true)
                return Ok(new { autenticado = true, nome = User.Identity.Name });

            return Unauthorized(new { autenticado = false });
        }
    }
}
