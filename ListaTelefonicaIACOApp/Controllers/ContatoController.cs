using Dapper;
using ListaTelefonicaIACOApp.Constantes;
using ListaTelefonicaIACOApp.Infrastructure;
using ListaTelefonicaIACOApp.Mappers;
using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.ViewModels;
using ListaTelefonicaIACOApp.ViewModels.Contato;
using ListaTelefonicaIACOApp.ViewModels.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using System.Text;


namespace ListaTelefonicaIACOApp.Controllers
{
    public class ContatoController : Controller
    {
        private ILogger<ContatoController> _logger;
        private IConfiguration? _configuration;
        private ListaTelefonicaDbContext _context;
        public ContatoIndexViewModel contato = new ContatoIndexViewModel();
        int registrosPorPagina = 10;
        int totalPaginas;
        int offset;

        public ContatoController(ILogger<ContatoController> logger,
                                 IConfiguration configuration,
                                 ListaTelefonicaDbContext context
                                   )
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        [AllowAnonymous]
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

                    // Mostrar botão Editar e Deletar somente se o usuário tiver um dos papéis
                    if (User.IsInRole(Roles.Administrador) || User.IsInRole(Roles.Recepcao) || User.IsInRole(Roles.Guarita))
                    {
                        sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                                  $"<a href='/Contato/Edit/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'>" +
                                  $"<i class='bi-pencil-square text-dark'></i></a></td>");

                        sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                                  $"<a href='/Contato/Details/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'>" +
                                  $"<i class='bi bi-eye text-dark'></i></a></td>");

