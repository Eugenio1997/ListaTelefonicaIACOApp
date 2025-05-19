using ListaTelefonicaIACOApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace ListaTelefonicaIACOApp.Controllers
{
    public class ColaboradorController : Controller
    {

        private readonly IConfiguration _configuration;

        // GET: ColaboradorController
        public ActionResult Index()
        {
            var lista = new List<Colaborador>();
            var connectionString = _configuration.GetConnectionString("ListaTelefonicaIACOConnectionString");

            using (var conn = new OracleConnection(connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT Id, Nome, Celular, Fixo, Endereco, Email FROM Colaboradores";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Colaborador
                        {
                            Id = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            Celular = reader.GetString(2),
                            Fixo = reader.GetString(3),
                            Endereco = reader.GetString(4),
                            Email = reader.GetString(4)
                        });
                    }
                }
            }

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
