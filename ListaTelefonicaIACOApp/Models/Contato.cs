using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.Models
{
    public class Contato
    {

        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Fixo { get; set; }
        public string? Celular { get; set; }
        public string? Comercial { get; set; }
        public string Endereco { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string CriadoAs { get; set; } = string.Empty;
        public string EditadoAs { get; set; } = string.Empty;

    }
}