                        sb.Append($"<td style='width:50px; height:60px' class='text-nowrap'>" +
                                  $"<a href='#' class='btn-abrir-modal-exclusao' data-id='{c.Id}' data-nome='{System.Net.WebUtility.HtmlEncode(c.Nome)}' data-bs-toggle='modal' data-bs-target='#modalConfirmarExclusao' title='Deletar'>" +
                                  $"<i class='bi bi-trash text-dark'></i></a></td>");
                    }
                    else if (!User.IsInRole("admin") && !User.IsInRole("recepcao") && !User.IsInRole("guarita"))
                    {
                        // Usuário sem permissão: mostrar apenas botão de detalhes (se quiser)
                        sb.Append($"<td colspan='3' style='height: 50px' class='text-nowrap'>" +
                                  $"<a href='/Contato/Details/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'>" +
                                  $"<i class='bi bi-eye text-dark'></i></a></td>");
                    }
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

        [AllowAnonymous]
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

                    // Mostrar botão Editar e Deletar somente se o usuário tiver um dos papéis
                    if (User.IsInRole(Roles.Administrador) || User.IsInRole(Roles.Recepcao) || User.IsInRole(Roles.Guarita))
                    {
                        sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                                  $"<a href='/Contato/Edit/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'>" +
                                  $"<i class='bi-pencil-square text-dark'></i></a></td>");

                        sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                                  $"<a href='/Contato/Details/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'>" +
                                  $"<i class='bi bi-eye text-dark'></i></a></td>");

                        sb.Append($"<td style='width:50px; height:60px' class='text-nowrap'>" +
                                  $"<a href='#' class='btn-abrir-modal-exclusao' data-id='{c.Id}' data-nome='{System.Net.WebUtility.HtmlEncode(c.Nome)}' data-bs-toggle='modal' data-bs-target='#modalConfirmarExclusao' title='Deletar'>" +
                                  $"<i class='bi bi-trash text-dark'></i></a></td>");
                    }
                    else if (!User.IsInRole(Roles.Administrador) && !User.IsInRole(Roles.Recepcao) && !User.IsInRole(Roles.Guarita))
                    {
                        // Usuário sem permissão: mostrar apenas botão de detalhes (se quiser)
                        sb.Append($"<td colspan='3' style='height: 50px' class='text-nowrap'>" +
                                  $"<a href='/Contato/Details/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'>" +
                                  $"<i class='bi bi-eye text-dark'></i></a></td>");
                    }
                    sb.Append("</tr>");
                }

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
        [AllowAnonymous]
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
        [AllowAnonymous]
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

        [Authorize(Roles = $"{Roles.Administrador},{Roles.Recepcao},{Roles.Guarita}")]
        // GET: ContatoController/Create
        public IActionResult Create()
        {

            return View();

        }

        // POST: ContatoController/Create
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Recepcao},{Roles.Guarita}")]
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


            var claimValor = User.FindFirst("UserId")?.Value;
            var ip = HttpContext.Connection.RemoteIpAddress;
            string ipFormatado = ip?.IsIPv4MappedToIPv6 == true
                    ? ip.MapToIPv4().ToString()
                    : ip?.ToString();


            var log = new LogsAcoesUsuario
            {
                UsuarioId = Convert.ToInt32(claimValor),
                Acao = AcoesContato.CRIAR,
                DataHoraAcao = DateTime.Now,
                EnderecoIp = ipFormatado,
                UserAgent = Request.Headers["User-Agent"].ToString(),
                DetalhesRegistroAfetado = JsonConvert.SerializeObject(model),
                MensagemErro = null
            };

            try
            {
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

                log.Sucesso = ResultadoAcao.Sucesso;
                await GravarLog(log, conn);



                return Ok(new { sucesso = true });
            }
            catch (Exception ex)
            {
                log.Sucesso = ResultadoAcao.Falha;
                log.MensagemErro = ex.Message;
                await GravarLog(log, conn);
                return StatusCode(500, new { sucesso = false, mensagem = "Erro interno ao criar o contato." });
            }


        }



        private async Task GravarLog(LogsAcoesUsuario log, IDbConnection conn)
        {
            string insertLog = @$"
                INSERT INTO LogsAcoesUsuario (
                    UsuarioId,
                    Acao,
                    DataHoraAcao,
                    EnderecoIp,
                    UserAgent,
                    DetalhesRegistroAfetado,
                    Sucesso,
                    MensagemErro
                ) VALUES (
                    '{log.UsuarioId}',
                    '{log.Acao}',
                    TO_TIMESTAMP('{log.DataHoraAcao:yyyy-MM-dd HH:mm:ss}', 'YYYY-MM-DD HH24:MI:SS'),
                    '{log.EnderecoIp}',
                    '{log.UserAgent}',
                    '{log.DetalhesRegistroAfetado.Replace("'", "''")}',
                    '{(int)log.Sucesso}',
                    {(log.MensagemErro != null ? $"'{log.MensagemErro.Replace("'", "''")}'" : "NULL")}
                )";

            await conn.ExecuteAsync(insertLog);
        }



        // GET: ContatoController/Edit/5
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Recepcao},{Roles.Guarita}")]
        public async Task<ActionResult> Edit(int id)
        {
            var query = $@"SELECT * FROM LISTA_CONTATOS WHERE ID = {id}";
            using var conn = _context.CreateConnection();
            conn.Open();
            var model = await conn.QuerySingleAsync<ContatoEditViewModel>(query);

            return View(model);
        }

        // POST: ContatoController/Edit/5
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Recepcao},{Roles.Guarita}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Contato contatoAtualizado)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var conn = _context.CreateConnection();
            conn.Open();

            // Buscar contato atual
            var queryBusca = $"SELECT NOME, FIXO, CELULAR, ENDERECO, EMAIL FROM LISTA_CONTATOS WHERE ID = {id}";

            var contatoAtual = await conn.QuerySingleAsync<ContatoEditViewModel>(queryBusca);
            var claimValor = User.FindFirst("UserId")?.Value;
            var ip = HttpContext.Connection.RemoteIpAddress;
            string ipFormatado = ip?.IsIPv4MappedToIPv6 == true
                    ? ip.MapToIPv4().ToString()
                    : ip?.ToString();

            var log = new LogsAcoesUsuario
            {
                UsuarioId = Convert.ToInt32(claimValor),
                Acao = AcoesContato.EDITAR, // Supondo que você tenha essa constante
                DataHoraAcao = DateTime.Now,
                EnderecoIp = ipFormatado,
                UserAgent = Request.Headers["User-Agent"].ToString(),
                DetalhesRegistroAfetado = JsonConvert.SerializeObject(new
                {
                    Antes = contatoAtual,
                    Depois = contatoAtualizado
                }),
                Sucesso = ResultadoAcao.Sucesso,
                MensagemErro = null
            };


            if (contatoAtual == null)
                return NotFound();

            // Verificar se valores mudaram
            bool alterado =
                !string.Equals(contatoAtualizado.Nome?.Trim(), contatoAtual.Nome?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(contatoAtualizado.Fixo?.Trim(), contatoAtual.Fixo?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(contatoAtualizado.Celular?.Trim(), contatoAtual.Celular?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(contatoAtualizado.Endereco?.Trim(), contatoAtual.Endereco?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(contatoAtualizado.Email?.Trim(), contatoAtual.Email?.Trim(), StringComparison.OrdinalIgnoreCase);

            if (!alterado)
                return NoContent();



            // Verificar duplicidade

            if (!string.IsNullOrWhiteSpace(contatoAtualizado.Fixo))
            {
                var query = $"SELECT COUNT(*) FROM LISTA_CONTATOS WHERE FIXO = '{contatoAtualizado.Fixo}' AND ID != {id}";
                var existeFixo = await conn.QuerySingleAsync<int>(query);
                if (existeFixo > 0)
                    return Conflict(new { sucesso = false, mensagem = "Já existe um contato com esse FIXO." });
            }

            if (!string.IsNullOrWhiteSpace(contatoAtualizado.Email))
            {
                var query = $"SELECT COUNT(*) FROM LISTA_CONTATOS WHERE EMAIL = '{contatoAtualizado.Email}' AND ID != {id}";
                var existeEmail = await conn.QuerySingleAsync<int>(query);
                if (existeEmail > 0)
                    return Conflict(new { sucesso = false, mensagem = "Já existe um contato com esse EMAIL." });
            }


            var setClausulas = new List<string>();

            try
            {
                if (!string.IsNullOrWhiteSpace(contatoAtualizado.Nome))
                {
                    setClausulas.Add(@$"NOME = '{contatoAtualizado.Nome}'");
                }

                if (!string.IsNullOrWhiteSpace(contatoAtualizado.Fixo))
                {
                    setClausulas.Add(@$"FIXO = '{contatoAtualizado.Fixo}'");
                }
                if (!string.IsNullOrWhiteSpace(contatoAtualizado.Celular))
                {
                    setClausulas.Add(@$"CELULAR = '{contatoAtualizado.Celular}'");
                }
                if (!string.IsNullOrWhiteSpace(contatoAtualizado.Endereco))
                {
                    setClausulas.Add(@$"ENDERECO = '{contatoAtualizado.Endereco}'");
                }
                if (!string.IsNullOrWhiteSpace(contatoAtualizado.Email))
                {
                    setClausulas.Add(@$"EMAIL = '{contatoAtualizado.Email}'");
                }

                setClausulas.Add(@$"EDITADO_AS = TO_DATE('{DateTime.Now:yyyy-MM-dd HH:mm:ss}', 'YYYY-MM-DD HH24:MI:SS')");

                if (setClausulas.Any())
                {
                    var query = @$"UPDATE LISTA_CONTATOS SET {string.Join(", ", setClausulas)} WHERE ID = {id}";
                    await conn.ExecuteAsync(query);
                    log.Sucesso = ResultadoAcao.Sucesso;
                    await GravarLog(log, conn);
                    return Ok(new { sucesso = true });
                }


            }
            catch (Exception ex)
            {
                log.Sucesso = ResultadoAcao.Falha;
                log.MensagemErro = ex.Message;
                await GravarLog(log, conn);
                return BadRequest(new { Mensagem = ex.Message });
            }

            return NoContent();
        }


        // POST: ContatoController/Delete/5
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Recepcao},{Roles.Guarita}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteContato(int id)
        {

            // Buscar contato a ser deletado
            var queryBuscaContatoPorId = $"SELECT * FROM LISTA_CONTATOS WHERE ID = {id}";

            // Deletar contato por Id
            var queryDeletaContatoPorId = $@"DELETE FROM LISTA_CONTATOS WHERE ID = {id}";

            var claimValor = User.FindFirst("UserId")?.Value;
            var ip = HttpContext.Connection.RemoteIpAddress;
            string ipFormatado = ip?.IsIPv4MappedToIPv6 == true
                    ? ip.MapToIPv4().ToString()
                    : ip?.ToString();


            var log = new LogsAcoesUsuario
            {
                UsuarioId = Convert.ToInt32(claimValor),
                Acao = AcoesContato.DELETAR,
                DataHoraAcao = DateTime.Now,
                EnderecoIp = ipFormatado,
                UserAgent = Request.Headers["User-Agent"].ToString(),
                MensagemErro = null
            };

            using var conn = _context.CreateConnection();
            conn.Open();

            try
            {


                var contatoAserDeletado = await conn.QuerySingleAsync<ContatoEditViewModel>(queryBuscaContatoPorId);

                log.DetalhesRegistroAfetado = JsonConvert.SerializeObject(contatoAserDeletado);

                var rows = await conn.ExecuteAsync(queryDeletaContatoPorId);

                if (rows == 0)
                    return NotFound(new { sucesso = false, mensagem = "Contato não encontrado." });

                log.Sucesso = ResultadoAcao.Sucesso;
                await GravarLog(log, conn);
                return Ok(new { sucesso = true });
            }
            catch (Exception ex)
            {
                log.Sucesso = ResultadoAcao.Falha;
                log.MensagemErro = ex.Message;
                await GravarLog(log, conn);
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

                    // Mostrar botão Editar e Deletar somente se o usuário tiver um dos papéis
                    if (User.IsInRole(Roles.Administrador) || User.IsInRole(Roles.Recepcao) || User.IsInRole(Roles.Guarita))
                    {
                        sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                                  $"<a href='/Contato/Edit/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'>" +
                                  $"<i class='bi-pencil-square text-dark'></i></a></td>");

                        sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                                  $"<a href='/Contato/Details/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'>" +
                                  $"<i class='bi bi-eye text-dark'></i></a></td>");

                        sb.Append($"<td style='width:50px; height:60px' class='text-nowrap'>" +
                                  $"<a href='#' class='btn-abrir-modal-exclusao' data-id='{c.Id}' data-nome='{System.Net.WebUtility.HtmlEncode(c.Nome)}' data-bs-toggle='modal' data-bs-target='#modalConfirmarExclusao' title='Deletar'>" +
                                  $"<i class='bi bi-trash text-dark'></i></a></td>");
                    }
                    else if (!User.IsInRole(Roles.Administrador) && !User.IsInRole(Roles.Recepcao) && !User.IsInRole(Roles.Guarita))
                    {
                        // Usuário sem permissão: mostrar apenas botão de detalhes (se quiser)
                        sb.Append($"<td colspan='3' style='height: 50px' class='text-nowrap'>" +
                                  $"<a href='/Contato/Details/{c.Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'>" +
                                  $"<i class='bi bi-eye text-dark'></i></a></td>");
                    }
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
