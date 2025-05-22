using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace ListaTelefonicaIACOApp.Controllers
{
    public class ContatoController : Controller
    {
        private readonly ILogger<ContatoController> _logger;
        private readonly IConfiguration? _configuration;
        int registrosPorPagina = 10;
        int TotalRegistros;
        int TotalPaginas;
        int offset;
        public ContatoController(ILogger<ContatoController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private ContatoViewModel ObterColaboradoresPaginados(int pagina, int registrosPorPagina, out int totalPaginas)
        {
            int offset = (pagina - 1) * registrosPorPagina;
            int totalRegistros = 0;
            totalPaginas = 0;

            var lista = new ContatoViewModel();
            var connectionString = _configuration?.GetConnectionString("ListaTelefonicaIACOConnectionString");

            using (var conn = new OracleConnection(connectionString))
            {
                conn.Open();

                using (var countCmd = conn.CreateCommand())
                {
                    countCmd.CommandText = "SELECT COUNT(*) FROM LISTA_FONES";
                    totalRegistros = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

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
                            lista.Colaboradores.Add(new ContatoViewModel
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

            return lista;
        }

        // GET: ColaboradorController
        public ActionResult Index(int paginaAtual = 1)
        {
            int registrosPorPagina = 10;
            var lista = ObterColaboradoresPaginados(paginaAtual, registrosPorPagina, out int totalPaginas);

            ViewBag.PaginaAtual = paginaAtual;
            ViewBag.TotalPaginas = totalPaginas;

            return View(lista);
        }

        public IActionResult PaginaDados(int paginaAtual = 1)
        {


            int registrosPorPagina = 10;
            var lista = ObterColaboradoresPaginados(paginaAtual, registrosPorPagina, out int totalPaginas);

            ViewBag.PaginaAtual = paginaAtual;
            ViewBag.TotalPaginas = totalPaginas;

            return PartialView("_TabelaColaboradores", lista);
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
        public ActionResult FiltrarPorContatos(IFormCollection collection)
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
