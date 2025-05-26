using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.ViewModels
{
    public class ContatoViewModel
    {

        // Dados pessoais
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [RegularExpression(@"^(?=.{3,})(?!.*\d)[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O Nome deve ter no mínimo 3 letras e não pode conter números.")]
        public string Nome { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Sobrenome é obrigatório.")]
        [RegularExpression(@"^(?=.{3,})(?!.*\d)[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O Sobrenome deve ter no mínimo 3 letras e não pode conter números.")]
        public string Sobrenome { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Fixo é obrigatório.")]
        [RegularExpression(@"^\(\d{2}\) \d{4}-\d{4}$", ErrorMessage = "Formato esperado: (99) 9999-9999")]
        public string Fixo { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Celular é obrigatório.")]
        [RegularExpression(@"^\(\d{2}\) \d{9}$", ErrorMessage = "Celular deve estar no formato (99) 999999999")]
        public string Celular { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Comercial é obrigatório.")]
        [RegularExpression(@"^\(\d{2}\) \d{4}-\d{4}$", ErrorMessage = "Formato esperado: (99) 9999-9999")]
        public string Comercial { get; set; } = string.Empty;
        [Required(ErrorMessage = "O e-mail é Obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = string.Empty;


        // Endereço
        [Required(ErrorMessage = "A rua é obrigatória.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A rua deve ter entre 3 e 100 caracteres.")]
        public string Rua { get; set; }

        [Required(ErrorMessage = "O número é obrigatório.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "O número deve conter apenas dígitos.")]
        public string Numero { get; set; }

        [Required(ErrorMessage = "O bairro é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O bairro deve ter entre 2 e 100 caracteres.")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "O CEP deve estar no formato 00000000.")]
        public string CEP { get; set; }
        public string? Complemento { get; set; }
        public List<ContatoViewModel> Colaboradores { get; set; } = new();
    }
}
