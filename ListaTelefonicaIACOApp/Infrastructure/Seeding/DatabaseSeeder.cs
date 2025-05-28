using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using Dapper;
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

                int contatosExistem = conn.QuerySingle<int>("SELECT COUNT(*) FROM LISTA_CONTATOS");
                if (contatosExistem > 0)
                {
                    Console.WriteLine("Seeder ignorado: registros já existem.");
                    return;
                }

                var enderecos = new List<(string Rua, string Numero, string Bairro, string Cidade, string CEP, string Complemento)>
                {
                    ("Rua B", "456", "Centro", "São Paulo", "01001-000", "Ap 12"),
                    ("Rua C", "789", "Jardins", "São Paulo", "01002-000", ""),
                    ("Rua D", "101", "Vila Mariana", "São Paulo", "01003-000", ""),
                    ("Rua E", "202", "Tatuapé", "São Paulo", "01004-000", ""),
                    ("Rua F", "303", "Moema", "São Paulo", "01005-000", ""),
                    ("Rua G", "404", "Pinheiros", "São Paulo", "01006-000", ""),
                    ("Rua H", "505", "Santana", "São Paulo", "01007-000", ""),
                    ("Rua I", "606", "Lapa", "São Paulo", "01008-000", ""),
                    ("Rua J", "707", "Ipiranga", "São Paulo", "01009-000", ""),
                    ("Rua K", "808", "Perdizes", "São Paulo", "01010-000", "")
                };

                var enderecoIds = new List<int>();

                foreach (var e in enderecos)
                {
                    string insertEndereco = $@"
                        INSERT INTO LISTA_ENDERECOS (RUA, NUMERO, BAIRRO, CIDADE, CEP, COMPLEMENTO)
                        VALUES ('{e.Rua}', '{e.Numero}', '{e.Bairro}', '{e.Cidade}', '{e.CEP}', '{e.Complemento}')
                    ";

                    conn.Execute(insertEndereco);

                    int id = conn.QuerySingle<int>("SELECT MAX(ID) FROM LISTA_ENDERECOS");
                    enderecoIds.Add(id);
                }

                var contatos = new List<(string Nome, string Sobrenome, string Fixo, string Celular, string Comercial, string Email)>
                {
                    ("Maria", "Oliveira", "2345-6789", "8765-4321", "2222-3333", "MariaOliveira@iaco.com"),
                    ("Carlos", "Pereira", "3456-7890", "7654-3210", "3333-4444", "CarlosPereira@iaco.com"),
                    ("Ana", "Costa", "4567-8901", "6543-2109", "4444-5555", "AnaCosta@iaco.com"),
                    ("Lucas", "Santos", "5678-9012", "5432-1098", "5555-6666", "LucasSantos@iaco.com"),
                    ("Fernanda", "Lima", "6789-0123", "4321-0987", "6666-7777", "FernandaLima@iaco.com"),
                    ("Ricardo", "Almeida", "7890-1234", "3210-9876", "7777-8888", "RicardoAlmeida@iaco.com"),
                    ("Patrícia", "Rocha", "8901-2345", "2109-8765", "8888-9999", "PatriciaRocha@iaco.com"),
                    ("Bruno", "Martins", "9012-3456", "1098-7654", "9999-0000", "BrunoMartins@iaco.com"),
                    ("Juliana", "Ferreira", "0123-4567", "0987-6543", "0000-1111", "JulianaFerreira@iaco.com"),
                    ("Eduardo", "Souza", "1234-5678", "9876-5432", "1111-2222", "EduardoSouza@iaco.com"),
                };

                for (int i = 0; i < contatos.Count; i++)
                {
                    var c = contatos[i];
                    int enderecoId = enderecoIds[i];

                    string insertContato = $@"
                        INSERT INTO LISTA_CONTATOS 
                        (NOME, SOBRENOME, FIXO, CELULAR, COMERCIAL, ENDERECO_ID, EMAIL)
                        VALUES (
                            '{c.Nome}', 
                            '{c.Sobrenome}', 
                            '{c.Fixo}', 
                            '{c.Celular}', 
                            '{c.Comercial}', 
                            {enderecoId}, 
                            '{c.Email}'
                        )";

                    conn.Execute(insertContato);
                }

                conn.Execute("COMMIT");

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
