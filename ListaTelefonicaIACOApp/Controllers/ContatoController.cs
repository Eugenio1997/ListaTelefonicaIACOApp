using Dapper;
using ListaTelefonicaIACOApp.Infrastructure;
using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Linq;
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
                                TO_CHAR(c.CRIADO_AS, 'DD/MM/YYYY HH24:MI:SS')  AS Contato_CriadoAs,
                                TO_CHAR(c.EDITADO_AS, 'DD/MM/YYYY HH24:MI:SS') AS Contato_EditadoAs,

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


            using (var conn = _context.CreateConnection())
            {
                conn.Open();



                var totalRegistros = await conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM LISTA_CONTATOS");
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                var contatos = await conn.QueryAsync<ContatoIndexViewModel, EnderecoViewModel, ContatoIndexViewModel>(
                    query,
                    (contato, endereco) =>
                    {
                        contato.Endereco = endereco;
                        return contato;
                    },
                    splitOn: "Endereco_Id"
                );


                StringBuilder sb = new StringBuilder();

                foreach (var c in contatos)
                {
                    sb.Append($"<tr style='height:60px'>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_Nome}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_Sobrenome}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-fixo input-sem-borda' value='{c.Contato_Fixo}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-celular input-sem-borda' value='{c.Contato_Celular}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-comercial input-sem-borda' value='{c.Contato_Comercial}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><a href='mailto:{c.Contato_Email}'>{c.Contato_Email}</a></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_CriadoAs}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_EditadoAs}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Rua}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Numero}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Bairro}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Cidade}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Complemento}</td>");
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Edit/{c.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'><i class='bi-pencil-square text-dark'></i></a></td>");
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Details/{c.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'><i class='bi bi-eye text-dark'></i></a></td>");
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Delete/{c.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Deletar'><i class='bi bi-trash text-dark'></i></a></td>");
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
        public async Task<IActionResult> ObterContatosFiltrados(FiltroViewModel filtros)
        {
            string queryBase = @"
                    SELECT 
                        c.ID              AS Contato_Id,
                        c.NOME            AS Contato_Nome,
                        c.SOBRENOME       AS Contato_Sobrenome,
                        c.FIXO            AS Contato_Fixo,
                        c.CELULAR         AS Contato_Celular,
                        c.COMERCIAL       AS Contato_Comercial,
                        c.ENDERECO_ID     AS Contato_EnderecoId,
                        c.EMAIL           AS Contato_Email,
                        TO_CHAR(c.CRIADO_AS, 'DD/MM/YYYY HH24:MI:SS')  AS Contato_CriadoAs,
                        TO_CHAR(c.EDITADO_AS, 'DD/MM/YYYY HH24:MI:SS') AS Contato_EditadoAs,

                        e.ID           AS Endereco_Id,
                        e.CEP          AS Endereco_CEP,
                        e.RUA          AS Endereco_Rua,
                        e.NUMERO       AS Endereco_Numero,
                        e.BAIRRO       AS Endereco_Bairro,
                        e.CIDADE       AS Endereco_Cidade,
                        e.COMPLEMENTO  AS Endereco_Complemento

                    FROM LISTA_CONTATOS c
                    LEFT JOIN LISTA_ENDERECOS e ON c.ENDERECO_ID = e.ID
                    WHERE 1=1
                ";



            if (!string.IsNullOrWhiteSpace(filtros.Nome))
            {
                queryBase += $@" AND LOWER(TRIM(c.NOME)) LIKE LOWER('%{filtros.Nome.Trim()}%')";
            }
            if (!string.IsNullOrWhiteSpace(filtros.Sobrenome))
            {
                queryBase += $@" AND LOWER(TRIM(c.SOBRENOME)) LIKE LOWER('%{filtros.Sobrenome.Trim()}%')";
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
            if (!string.IsNullOrWhiteSpace(filtros.Email))
            {
                queryBase += $@" AND LOWER(TRIM(c.EMAIL)) LIKE LOWER('%{filtros.Email.Trim()}%')";
            }
            if (!string.IsNullOrWhiteSpace(filtros.Rua))
            {
                queryBase += $@" AND LOWER(TRIM(e.RUA)) LIKE LOWER('%{filtros.Rua.Trim()}%')";
            }
            if (!string.IsNullOrWhiteSpace(filtros.Bairro))
            {
                queryBase += $@" AND LOWER(TRIM(e.BAIRRO)) LIKE LOWER('%{filtros.Bairro.Trim()}%')";
            }
            if (!string.IsNullOrWhiteSpace(filtros.Cidade))
            {
                queryBase += $@" AND LOWER(TRIM(e.CIDADE)) LIKE LOWER('%{filtros.Cidade.Trim()}%')";
            }




            queryBase += " ORDER BY c.NOME";

            using (var conn = _context.CreateConnection())
            {
                conn.Open();



                var contatos = await conn.QueryAsync<ContatoIndexViewModel, EnderecoViewModel, ContatoIndexViewModel>(
                    queryBase,
                     (contato, endereco) =>
                     {
                         contato.Endereco = endereco;
                         return contato;
                     },
                    splitOn: "Endereco_Id"
                );

                var sb = new StringBuilder();

                foreach (var c in contatos)
                {
                    sb.Append($"<tr style='height:60px'>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_Nome}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_Sobrenome}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-fixo input-sem-borda' value='{c.Contato_Fixo}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-celular input-sem-borda' value='{c.Contato_Celular}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-comercial input-sem-borda' value='{c.Contato_Comercial}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><a href='mailto:{c.Contato_Email}'>{c.Contato_Email}</a></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_CriadoAs}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_EditadoAs}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Rua}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Numero}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Bairro}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Cidade}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Complemento}</td>");
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Edit/{c.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'><i class='bi-pencil-square text-dark'></i></a></td>");
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Details/{c.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'><i class='bi bi-eye text-dark'></i></a></td>");
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Delete/{c.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Deletar'><i class='bi bi-trash text-dark'></i></a></td>");
                    sb.Append("</tr>");
                }

                //return Content(sb.ToString(), "text/html");
                return Json(new
                {
                    html = sb.ToString(), // Pode ser vazio
                    encontrou = contatos.Any()
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
                                c.ID AS Contato_Id,
                                c.NOME AS Contato_Nome,
                                c.SOBRENOME AS Contato_Sobrenome,
                                c.FIXO AS Contato_Fixo,
                                c.CELULAR AS Contato_Celular,
                                c.COMERCIAL AS Contato_Comercial,
                                c.ENDERECO_ID AS Contato_EnderecoId,
                                c.EMAIL AS Contato_Email,
                                TO_CHAR(c.CRIADO_AS, 'DD/MM/YYYY HH24:MI:SS')  AS Contato_CriadoAs,
                                TO_CHAR(c.EDITADO_AS, 'DD/MM/YYYY HH24:MI:SS') AS Contato_EditadoAs,                                

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

                StringBuilder sb = new StringBuilder();



                var contatos = await conn.QueryAsync<ContatoIndexViewModel, EnderecoViewModel, ContatoIndexViewModel>(
                    query,
                    (contato, endereco) =>
                    {
                        contato.Endereco = endereco;
                        return contato;
                    },
                    splitOn: "Endereco_Id"
                );


                foreach (var c in contatos)
                {
                    sb.Append($"<tr style='height:60px'>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_Nome}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_Sobrenome}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-fixo input-sem-borda' value='{c.Contato_Fixo}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-celular input-sem-borda' value='{c.Contato_Celular}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><input type='text' class='form-control telefone-comercial input-sem-borda' value='{c.Contato_Comercial}' /></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'><a href='mailto:{c.Contato_Email}'>{c.Contato_Email}</a></td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_CriadoAs}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Contato_EditadoAs}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Rua}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Numero}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Bairro}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Cidade}</td>");
                    sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{c.Endereco.Endereco_Complemento}</td>");
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Edit/{c.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Editar'><i class='bi-pencil-square text-dark'></i></a></td>");
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Details/{c.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Ver Detalhes'><i class='bi bi-eye text-dark'></i></a></td>");
                    sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                        $"<a href='/Contato/Delete/{c.Contato_Id}' data-bs-toggle='tooltip' data-bs-placement='top' title='Deletar'><i class='bi bi-trash text-dark'></i></a></td>");
                    sb.Append("</tr>");
                }

                var contatoIndexViewModel = new ContatoIndexViewModel
                {
                    Contatos = contatos.ToList()
                };
                ViewBag.PaginaAtual = paginaAtual;
                ViewBag.TotalPaginas = totalPaginas;



                return View(contatoIndexViewModel);
            }

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
        [Route("Contato/Create")]
        public async Task<IActionResult> Create(ContatoCadastroResponseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna erros de validação
            }

            using var conn = _context.CreateConnection();
            conn.Open();

            // 1. Inserir endereço
            string queryEndereco = $@"
                INSERT INTO LISTA_ENDERECOS (RUA, NUMERO, BAIRRO, CIDADE, CEP, COMPLEMENTO)
                VALUES ('{model.Rua}', '{model.Numero}', '{model.Bairro}', '{model.Cidade}', '{model.CEP}', '{model.Complemento}')";

            await conn.ExecuteAsync(queryEndereco);

            // 2. Recuperar ID do endereço
            int enderecoId = await conn.ExecuteScalarAsync<int>("SELECT MAX(ID) FROM LISTA_ENDERECOS");

            // 3. Inserir contato
            string queryContato = $@"
                INSERT INTO LISTA_CONTATOS (NOME, SOBRENOME, FIXO, CELULAR, COMERCIAL, ENDERECO_ID, EMAIL)
                VALUES ('{model.Nome}', '{model.Sobrenome}', '{model.Fixo}', '{model.Celular}', '{model.Comercial}', {enderecoId}, '{model.Email}')";

            await conn.ExecuteAsync(queryContato);

            return Ok(new { sucesso = true });
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
