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
        public ContatoIndexViewModel contato = new ContatoIndexViewModel();
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
                        FROM LISTA_CONTATOS 
                        ORDER BY {ordenacao}
                        OFFSET {offset} ROWS
                        FETCH NEXT {registrosPorPagina} ROWS ONLY";

            //contatos paginados
            if (_conn == null)
                _conn = await _context.GetOpenConnectionAsync();

            var totalRegistros = await _conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM LISTA_CONTATOS");
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            var contatos = (await _conn.QueryAsync<ContatoCadastroViewModel>(query)).ToList();

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
                    $"<a href='/Contato/Edit/{contato.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'><i class='bi-pencil-square text-dark'></i></a></td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                    $"<a href='/Contato/Details/{contato.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'><i class='bi bi-eye text-dark'></i></a></td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                    $"<a href='/Contato/Delete/{contato.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Deletar'><i class='bi bi-trash text-dark'></i></a></td>");


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
            var totalRegistros = await _conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM LISTA_CONTATOS");
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            offset = (paginaAtual - 1) * registrosPorPagina;
            string ordenacao = "NOME"; // default order by NOME
            string query = @$"
                            SELECT 
                                c.ID AS Contato_Id,
                                c.NOME AS Contato_Nome,
                                c.SOBRENOME AS Contato_Sobrenome,
                                c.FIXO AS Contato_Fixo,
                                c.CELULAR AS Contato_Celular,
                                c.COMERCIAL AS Contato_Comercial,
                                c.ENDERECO_ID AS Contato_EnderecoId,
                                c.EMAIL AS Contato_Email,

                                e.ID AS Endereco_Id,
                                e.CEP AS Endereco_CEP,
                                e.RUA AS Endereco_Rua,
                                e.NUMERO AS Endereco_Numero,
                                e.BAIRRO AS Endereco_Bairro,
                                e.CIDADE AS Endereco_Cidade,
                                e.COMPLEMENTO AS Endereco_Complemento
                            FROM LISTA_CONTATOS c
                            LEFT JOIN LISTA_ENDERECOS e ON c.ENDERECO_ID = e.ID
                            ORDER BY {ordenacao}
                            OFFSET {offset} ROWS FETCH NEXT {registrosPorPagina} ROWS ONLY";

            var contatos = await _conn.QueryAsync<ContatoIndexViewModel, EnderecoViewModel, ContatoIndexViewModel>(
                query,
                (contato, endereco) =>
                {
                    contato.Endereco = endereco;
                    return contato;
                },
                splitOn: "Endereco_Id"
            );
            var contatoIndexViewModel = new ContatoIndexViewModel
            {
                Colaboradores = contatos.ToList()
            };
            ViewBag.PaginaAtual = paginaAtual;
            ViewBag.TotalPaginas = totalPaginas;

            return View(contatoIndexViewModel);
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
