using Dapper;
using ListaTelefonicaIACOApp.Constantes;
using ListaTelefonicaIACOApp.Infrastructure;
using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.Services;
using ListaTelefonicaIACOApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ListaTelefonicaIACOApp.Controllers
{
    public class AutenticacaoController : Controller
    {

        private ListaTelefonicaDbContext _context;
        private readonly IHashService _hashService;
        public AutenticacaoController(ListaTelefonicaDbContext context, IHashService hashService)
        {
            _context = context;
            _hashService = hashService;
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            var usuario = new Usuario
            {
                Nome = model.Nome,
                Role = model.Role,
                Senha = model.Senha
            };

            var query = $"SELECT * FROM USUARIOS WHERE NOME = '{model.Nome}'";
            using var conn = _context.CreateConnection();

            conn.Open();

            // Consulta usando interpolação de strings (não seguro para produção)

            var usuarioDB = await conn.QueryFirstOrDefaultAsync<Usuario>(query);

            var senhaHash = _hashService.HashPassword(usuario, model.Senha);

            if (usuarioDB != null && (_hashService.VerifyPassword(usuario, senhaHash, usuarioDB.Senha)))
            {
                return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });
            }

            // Cria os claims de identidade
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuarioDB.Nome),
                new Claim("UserId", usuarioDB.Id.ToString()),
                new Claim(ClaimTypes.Role, usuarioDB.Role ?? "Usuario")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Cria o cookie de autenticação
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Retorna sucesso para AJAX
            return Ok(new { mensagem = "Login realizado com sucesso" });
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Contato");
        }

        [Authorize(Roles = $"{Roles.Administrador},{Roles.Recepcao},{Roles.Guarita}")]
        [HttpGet("autenticado")]
        public IActionResult Autenticado()
        {
            if (User.Identity?.IsAuthenticated == true)
                return Ok(new { autenticado = true, nome = User.Identity.Name });

            return Unauthorized(new { autenticado = false });
        }

    }
}
