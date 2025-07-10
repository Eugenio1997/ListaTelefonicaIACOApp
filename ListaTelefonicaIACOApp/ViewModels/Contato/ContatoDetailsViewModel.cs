using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.ViewModels.Contato
{
    public class ContatoDetailsViewModel
    {
        // Dados pessoais
        public string Nome { get; set; } = string.Empty;
        public string Fixo { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        [DisplayName("Ramal")]
        public string Comercial { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;

    }
}
