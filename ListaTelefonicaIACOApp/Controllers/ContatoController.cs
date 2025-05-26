using Dapper;
using ListaTelefonicaIACOApp.Infrastructure;
using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Text;


namespace ListaTelefonicaIACOApp.Controllers
{
    public class ContatoController : Controller
    {
        private readonly ILogger<ContatoController> _logger;
        private readonly IConfiguration? _configuration;
        private readonly ListaTelefonicaDbContext _context;
        public ContatoViewModel contato = new ContatoViewModel();
        int registrosPorPagina = 10;
        int totalRegistros;
        int totalPaginas;
        int offset;
        private IDbConnection _conn;

        public ContatoController(ILogger<ContatoController> logger,
                                 IConfiguration configuration,
                                 ListaTelefonicaDbContext context
                                   )
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

       
        public async Task<IActionResult> ObterColaboradoresPaginados(int registrosPorPagina = 10, int paginaAtual = 1)
        {
            
            offset = (paginaAtual - 1) * registrosPorPagina;
            string ordenacao = "NOME"; // default order by NOME
            string query = @$"SELECT *
                        FROM LISTA_FONES 
                        ORDER BY {ordenacao}
                        OFFSET {offset} ROWS
                        FETCH NEXT {registrosPorPagina} ROWS ONLY";

            //contatos paginados
            if (_conn == null)
                _conn = await _context.GetOpenConnectionAsync();

            var totalRegistros = await _conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM LISTA_FONES");
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            var contatos = (await _conn.QueryAsync<ContatoViewModel>(query)).ToList();
          
            StringBuilder sb = new StringBuilder();
            foreach (var contato in contatos)
            {
                sb.Append("<tr>");
                sb.Append($"<td class='text-nowrap'>{contato.Id}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Nome}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Fixo}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Celular}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Comercial}</td>");
                //sb.Append($"<td class='text-nowrap'>{contato.Endereco}</td>");
                sb.Append($"<td class='text-nowrap'><a href='mailto:{contato.Email}'>{contato.Email}</a></td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                    $"<a href='/Contato/Edit/{contato.Id}' title='Editar'><i class='bi-pencil-square text-dark'></i></a></td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                    $"<a href='/Contato/Details/{contato.Id}' title='Detalhes'><i class='bi bi-eye text-dark'></i></a></td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                    $"<a href='/Contato/Delete/{contato.Id}' title='Excluir'><i class='bi bi-trash text-dark'></i></a></td>");


            }

            return Json(new
            {
                html = sb.ToString(),
                paginaAtual,
                totalPaginas
            });
        }

        // GET: ColaboradorController
        public async Task<IActionResult> Index(int paginaAtual = 1)
        {

            _conn = await _context.GetOpenConnectionAsync();
            var totalRegistros = await _conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM LISTA_FONES");
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            offset = (paginaAtual - 1) * registrosPorPagina;
            string ordenacao = "NOME"; // default order by NOME
            string query = @$"SELECT *
                        FROM LISTA_FONES 
                        ORDER BY {ordenacao}
                        OFFSET {offset} ROWS
                        FETCH NEXT {registrosPorPagina} ROWS ONLY";

            //contatos paginados
            contato.Colaboradores = (await _conn.QueryAsync<ContatoViewModel>(query)).ToList();

           

            ViewBag.PaginaAtual = paginaAtual;
            ViewBag.TotalPaginas = totalPaginas;

            return View(contato);
        }


        // GET: ColaboradorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ColaboradorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ColaboradorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FiltrarPorContatos(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ColaboradorController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return View();
        }

        // POST: ColaboradorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ColaboradorController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        // POST: ColaboradorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
