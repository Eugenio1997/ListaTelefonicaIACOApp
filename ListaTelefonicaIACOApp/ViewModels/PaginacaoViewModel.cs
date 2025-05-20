namespace ListaTelefonicaIACOApp.ViewModels
{
    public class PaginacaoViewModel
    {
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
    }
}
