using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.ViewModels.Contato
{
    public class ContatoEditViewModel
    {
        // Dados pessoais

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [RegularExpression(@"^(?=.{3,})(?!.*\d)[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O Nome deve ter no mínimo 3 letras e não pode conter números.")]
        public string Nome { get; set; }
        //[RegularExpression(@"^\(\d{2}\) \d{4}-\d{4}$", ErrorMessage = "Formato esperado: 99 9999-9999")]
        public string? Fixo { get; set; }

        //[RegularExpression(@"^\(\d{2}\) \d{5}-\d{4}$", ErrorMessage = "Celular deve estar no formato 99 99999-9999")]
        public string? Celular { get; set; }
        [DisplayName("Ramal")]
        //[RegularExpression(@"^\d{4}$", ErrorMessage = "Formato esperado: 0000")]
        public string? Comercial { get; set; }

        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "O endereço é Obrigatório.")]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }
        public List<ContatoIndexViewModel> Contatos { get; set; } = new();
        [Display(Name = "Criado Em")]
        public string CriadoAs { get; set; } = string.Empty;

        [Display(Name = "Editado Em")]
        public string EditadoAs { get; set; } = string.Empty;
    }
}
