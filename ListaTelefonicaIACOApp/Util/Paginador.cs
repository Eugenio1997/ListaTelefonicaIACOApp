using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CRUD_cliente_IACO.Util
{


    public class PaginacaoResultado<T>
    {
        public List<T> Registros { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }

    }

    public static class Paginador
    {
        public static PaginacaoResultado<T> ExecutarPaginacao<T>(
            string tabelaOuView,
            string ordenacao,
            int paginaAtual,
            int registrosPorPagina,
            Func<IDataReader, T> mapeador,
            OracleConnection conexao)
        {
            var resultado = new PaginacaoResultado<T>();
            var registros = new List<T>();

            Console.WriteLine(tabelaOuView + "\n" + ordenacao + "\n" + paginaAtual + "\n" + registrosPorPagina);
            int inicio = (paginaAtual - 1) * registrosPorPagina;
            int fim = paginaAtual * registrosPorPagina;

            // Query para total de registros
            string queryTotal = $"SELECT COUNT(*) FROM {tabelaOuView}";

            using (var cmdTotal = conexao.CreateCommand())
            {
                cmdTotal.CommandText = queryTotal;
                resultado.TotalRegistros = Convert.ToInt32(cmdTotal.ExecuteScalar());
                resultado.TotalPaginas = (int)Math.Ceiling((double)resultado.TotalRegistros / registrosPorPagina);
            }

            // Query de paginação (modelo Oracle com ROWNUM)
            string queryPaginada = $@"
                SELECT * FROM (     
                    SELECT c.*, ROWNUM rnum
                    FROM (
                        SELECT * FROM {tabelaOuView} ORDER BY {ordenacao}
                    ) c
                    WHERE ROWNUM <= :fim
                )
                WHERE rnum > :inicio";

            using (var cmd = conexao.CreateCommand())
            {
                cmd.CommandText = queryPaginada;

                cmd.Parameters.Add(new OracleParameter("fim", fim));
                cmd.Parameters.Add(new OracleParameter("inicio", inicio));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        registros.Add(mapeador(reader));
                    }
                }
            }

            resultado.Registros = registros;
            return resultado;
        }
    }
}
