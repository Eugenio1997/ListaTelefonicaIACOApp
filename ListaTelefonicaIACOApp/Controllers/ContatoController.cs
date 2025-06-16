using Dapper;
using ListaTelefonicaIACOApp.Infrastructure;
using ListaTelefonicaIACOApp.Mappers;
using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.ViewModels.Contato;
using ListaTelefonicaIACOApp.ViewModels.Endereco;
using ListaTelefonicaIACOApp.ViewModels.Filtros;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


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
                                c.ID AS Id,
                                c.NOME AS Nome,
                                c.FIXO AS Fixo,
                                c.CELULAR AS Celular,
                                c.COMERCIAL AS Comercial,
                                c.ENDERECO AS Endereco,
                                c.EMAIL AS Email,
                                TO_CHAR(c.CRIADO_AS, 'DD/MM/YYYY HH24:MI:SS')  AS CriadoAs,
                                TO_CHAR(c.EDITADO_AS, 'DD/MM/YYYY HH24:MI:SS') AS EditadoAs
                            FROM LISTA_CONTATOS c
                            ORDER BY {ordenacao}
                            OFFSET {offset} ROWS FETCH NEXT {registrosPorPagina} ROWS ONLY";


            using (var conn = _context.CreateConnection())
            {
                conn.Open();



                var totalRegistros = await conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM LISTA_CONTATOS");
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);




                StringBuilder sb = new StringBuilder();
                var contatos = (await conn.QueryAsync<Contato>(query)).ToList();


                foreach (var c in contatos)
                {
                    sb.Append($"<tr style='height:60px'>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Nome}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-fixo input-sem-borda' value='{c.Fixo}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-celular input-sem-borda' value='{c.Celular}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-comercial input-sem-borda' value='{c.Comercial}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><a href='mailto:{c.Email}'>{c.Email}</a></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.CriadoAs}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.EditadoAs}</td>");
                    ;
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Edit/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'><i class='bi-pencil-square text-dark'></i></a></td>");
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Details/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'><i class='bi bi-eye text-dark'></i></a></td>");
                    sb.Append($"<td style='width:50px; height:60px' class='text-nowrap'>" +
                         $"<a href='#' class='btn-abrir-modal-exclusao' data-id='{c.Id}' data-nome='{System.Net.WebUtility.HtmlEncode(c.Nome)}' data-bs-toggle='modal' data-bs-target='#modalConfirmarExclusao' title='Deletar'>" +
                         $"<i class='bi bi-trash text-dark'></i></a></td>");
                    sb.Append("</tr>");

                }

                return Json(new
                {
                    html = sb.ToString(),
                    paginaAtual,
                    totalPaginas
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterContatosFiltrados(FiltroContatoViewModel filtros, int registrosPorPagina = 10, int paginaAtual = 1)
        {

            offset = (paginaAtual - 1) * registrosPorPagina;

            string queryBase = @"
                    SELECT 
                        c.ID              AS Id,
                        c.NOME            AS Nome,
                        c.FIXO            AS Fixo,
                        c.CELULAR         AS Celular,
                        c.COMERCIAL       AS Comercial,
                        c.ENDERECO        AS Endereco,
                        c.EMAIL           AS Email,
                        TO_CHAR(c.CRIADO_AS, 'DD/MM/YYYY HH24:MI:SS')  AS CriadoAs,
                        TO_CHAR(c.EDITADO_AS, 'DD/MM/YYYY HH24:MI:SS') AS EditadoAs
                    FROM LISTA_CONTATOS c
                    WHERE 1=1
                ";



            if (!string.IsNullOrWhiteSpace(filtros.Nome))
            {
                queryBase += $@" AND LOWER(TRIM(c.NOME)) LIKE LOWER('%{filtros.Nome.Trim()}%')";
            }
            if (!string.IsNullOrWhiteSpace(filtros.Fixo))
            {
                queryBase += $@" AND LOWER(TRIM(c.FIXO)) LIKE LOWER('%{filtros.Fixo.Trim()}%')";
            }
            if (!string.IsNullOrWhiteSpace(filtros.Celular))
            {
                queryBase += $@" AND LOWER(TRIM(c.CELULAR)) LIKE LOWER('%{filtros.Celular.Trim()}%')";
            }
            if (!string.IsNullOrWhiteSpace(filtros.Comercial))
            {
                queryBase += $@" AND LOWER(TRIM(c.COMERCIAL)) LIKE LOWER('%{filtros.Comercial.Trim()}%')";
            }
            if (!string.IsNullOrWhiteSpace(filtros.Endereco))
            {
                queryBase += $@" AND LOWER(TRIM(c.ENDERECO)) LIKE LOWER('%{filtros.Endereco.Trim()}%')";
            }
            if (!string.IsNullOrWhiteSpace(filtros.Email))
            {
                queryBase += $@" AND LOWER(TRIM(c.EMAIL)) LIKE LOWER('%{filtros.Email.Trim()}%')";
            }

            queryBase += " ORDER BY c.NOME";

            using (var conn = _context.CreateConnection())
            {
                conn.Open();


                var totalRegistros = (await conn.QueryAsync<Contato>(queryBase)).ToList().Count();
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                var sb = new StringBuilder();
                var contatos = (await conn.QueryAsync<Contato>(queryBase)).ToList();

                foreach (var c in contatos)
                {
                    sb.Append($"<tr style='height:60px'>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Nome}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-fixo input-sem-borda' value='{c.Fixo}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-celular input-sem-borda' value='{c.Celular}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-comercial input-sem-borda' value='{c.Comercial}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><a href='mailto:{c.Email}'>{c.Email}</a></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.CriadoAs}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.EditadoAs}</td>");

                    sb.Append($"<td style='width:50px; height:60px' class='text-nowrap'>" +
                        $"<a href='/Contato/Edit/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'><i class='bi-pencil-square text-dark'></i></a></td>");
                    sb.Append($"<td style='width:50px; height:60px' class='text-nowrap'>" +
                        $"<a href='/Contato/Details/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'><i class='bi bi-eye text-dark'></i></a></td>");
                    sb.Append($"<td style='width:50px; height:60px' class='text-nowrap'>" +
                        $"<a href='#' class='btn-abrir-modal-exclusao' data-id='{c.Id}' data-nome='{System.Net.WebUtility.HtmlEncode(c.Nome)}' data-bs-toggle='modal' data-bs-target='#modalConfirmarExclusao' title='Deletar'>" +
                        $"<i class='bi bi-trash text-dark'></i></a></td>");
                    sb.Append("</tr>");
                }

                //return Content(sb.ToString(), "text/html");
                return Json(new
                {
                    html = sb.ToString(), // Pode ser vazio
                    encontrou = contatos.Any(),
                    paginaAtual,
                    totalPaginas
                });
            }
        }


        // GET: ContatoController
        public async Task<IActionResult> Index(int paginaAtual = 1)
        {

            using (var conn = _context.CreateConnection())
            {
                conn.Open();

                var totalRegistros = await conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM LISTA_CONTATOS");
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                offset = (paginaAtual - 1) * registrosPorPagina;
                string ordenacao = "NOME"; // default order by NOME
                string query = @$"
                            SELECT 
                                c.ID AS Id,
                                c.NOME AS Nome,
                                c.FIXO AS Fixo,
                                c.CELULAR AS Celular,
                                c.COMERCIAL AS Comercial,
                                c.ENDERECO AS Endereco,
                                c.EMAIL AS Email,
                                TO_CHAR(c.CRIADO_AS, 'DD/MM/YYYY HH24:MI:SS')  AS CriadoAs,
                                TO_CHAR(c.EDITADO_AS, 'DD/MM/YYYY HH24:MI:SS') AS EditadoAs                             
                            FROM LISTA_CONTATOS c
                            ORDER BY {ordenacao}
                            OFFSET {offset} ROWS FETCH NEXT {registrosPorPagina} ROWS ONLY";


                var contatos = (await conn.QueryAsync<Contato>(query)).ToList();

                ContatoIndexViewModel model = new ContatoIndexViewModel();
                ViewBag.PaginaAtual = paginaAtual;
                ViewBag.TotalPaginas = totalPaginas;

                foreach (var contato in contatos)
                {
                    model.Contatos.Add(ContatoMapper.MapToViewModel(contato));
                }

                return View(model);
            }

        }


        // GET: ContatoController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var query = @$"SELECT * FROM LISTA_CONTATOS WHERE ID = {id}";
            using (var conn = _context.CreateConnection())
            {
                conn.Open();
                var model = await conn.QuerySingleAsync<ContatoDetailsViewModel>(query);

                return View(model);
            }
        }
        // GET: ContatoController/Create
        public IActionResult Create()
        {

            return View();

        }

        // POST: ContatoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Contato/Create")]
        public async Task<IActionResult> Create(Contato model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var conn = _context.CreateConnection();
            conn.Open();

            // Verifica FIXO
            if (!string.IsNullOrWhiteSpace(model.Fixo))
            {
                var query = $"SELECT COUNT(*) FROM LISTA_CONTATOS WHERE FIXO = '{model.Fixo}'";
                var existeFixo = await conn.QuerySingleAsync<int>(query);
                if (existeFixo > 0)
                    return Conflict(new { sucesso = false, mensagem = "Já existe um contato com esse FIXO." });
            }

            // Verifica EMAIL
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var query = $"SELECT COUNT(*) FROM LISTA_CONTATOS WHERE EMAIL = '{model.Email}'";
                var existeEmail = await conn.QuerySingleAsync<int>(query);
                if (existeEmail > 0)
                    return Conflict(new { sucesso = false, mensagem = "Já existe um contato com esse EMAIL." });
            }


            // Insere o novo contato
            string queryInserir = $@"
                INSERT INTO LISTA_CONTATOS (NOME, FIXO, CELULAR, COMERCIAL, ENDERECO, EMAIL)
                VALUES ('{model.Nome}', '{model.Fixo}', '{model.Celular}', '{model.Comercial}', '{model.Endereco}', '{model.Email}')";

            await conn.ExecuteAsync(queryInserir);

            return Ok(new { sucesso = true });
        }

        // GET: ContatoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var query = $@"SELECT * FROM LISTA_CONTATOS WHERE ID = {id}";
            using var conn = _context.CreateConnection();
            conn.Open();
            var model = await conn.QuerySingleAsync<ContatoEditViewModel>(query);

            return View(model);
        }

        // POST: ContatoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Contato model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var conn = _context.CreateConnection();
            conn.Open();

            // Buscar contato atual
            var queryBusca = $"SELECT NOME, FIXO, CELULAR, ENDERECO, EMAIL FROM LISTA_CONTATOS WHERE ID = {id}";
            var contatoAtual = await conn.QuerySingleAsync<ContatoEditViewModel>(queryBusca);

            if (contatoAtual == null)
                return NotFound();

            // Verificar se valores mudaram
            bool alterado =
                !string.Equals(model.Nome?.Trim(), contatoAtual.Nome?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(model.Fixo?.Trim(), contatoAtual.Fixo?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(model.Celular?.Trim(), contatoAtual.Celular?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(model.Endereco?.Trim(), contatoAtual.Endereco?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(model.Email?.Trim(), contatoAtual.Email?.Trim(), StringComparison.OrdinalIgnoreCase);

            if (!alterado)
                return NoContent();



            // Verificar duplicidade

            if (!string.IsNullOrWhiteSpace(model.Fixo))
            {
                var query = $"SELECT COUNT(*) FROM LISTA_CONTATOS WHERE FIXO = '{model.Fixo}' AND ID != {id}";
                var existeFixo = await conn.QuerySingleAsync<int>(query);
                if (existeFixo > 0)
                    return Conflict(new { sucesso = false, mensagem = "Já existe um contato com esse FIXO." });
            }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var query = $"SELECT COUNT(*) FROM LISTA_CONTATOS WHERE EMAIL = '{model.Email}' AND ID != {id}";
                var existeEmail = await conn.QuerySingleAsync<int>(query);
                if (existeEmail > 0)
                    return Conflict(new { sucesso = false, mensagem = "Já existe um contato com esse EMAIL." });
            }


            var setClausulas = new List<string>();

            try
            {
                if (!string.IsNullOrWhiteSpace(model.Nome))
                {
                    setClausulas.Add(@$"NOME = '{model.Nome}'");
                }

                if (!string.IsNullOrWhiteSpace(model.Fixo))
                {
                    setClausulas.Add(@$"FIXO = '{model.Fixo}'");
                }
                if (!string.IsNullOrWhiteSpace(model.Celular))
                {
                    setClausulas.Add(@$"CELULAR = '{model.Celular}'");
                }
                if (!string.IsNullOrWhiteSpace(model.Endereco))
                {
                    setClausulas.Add(@$"Endereco = '{model.Endereco}'");
                }
                if (!string.IsNullOrWhiteSpace(model.Email))
                {
                    setClausulas.Add(@$"Email = '{model.Email}'");
                }

                if (setClausulas.Any())
                {
                    var query = @$"UPDATE LISTA_CONTATOS SET {string.Join(", ", setClausulas)} WHERE ID = {id}";
                    await conn.ExecuteAsync(query);
                    return Ok(new { sucesso = true });
                }


            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return NoContent();
        }

        // GET: ContatoController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        // POST: ContatoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteContato(int id)
        {
            try
            {
                var query = $@"DELETE FROM LISTA_CONTATOS WHERE ID = {id}";
                using var conn = _context.CreateConnection();
                conn.Open();

                var rows = await conn.ExecuteAsync(query);

                if (rows == 0)
                    return NotFound(new { sucesso = false, mensagem = "Contato não encontrado." });

                return Ok(new { sucesso = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> ObterTabela(int totalPaginas, int paginaAtual = 1, int registrosPorPagina = 10)
        {
            offset = (paginaAtual - 1) * registrosPorPagina;
            string ordenacao = "NOME"; // default order by NOME
            string query = @$"SELECT 
                                c.ID AS Id,
                                c.NOME AS Nome,
                                c.FIXO AS Fixo,
                                c.CELULAR AS Celular,
                                c.COMERCIAL AS Comercial,
                                c.ENDERECO AS Endereco,
                                c.EMAIL AS Email,
                                TO_CHAR(c.CRIADO_AS, 'DD/MM/YYYY HH24:MI:SS')  AS CriadoAs,
                                TO_CHAR(c.EDITADO_AS, 'DD/MM/YYYY HH24:MI:SS') AS EditadoAs
                            FROM LISTA_CONTATOS c
                            ORDER BY {ordenacao}
                            OFFSET {offset} ROWS FETCH NEXT {registrosPorPagina} ROWS ONLY";


            using (var conn = _context.CreateConnection())
            {
                conn.Open();



                var totalRegistros = await conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM LISTA_CONTATOS");
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);




                StringBuilder sb = new StringBuilder();
                var contatos = (await conn.QueryAsync<Contato>(query)).ToList();


                foreach (var c in contatos)
                {
                    sb.Append($"<tr style='height:60px'>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Nome}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-fixo input-sem-borda' value='{c.Fixo}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-celular input-sem-borda' value='{c.Celular}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-comercial input-sem-borda' value='{c.Comercial}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><a href='mailto:{c.Email}'>{c.Email}</a></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.CriadoAs}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.EditadoAs}</td>");
                    ;
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Edit/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'><i class='bi-pencil-square text-dark'></i></a></td>");
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Details/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'><i class='bi bi-eye text-dark'></i></a></td>");
                    sb.Append($"<td style='width:50px; height:60px' class='text-nowrap'>" +
                         $"<a href='#' class='btn-abrir-modal-exclusao' data-id='{c.Id}' data-nome='{System.Net.WebUtility.HtmlEncode(c.Nome)}' data-bs-toggle='modal' data-bs-target='#modalConfirmarExclusao' title='Deletar'>" +
                         $"<i class='bi bi-trash text-dark'></i></a></td>");
                    sb.Append("</tr>");

                }

                return Json(new
                {
                    html = sb.ToString(),
                    paginaAtual,
                    totalPaginas,
                    sucesso = true
                });
            }
        }
    }
}
