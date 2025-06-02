using Dapper;
using ListaTelefonicaIACOApp.Controllers;
using ListaTelefonicaIACOApp.Infrastructure;
using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.ViewModels.Endereco;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public class EnderecoController : Controller
{
    private readonly ILogger<ContatoController> _logger;
    private readonly IConfiguration? _configuration;
    private readonly ListaTelefonicaDbContext _context;
    public EnderecoIndexViewModel contato = new EnderecoIndexViewModel();
    int registrosPorPagina = 10;
    int totalRegistros;
    int totalPaginas;
    int offset;

    public EnderecoController(ILogger<ContatoController> logger,
                                 IConfiguration configuration,
                                 ListaTelefonicaDbContext context)
    {
        _logger = logger;
        _configuration = configuration;
        _context = context;
    }

    public async Task<IActionResult> ObterEnderecosPaginados(int registrosPorPagina = 10, int paginaAtual = 1)
    {
        int offset = (paginaAtual - 1) * registrosPorPagina;
        string ordenacao = "CRIADO_AS"; // Ordena por data de criação

        string query = @$"
        SELECT 
            e.ID AS Endereco_Id,
            e.RUA AS Endereco_Rua,
            e.NUMERO AS Endereco_Numero,
            e.BAIRRO AS Endereco_Bairro,
            e.CIDADE AS Endereco_Cidade,
            e.CEP AS Endereco_CEP,
            e.COMPLEMENTO AS Endereco_Complemento,
            TO_CHAR(e.CRIADO_AS, 'DD/MM/YYYY HH24:MI:SS') AS Endereco_CriadoAs,
            TO_CHAR(e.EDITADO_AS, 'DD/MM/YYYY HH24:MI:SS') AS Endereco_EditadoAs
        FROM LISTA_ENDERECOS e
        ORDER BY {ordenacao}
        OFFSET {offset} ROWS FETCH NEXT {registrosPorPagina} ROWS ONLY";

        using (var conn = _context.CreateConnection())
        {
            conn.Open();

            var totalRegistros = await conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM LISTA_ENDERECOS");
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            var enderecos = await conn.QueryAsync<EnderecoCreateViewModel>(query);

            StringBuilder sb = new StringBuilder();

            foreach (var e in enderecos)
            {
                sb.Append("<tr style='height:60px'>");
                sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{e.Endereco_Rua}</td>");
                sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{e.Endereco_Numero}</td>");
                sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{e.Endereco_Bairro}</td>");
                sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{e.Endereco_Cidade}</td>");
                sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{e.Endereco_CEP}</td>");
                sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{e.Endereco_Complemento}</td>");
                sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{e.Endereco_CriadoAs}</td>");
                sb.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{e.Endereco_EditadoAs}</td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                          $"<a href='/Endereco/Edit/{e.Endereco_Id}' title='Editar'><i class='bi bi-pencil-square text-dark'></i></a></td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                          $"<a href='/Endereco/Details/{e.Endereco_Id}' title='Ver Detalhes'><i class='bi bi-eye text-dark'></i></a></td>");
                sb.Append($"<td style='height: 50px' class='text-nowrap'>" +
                          $"<a href='/Endereco/Delete/{e.Endereco_Id}' title='Deletar'><i class='bi bi-trash text-dark'></i></a></td>");
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
    public IActionResult ObterEnderecosFiltrados(string? Rua, string? Bairro, string? Cidade)
    {
        string query = @"
        SELECT * 
        FROM LISTA_ENDERECOS
        WHERE 1 = 1";

        if (!string.IsNullOrWhiteSpace(Rua))
            query += $" AND LOWER(TRIM(RUA)) LIKE LOWER('%{Rua.Trim()}%')";

        if (!string.IsNullOrWhiteSpace(Bairro))
            query += $" AND LOWER(TRIM(BAIRRO)) LIKE LOWER('%{Bairro.Trim()}%')";

        if (!string.IsNullOrWhiteSpace(Cidade))
            query += $" AND LOWER(TRIM(CIDADE)) LIKE LOWER('%{Cidade.Trim()}%')";

        using (var conn = _context.CreateConnection())
        {
            conn.Open();

            var enderecos = conn.Query<ListaTelefonicaIACOApp.Models.Endereco>(query);

            if (!enderecos.Any())
            {
                return Json(new { encontrou = false });
            }

            var htmlBuilder = new StringBuilder();

            foreach (var item in enderecos)
            {
                htmlBuilder.Append("<tr style='height:60px'>");
                htmlBuilder.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{item.Rua}</td>");
                htmlBuilder.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{item.Numero}</td>");
                htmlBuilder.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{item.Bairro}</td>");
                htmlBuilder.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{item.Cidade}</td>");
                htmlBuilder.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{item.CEP}</td>");
                htmlBuilder.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{item.Complemento}</td>");
                htmlBuilder.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{item.CriadoAs}</td>");
                htmlBuilder.Append($"<td style='min-width:180px; min-height:60px' class='text-nowrap'>{item.EditadoAs}</td>");
                htmlBuilder.Append($"<td style='height: 50px' class='text-nowrap'><a href='/Endereco/Edit/{item.Id}'><i class='bi bi-pencil-square text-dark'></i></a></td>");
                htmlBuilder.Append($"<td style='height: 50px' class='text-nowrap'><a href='/Endereco/Details/{item.Id}'><i class='bi bi-eye text-dark'></i></a></td>");
                htmlBuilder.Append($"<td style='height: 50px' class='text-nowrap'><a href='/Endereco/Delete/{item.Id}'><i class='bi bi-trash text-dark'></i></a></td>");
                htmlBuilder.Append("</tr>");
            }

            return Json(new
            {
                encontrou = true,
                html = htmlBuilder.ToString()
            });
        }
    }


    public async Task<IActionResult> Index(int paginaAtual = 1)
    {
        using (var conn = _context.CreateConnection())
        {
            conn.Open();

            var totalRegistros = await conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM LISTA_ENDERECOS");
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            offset = (paginaAtual - 1) * registrosPorPagina;
            string ordenacao = "CRIADO_AS"; // default order by NOME

            string query = @$"
                            SELECT
                                e.ID AS Endereco_Id,
                                e.RUA AS Endereco_Rua,
                                e.NUMERO AS Endereco_Numero,
                                e.BAIRRO AS Endereco_Bairro,
                                e.CIDADE AS Endereco_Cidade,
                                e.CEP AS Endereco_CEP,
                                TO_CHAR(e.CRIADO_AS, 'DD/MM/YYYY HH24:MI:SS')  AS Endereco_CriadoAs,
                                TO_CHAR(e.EDITADO_AS, 'DD/MM/YYYY HH24:MI:SS') AS Endereco_EditadoAs
                            FROM LISTA_ENDERECOS e
                            ORDER BY {ordenacao}
                            OFFSET {offset} ROWS FETCH NEXT {registrosPorPagina} ROWS ONLY";

            var enderecos = await conn.QueryAsync<EnderecoIndexViewModel>(query);


            var enderecoIndexViewModel = new EnderecoIndexViewModel
            {
                Enderecos = enderecos.ToList()
            };

            ViewBag.PaginaAtual = paginaAtual;
            ViewBag.TotalPaginas = totalPaginas;

            return View(enderecoIndexViewModel);
        }
    }

    public IActionResult Create() => View();


    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EnderecoCreateViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(model);

        
        
        string inserirEnderecoQuery = $@"
                INSERT INTO LISTA_ENDERECOS 
                    (RUA, NUMERO, BAIRRO, CIDADE, CEP, COMPLEMENTO)
                VALUES 
                    ('{model.Endereco_Rua}', '{model.Endereco_Numero}', '{model.Endereco_Bairro}', 
                     '{model.Endereco_Cidade}', '{model.Endereco_CEP}', '{model.Endereco_Complemento}')";

        using var conn = _context.CreateConnection();
        conn.Open();

        // Verifica CELULAR
        if (!string.IsNullOrWhiteSpace(model.Endereco_Numero) &&
            !string.IsNullOrWhiteSpace(model.Endereco_Rua) &&
            !string.IsNullOrWhiteSpace(model.Endereco_Bairro) &&
            !string.IsNullOrWhiteSpace(model.Endereco_Cidade) &&
            !string.IsNullOrWhiteSpace(model.Endereco_CEP) &&
            !string.IsNullOrWhiteSpace(model.Endereco_Complemento)
           ) 
        {

            string verificarEnderecoExiste = @$"
                SELECT COUNT(*) FROM LISTA_ENDERECOS e
                WHERE
                    e.NUMERO = '{model.Endereco_Numero}' AND
                    e.RUA = '{model.Endereco_Rua}' AND
                    e.BAIRRO = '{model.Endereco_Bairro}' AND
                    e.CIDADE = '{model.Endereco_Cidade}' AND
                    e.CEP = '{model.Endereco_CEP}' AND
                    e.COMPLEMENTO = '{model.Endereco_Complemento}'
                ";


            var existeEndereco = await conn.QuerySingleAsync<int>(verificarEnderecoExiste);
            if (existeEndereco > 0)
                return Conflict(new { sucesso = false, mensagem = "Já existe um endereco cadastrado com esses dados." });
        }

        Endereco endereco = new Endereco()
        {
            Numero = model.Endereco_Numero,
            Rua = model.Endereco_Rua,
            Bairro = model.Endereco_Bairro,
            Cidade = model.Endereco_Cidade,
            CEP = model.Endereco_CEP,
            Complemento = model.Endereco_Complemento,
        };

        await conn.ExecuteAsync(inserirEnderecoQuery, endereco);
        
        return Ok( new
            {
                sucesso = true
            }    
        );
       

    }
    /*
    public async Task<IActionResult> Edit(int id)
    {
        var query = $"SELECT * FROM LISTA_ENDERECOS WHERE ID = {id}";
        var endereco = await _conn.QueryFirstOrDefaultAsync<ListaTelefonicaIACOApp.Models.EnderecoViewModel>(query);

        return endereco == null ? NotFound() : View(endereco);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Endereco endereco)
    {
        if (!ModelState.IsValid) return View(endereco);

        string query = $@"
            UPDATE LISTA_ENDERECOS SET 
                RUA = '{endereco.Rua}', 
                NUMERO = '{endereco.Numero}', 
                BAIRRO = '{endereco.Bairro}',
                CIDADE = '{endereco.Cidade}', 
                CEP = '{endereco.CEP}', 
                COMPLEMENTO = '{endereco.Complemento}'
            WHERE ID = {endereco.Id}";

        await _conn.ExecuteAsync(query);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var query = $"SELECT * FROM LISTA_ENDERECOS WHERE ID = {id}";
        var endereco = await _conn.QueryFirstOrDefaultAsync<Endereco>(query);

        return endereco == null ? NotFound() : View(endereco);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var query = $"DELETE FROM LISTA_ENDERECOS WHERE ID = {id}";
        await _conn.ExecuteAsync(query);
        return RedirectToAction(nameof(Index));
    }
   
    public async Task<IActionResult> Details(int id)
    {
        var query = $"SELECT * FROM LISTA_ENDERECOS WHERE ID = {id}";
        var endereco = await _conn.QueryFirstOrDefaultAsync<Endereco>(query);

        return endereco == null ? NotFound() : View(endereco);
    }

     */
}
