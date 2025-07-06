
using Dapper;
using ListaTelefonicaIACOApp.Infrastructure;
using ListaTelefonicaIACOApp.ViewModels.Contato;
using Microsoft.Extensions.Logging;

namespace ListaTelefonicaIACOApp.BackgroundServices
{
    public class LogLimpezaBackgroundService : BackgroundService
    {
        private readonly ILogger<LogLimpezaBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public LogLimpezaBackgroundService(
            ILogger<LogLimpezaBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de limpeza de logs iniciado.");


            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation($"Serviço de limpeza executado em: {DateTime.Now}");

                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ListaTelefonicaDbContext>();

                    using var conn = dbContext.CreateConnection();

                    if (conn.State != System.Data.ConnectionState.Open)
                        conn.Open();


                    //Delete todos os logs cuja data tenha ultrapassado 1 mês
                    var query = $@"DELETE FROM LOGSACOESUSUARIOS
                                   WHERE 
                                        DataHoraAcao < SYSDATE - 30";

                    var rowsAfetadas = await conn.ExecuteAsync(query);
                    _logger.LogInformation("Limpeza concluída. {Rows} registros removidos.", rowsAfetadas);
                    await conn.ExecuteAsync("COMMIT");

                    _logger.LogInformation("BackgroundService sendo executado !");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro durante a execução da limpeza de logs.");
                }

                await Task.Delay(TimeSpan.FromDays(30), stoppingToken);

            }

            _logger.LogInformation("Serviço de limpeza de logs encerrado.");

        }
    }
}