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


        public async Task<IActionResult> ObterContatosPaginados(int registrosPorPagina = 10, int paginaAtual = 1)
        {

            offset = (paginaAtual - 1) * registrosPorPagina;
            string ordenacao = "NOME"; // default order by NOME
            string query = @$"SELECT 
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

            //contatos paginados
            if (_conn == null)
                _conn = await _context.GetOpenConnectionAsync();

            var totalRegistros = await _conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM LISTA_CONTATOS");
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            var contatos = await _conn.QueryAsync<ContatoIndexViewModel, EnderecoViewModel, ContatoIndexViewModel>(
                query,
                (contato, endereco) =>
                {
                    contato.Endereco = endereco;
                    return contato;
                },
                splitOn: "Endereco_Id"
            );


            StringBuilder sb = new StringBuilder();

            foreach (var contato in contatos)
            {
                sb.Append("<tr>");
                //sb.Append($"<td class='text-nowrap'>{contato.Id}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Contato_Nome}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Contato_Sobrenome}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Contato_Fixo}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Contato_Celular}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Contato_Comercial}</td>");
                //sb.Append($"<td class='text-nowrap'>{contato.Endereco}</td>");
                sb.Append($"<td class='text-nowrap'><a href='mailto:{contato.Contato_Email}'>{contato.Contato_Email}</a></td>");
                sb.Append($"<td class='text-nowrap'>{contato.Endereco.Endereco_Rua}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Endereco.Endereco_Numero}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Endereco.Endereco_Bairro}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Endereco.Endereco_Cidade}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Endereco.Endereco_Complemento}</td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                    $"<a href='/Contato/Edit/{contato.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'><i class='bi-pencil-square text-dark'></i></a></td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                    $"<a href='/Contato/Details/{contato.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'><i class='bi bi-eye text-dark'></i></a></td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                    $"<a href='/Contato/Delete/{contato.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Deletar'><i class='bi bi-trash text-dark'></i></a></td>");


            }

            return Json(new
            {
                html = sb.ToString(),
                paginaAtual,
                totalPaginas
            });
        }

        [HttpPost]
        public async Task<IActionResult> ObterContatosFiltrados([FromBody] List<FiltroViewModel> filtros)
        {
            string queryBase = @"
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
                        WHERE 1=1
                    ";

            var parametros = new DynamicParameters();

            foreach (var filtro in filtros)
            {
                if (filtro.Nome == "Contato_Nome")
                {
                    queryBase += $@" AND LOWER(c.NOME) LIKE LOWER({filtro.Valor})";

                }
                if (filtro.Nome == "Endereco.Contato_Sobrenome")
                {
                    queryBase += $@" AND LOWER(c.SOBRENOME) = LOWER({filtro.Valor})";
                }
                if (filtro.Nome == "Endereco.Contato_Fixo")
                {
                    queryBase += $@" AND LOWER(c.FIXO) = LOWER({filtro.Valor})";
                }
                if (filtro.Nome == "Endereco.Contato_Celular")
                {
                    queryBase += $@" AND LOWER(c.CELULAR) = LOWER({filtro.Valor})";
                }
                if (filtro.Nome == "Endereco.Contato_Comercial")
                {
                    queryBase += $@" AND LOWER(c.COMERCIAL) = LOWER({filtro.Valor})";
                }
                if (filtro.Nome == "Endereco.Contato_Email")
                {
                    queryBase += $@" AND LOWER(c.EMAIL) = LOWER({filtro.Valor})";
                }
                if (filtro.Nome == "Endereco.Endereco_Rua")
                {
                    queryBase += $@" AND LOWER(c.RUA) = LOWER({filtro.Valor})";
                }
                if (filtro.Nome == "Endereco.Endereco_Bairro")
                {
                    queryBase += $@" AND LOWER(e.BAIRRO) = LOWER({filtro.Valor})";
                }
                if (filtro.Nome == "Endereco.Endereco_Cidade")
                {
                    queryBase += $@" AND LOWER(e.CIDADE) = LOWER({filtro.Valor})";
                }

            }

            queryBase += " ORDER BY c.NOME";

            if (_conn == null)
                _conn = await _context.GetOpenConnectionAsync();

            var contatos = await _conn.QueryAsync<ContatoIndexViewModel, EnderecoViewModel, ContatoIndexViewModel>(
                queryBase,
                (contato, endereco) =>
                {
                    contato.Endereco = endereco;
                    return contato;
                },
                splitOn: "Endereco_Id",
                param: parametros
            );

            var sb = new StringBuilder();

            foreach (var c in contatos)
            {
                sb.Append("<tr>");
                //sb.Append($"<td class='text-nowrap'>{contato.Id}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Contato_Nome}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Contato_Sobrenome}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Contato_Fixo}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Contato_Celular}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Contato_Comercial}</td>");
                //sb.Append($"<td class='text-nowrap'>{contato.Endereco}</td>");
                sb.Append($"<td class='text-nowrap'><a href='mailto:{contato.Contato_Email}'>{contato.Contato_Email}</a></td>");
                sb.Append($"<td class='text-nowrap'>{contato.Endereco.Endereco_Rua}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Endereco.Endereco_Numero}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Endereco.Endereco_Bairro}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Endereco.Endereco_Cidade}</td>");
                sb.Append($"<td class='text-nowrap'>{contato.Endereco.Endereco_Complemento}</td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                    $"<a href='/Contato/Edit/{contato.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'><i class='bi-pencil-square text-dark'></i></a></td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                    $"<a href='/Contato/Details/{contato.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'><i class='bi bi-eye text-dark'></i></a></td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                    $"<a href='/Contato/Delete/{contato.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Deletar'><i class='bi bi-trash text-dark'></i></a></td>");
            }

            return Content(sb.ToString(), "text/html");
        }


        // GET: ContatoController
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


        // GET: ContatoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ContatoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContatoController/Create
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

        // GET: ContatoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return View();
        }

        // POST: ContatoController/Edit/5
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

        // GET: ContatoController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        // POST: ContatoController/Delete/5
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
