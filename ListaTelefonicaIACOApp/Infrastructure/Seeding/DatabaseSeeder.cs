using Dapper;
using ListaTelefonicaIACOApp.Models;
using Microsoft.AspNetCore.Identity;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ListaTelefonicaIACOApp.Infrastructure.Seeding
{
    public class DatabaseSeeder
    {
        public static void Seed(OracleConnection conn)
        {

            if (conn == null || conn.State != ConnectionState.Open)
            {
                conn?.Open();
            }

            try
            {
                Console.WriteLine("Aplicando Seeder...");

                int usuariosExistem = conn.QuerySingle<int>("SELECT COUNT(*) FROM USUARIOS");
                if (usuariosExistem > 0)
                {
                    Console.WriteLine("Seeder ignorado: usuarios já existem.");
                    return;
                }

                var usuarios = new Usuario[]
                {
                    new Usuario { Nome = "guarita-nucleo", Senha = "gu4r1t4-nucl30", Role = "Guarita" },
                    new Usuario { Nome = "guarita-industria", Senha = "gu4r1t4-industr14", Role = "Guarita" },
                    new Usuario { Nome = "recepcao", Senha = "r3c3pc4o", Role = "Recepcao" },
                    new Usuario { Nome = "admin", Senha = "4dmln", Role = "Administrador" },
                };

                var hasher = new PasswordHasher<Usuario>();

                foreach (var u in usuarios)
                {
                    var usuario = new Usuario { Nome = u.Nome, Role = u.Role };
                    var senhaHash = hasher.HashPassword(usuario, u.Senha);

                    conn.Execute($@"
                    INSERT INTO USUARIOS (NOME, SENHA, ROLE) VALUES 
                        ('{u.Nome}', '{senhaHash}', '{u.Role}')
                    ");

                    conn.Execute("COMMIT");
                }

                Console.WriteLine("Seeder aplicado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
    }
}
