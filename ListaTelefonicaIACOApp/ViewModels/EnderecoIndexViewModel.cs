using System.Collections.Generic;

namespace ListaTelefonicaIACOApp.ViewModels
{
    public class EnderecoIndexViewModel
    {
        // Lista de endereços que será exibida na tabela
        public List<EnderecoIndexViewModel> Enderecos { get; set; } = new List<EnderecoIndexViewModel>();

        // Filtros usados no formulário
        public int? Id { get; set; }
        public string? Rua { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? CEP { get; set; }
        public string? Complemento { get; set; }
        public DateTime CriadoAs { get; set; }
        public DateTime? EditadoAs { get; set; }
    }

}