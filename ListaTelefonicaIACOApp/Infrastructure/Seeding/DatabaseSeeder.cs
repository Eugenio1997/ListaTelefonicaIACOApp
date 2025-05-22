using ListaTelefonicaIACOApp.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol.Plugins;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace ListaTelefonicaIACOApp.Infrastructure.Seeding
{
    public class DatabaseSeeder
    {

        public static void Seed(OracleConnection conn)
        {

            try
            {

                string verificaSeTabelaVaziaQuery = $"SELECT COUNT(*) FROM LISTA_FONES";

                var colaboradores = new List<(string Nome, string Fixo, string Celular, string Comercial, string Endereco, string Email)>
                {
                    ("Maria Oliveira", "2345-6789", "8765-4321", "2222-3333", "Rua B, 456", "MariaOliveira@iaco.com"),
                    ("Carlos Pereira", "3456-7890", "7654-3210", "3333-4444", "Rua C, 789", "CarlosPereira@iaco.com"),
                    ("Ana Costa", "4567-8901", "6543-2109", "4444-5555", "Rua D, 101", "AnaCosta@iaco.com"),
                    ("Lucas Santos", "5678-9012", "5432-1098", "5555-6666", "Rua E, 202", "LucasSantos@iaco.com"),
                    ("Fernanda Lima", "6789-0123", "4321-0987", "6666-7777", "Rua F, 303", "FernandaLima@iaco.com"),
                    ("Ricardo Almeida", "7890-1234", "3210-9876", "7777-8888", "Rua G, 404", "RicardoAlmeida@iaco.com"),
                    ("Patrícia Rocha", "8901-2345", "2109-8765", "8888-9999", "Rua H, 505", "PatriciaRocha@iaco.com"),
                    ("Bruno Martins", "9012-3456", "1098-7654", "9999-0000", "Rua I, 606", "BrunoMartins@iaco.com"),
                    ("Juliana Ferreira", "0123-4567", "0987-6543", "0000-1111", "Rua J, 707", "JulianaFerreira@iaco.com"),
                    ("Eduardo Souza", "1234-5678", "9876-5432", "1111-2222", "Rua K, 808", "EduardoSouza@iaco.com"),
                    ("Tatiane Mendes", "2345-6789", "8765-4321", "2222-3333", "Rua L, 909", "TatianeMendes@iaco.com"),
                    ("Gustavo Lima", "3456-7890", "7654-3210", "3333-4444", "Rua M, 1010", "GustavoLima@iaco.com"),
                    ("Sofia Martins", "4567-8901", "6543-2109", "4444-5555", "Rua N, 1111", "SofiaMartins@iaco.com"),
                    ("Felipe Almeida", "5678-9012", "5432-1098", "5555-6666", "Rua O, 1212", "FelipeAlmeida@iaco.com"),
                    ("Larissa Costa", "6789-0123", "4321-0987", "6666-7777", "Rua P, 1313", "LarissaCosta@iaco.com"),
                    ("André Rocha", "7890-1234", "3210-9876", "7777-8888", "Rua Q, 1414", "AndreRocha@iaco.com"),
                    ("Camila Santos", "8901-2345", "2109-8765", "8888-9999", "Rua R, 1515", "CamilaSantos@iaco.com"),
                    ("Leonardo Ferreira", "9012-3456", "1098-7654", "9999-0000", "Rua S, 1616", "LeonardoFerreira@iaco.com"),
                    ("Vanessa Lima", "0123-4567", "0987-6543", "0000-1111", "Rua T, 1717", "VanessaLima@iaco.com")
                };


                using var cmdVerificaSeVazia = new OracleCommand(verificaSeTabelaVaziaQuery, conn);
                using var cmdInserir = new OracleCommand(null, conn);

                int linhasAfetadas = Convert.ToInt32(cmdVerificaSeVazia.ExecuteScalar()); // retorna o número de linhas alteradas

                if (linhasAfetadas == 0)
                {
                    Console.WriteLine("Tabela LISTA_FONES vazia.");
                    Console.WriteLine("Aplicando Seeder ...");

                    foreach (var colaborador in colaboradores)
                    {
                        string nome = colaborador.Nome.Replace("'", "''");
                        string fixo = colaborador.Fixo.Replace("'", "''");
                        string celular = colaborador.Celular.Replace("'", "''");
                        string comercial = colaborador.Comercial.Replace("'", "''");
                        string endereco = colaborador.Endereco.Replace("'", "''");
                        string email = colaborador.Email.Replace("'", "''");

                        var query = $@"
                            INSERT INTO LISTA_FONES(ID, Nome, Fixo, Celular, Comercial, Endereco, Email)
                            VALUES (SEQ_LISTA_FONES.NEXTVAL, '{nome}', '{fixo}', '{celular}', '{comercial}', '{endereco}', '{email}')";

                        cmdInserir.CommandText = query;
                        cmdInserir.ExecuteNonQuery();
                    }

                    // Commit final único
                    using var commitCmd = conn.CreateCommand();
                    commitCmd.CommandText = "COMMIT";
                    commitCmd.ExecuteNonQuery();

                    Console.WriteLine("Seeder aplicado com sucesso!");

                }
                else
                {
                    Console.WriteLine("Tabela já populada.");
                }


            }
            catch (OracleException ex)
            {

                throw new Exception($"Erro ao aplicar o Seeder: {ex.Message}", ex);
            }

            finally
            {

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }
    }
}