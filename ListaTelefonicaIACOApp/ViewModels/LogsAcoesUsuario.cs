namespace ListaTelefonicaIACOApp.ViewModels
{
    public class LogsAcoesUsuario
    {
        public int Id { get; set; }                     // Identificador único do log
        public int UsuarioId { get; set; }              // ID do usuário que realizou a ação
        public string Acao { get; set; }                 // Nome ou tipo da ação realizada
        public DateTime DataHoraAcao { get; set; }       // Momento em que a ação ocorreu
        public string EnderecoIp { get; set; }           // Endereço IP do usuário
        public string UserAgent { get; set; }            // Informações do navegador/dispositivo
        public string DetalhesRegistroAfetado { get; set; } // Informações adicionais (em JSON ou texto)
        public ResultadoAcao Sucesso { get; set; }                // Indica se a ação foi bem-sucedida
        public string MensagemErro { get; set; }         // Mensagem de erro, caso a ação tenha falhado
    }
}
