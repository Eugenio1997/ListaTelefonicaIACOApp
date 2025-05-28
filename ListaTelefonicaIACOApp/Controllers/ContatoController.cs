using Dapper;
using ListaTelefonicaIACOApp.Infrastructure;
using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
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
                    //sb.Append($"<td class='text-nowrap'>{contato.Id}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Contato_Nome}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Contato_Sobrenome}</td>");
                    sb.Append($"<td class='text-nowrap'><input type='text' class='form-control telefone-fixo input-sem-borda' value='{c.Contato_Fixo}' /></td>");
                    sb.Append($"<td class='text-nowrap'><input type='text' class='form-control telefone-celular input-sem-borda' value='{c.Contato_Celular}' /></td>");
                    sb.Append($"<td class='text-nowrap'><input type='text' class='form-control telefone-comercial input-sem-borda' value='{c.Contato_Comercial}' /></td>");
                    //sb.Append($"<td class='text-nowrap'>{contato.Endereco}</td>");
                    sb.Append($"<td class='text-nowrap'><a href='mailto:{c.Contato_Email}'>{c.Contato_Email}</a></td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Rua}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Numero}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Bairro}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Cidade}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Complemento}</td>");
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



            if (!string.IsNullOrEmpty(filtros.Nome))
            {
                queryBase += $@" AND LOWER(c.NOME) LIKE LOWER('{filtros.Nome}%')";

            }
            if (!string.IsNullOrEmpty(filtros.Sobrenome))
            {
                queryBase += $@" AND LOWER(c.SOBRENOME) LIKE LOWER('{filtros.Sobrenome}%')";
            }
            if (!string.IsNullOrEmpty(filtros.Fixo))
            {
                queryBase += $@" AND LOWER(c.FIXO) LIKE LOWER('{filtros.Fixo}%')";
            }
            if (!string.IsNullOrEmpty(filtros.Celular))
            {
                queryBase += $@" AND LOWER(c.CELULAR) LIKE LOWER('{filtros.Celular}%')";
            }
            if (!string.IsNullOrEmpty(filtros.Comercial))
            {
                queryBase += $@" AND LOWER(c.COMERCIAL) LIKE LOWER('{filtros.Comercial}%')";
            }
            if (!string.IsNullOrEmpty(filtros.Email))
            {
                queryBase += $@" AND LOWER(c.EMAIL) LIKE LOWER('{filtros.Email}%')";
            }
            if (!string.IsNullOrEmpty(filtros.Rua))
            {
                queryBase += $@" AND LOWER(e.RUA) LIKE LOWER('{filtros.Rua}%')";
            }
            if (!string.IsNullOrEmpty(filtros.Bairro))
            {
                queryBase += $@" AND LOWER(e.BAIRRO) LIKE LOWER('{filtros.Bairro}%')";
            }
            if (!string.IsNullOrEmpty(filtros.Cidade))
            {
                queryBase += $@" AND LOWER(e.CIDADE) LIKE LOWER('{filtros.Cidade}%')";
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
                    //sb.Append($"<td class='text-nowrap'>{contato.Id}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Contato_Nome}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Contato_Sobrenome}</td>");
                    sb.Append($"<td class='text-nowrap'><input type='text' class='form-control telefone-fixo input-sem-borda' value='{c.Contato_Fixo}' /></td>");
                    sb.Append($"<td class='text-nowrap'><input type='text' class='form-control telefone-celular input-sem-borda' value='{c.Contato_Celular}' /></td>");
                    sb.Append($"<td class='text-nowrap'><input type='text' class='form-control telefone-comercial input-sem-borda' value='{c.Contato_Comercial}' /></td>");
                    //sb.Append($"<td class='text-nowrap'>{contato.Endereco}</td>");
                    sb.Append($"<td class='text-nowrap'><a href='mailto:{c.Contato_Email}'>{c.Contato_Email}</a></td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Rua}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Numero}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Bairro}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Cidade}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Complemento}</td>");
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
                    //sb.Append($"<td class='text-nowrap'>{contato.Id}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Contato_Nome}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Contato_Sobrenome}</td>");
                    sb.Append($"<td class='text-nowrap'><input type='text' class='form-control telefone-fixo input-sem-borda' value='{c.Contato_Fixo}' /></td>");
                    sb.Append($"<td class='text-nowrap'><input type='text' class='form-control telefone-celular input-sem-borda' value='{c.Contato_Celular}' /></td>");
                    sb.Append($"<td class='text-nowrap'><input type='text' class='form-control telefone-comercial input-sem-borda' value='{c.Contato_Comercial}' /></td>");
                    //sb.Append($"<td class='text-nowrap'>{contato.Endereco}</td>");
                    sb.Append($"<td class='text-nowrap'><a href='mailto:{c.Contato_Email}'>{c.Contato_Email}</a></td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Rua}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Numero}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Bairro}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Cidade}</td>");
                    sb.Append($"<td class='text-nowrap'>{c.Endereco.Endereco_Complemento}</td>");
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
        public async Task<IActionResult> Create(ContatoCadastroViewModel contato)
        {

            string query = $@"
                    INSERT INTO LISTA_ENDERECOS (
                        RUA, NUMERO, BAIRRO, CIDADE, CEP, COMPLEMENTO, CRIADO_AS, EDITADO_AS
                    ) VALUES (
                        {contato.Rua}, {contato.Numero}, {contato.Numero}, {contato.Numero}, {contato.Numero}, {contato.Numero}, :Complemento, :CriadoAs, :EditadoAs
                    )";

            try
            {
                using (var conn = _context.CreateConnection())
                {



                    conn.Open();

                    await conn.ExecuteAsync()
                    //recebo um contato com um objeto complexo endereco
                    //1.Cadastrar o endereco
                    //2.Recuperar o ID do endereco cadastrado
                    //3.Cadastrar o Contato e passar como valor para a coluna Endereco_ID (chave estrangeira) o ID recuperado
                    //4. 

                }

            }
            catch
            {
                return View();
            }
            finally
            {

                RedirectToAction(nameof(Index));
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
