
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
            int count = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                count++;
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ListaTelefonicaDbContext>();

                using var conn = dbContext.CreateConnection();
                conn.Open();

                //Delete todos os logs cuja data tenha ultrapassado 1 mês
                var query = $@"DELETE FROM LogsAcoesUsuario
                               WHERE 
                                    DataHoraAcao < SYSDATE - 30";
                try
                {
                    await conn.ExecuteAsync(query);
                    await conn.ExecuteAsync("COMMIT");

                    _logger.LogInformation("BackgroundService sendo executado !");
                    _logger.LogInformation($@"{count}");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

            }

        }
    }
}