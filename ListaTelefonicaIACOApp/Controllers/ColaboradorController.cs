using ListaTelefonicaIACOApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace ListaTelefonicaIACOApp.Controllers
{
    public class ColaboradorController : Controller
    {
        private readonly ILogger<ColaboradorController> _logger;
        private readonly IConfiguration? _configuration;
        int registrosPorPagina = 10;
        int TotalRegistros;
        int TotalPaginas;
        int offset;
        public ColaboradorController(ILogger<ColaboradorController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // GET: ColaboradorController
        public ActionResult Index(int paginaAtual = 1)
        {
            int registrosPorPagina = 10;
            int offset = (paginaAtual - 1) * registrosPorPagina;

            var lista = new List<Colaborador>();
            int totalRegistros = 0;
            int totalPaginas = 0;

            var connectionString = _configuration?.GetConnectionString("ListaTelefonicaIACOConnectionString");

            try
            {
                using (var conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    // 1. Obter total de registros
                    using (var countadorRegistrosCmd = conn.CreateCommand())
                    {
                        countadorRegistrosCmd.CommandText = "SELECT COUNT(*) FROM LISTA_FONES";
                        totalRegistros = Convert.ToInt32(countadorRegistrosCmd.ExecuteScalar());
                    }

                    totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                    // 2. Buscar página atual com SQL paginado
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"
                            SELECT *
                            FROM LISTA_FONES
                            ORDER BY ID
                            OFFSET {offset} ROWS FETCH NEXT {registrosPorPagina} ROWS ONLY";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Colaborador
                                {
                                    Id = reader.GetInt32(0),
                                    Nome = reader.GetString(1),
                                    Fixo = reader.GetString(2),
                                    Celular = reader.GetString(3),
                                    Comercial = reader.GetString(4),
                                    Endereco = reader.GetString(5),
                                    Email = reader.GetString(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (OracleException)
            {
                throw; // ou log e redirect para uma página de erro amigável
            }

            ViewBag.PaginaAtual = paginaAtual;
            ViewBag.TotalPaginas = totalPaginas;

            return View(lista);
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

        // GET: ColaboradorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ColaboradorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ColaboradorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
