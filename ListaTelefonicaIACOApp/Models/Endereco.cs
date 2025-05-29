using ListaTelefonicaIACOApp.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.Models
{
    public class Endereco
    {
        public int Id { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string? CEP { get; set; } = string.Empty;
        public string? Complemento { get; set; }

        //Um endereco contém uma lista de contatos
        public List<ContatoCadastroRequestViewModel> Contatos { get; set; } = new();

        public DateTime CriadoAs { get; set; }
        public DateTime? EditadoAs { get; set; }
    }
}
