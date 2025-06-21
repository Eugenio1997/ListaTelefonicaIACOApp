using Dapper;
using ListaTelefonicaIACOApp.Infrastructure;
using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ListaTelefonicaIACOApp.Controllers
{
    public class AutenticacaoController : Controller
    {

        private ListaTelefonicaDbContext _context;
        public AutenticacaoController(ListaTelefonicaDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");


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

        [Authorize(Roles = "admin,recepcao,guarita")]
        [HttpGet("autenticado")]
        public IActionResult Autenticado()
        {
            if (User.Identity?.IsAuthenticated == true)
                return Ok(new { autenticado = true, nome = User.Identity.Name });

            return Unauthorized(new { autenticado = false });
        }

        public async Task<Usuario> ObterUsuarioPorNomeESenhaAsync(LoginViewModel usuario)
        {

            var query = $@"
                SELECT ID, NOME, PERFIL
                FROM USUARIOS
                WHERE NOME = '{usuario.Nome}' AND SENHA = '{usuario.Senha}'";

            using (var conn = _context.CreateConnection())
            {
                conn.Open();

                return await conn.QueryFirstOrDefaultAsync<Usuario>(query);


            }



        }


    }
}
