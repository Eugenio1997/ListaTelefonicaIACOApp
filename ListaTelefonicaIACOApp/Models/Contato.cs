using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.Models
{
    public class Contato
    {

        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Fixo { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public string Comercial { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //Um contato possui associado a ele um endereco
        //public int EnderecoId { get; set; }
        //public Endereco? Endereco { get; set; }
        public string CriadoAs { get; set; } = string.Empty;
        public string EditadoAs { get; set; } = string.Empty;

    }
}
