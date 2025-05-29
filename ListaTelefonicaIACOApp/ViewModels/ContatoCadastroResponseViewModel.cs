using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.ViewModels
{
    public class ContatoCadastroResponseViewModel
    {
        // Dados pessoais
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [RegularExpression(@"^(?=.{3,})(?!.*\d)[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O Nome deve ter no mínimo 3 letras e não pode conter números.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Sobrenome é obrigatório.")]
        [RegularExpression(@"^(?=.{3,})(?!.*\d)[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O Sobrenome deve ter no mínimo 3 letras e não pode conter números.")]
        public string Sobrenome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Fixo é obrigatório.")]
        [RegularExpression(@"^\d{8,15}$", ErrorMessage = "O Fixo deve conter apenas dígitos (8 a 15).")]
        public string Fixo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Celular é obrigatório.")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "O Celular deve conter apenas dígitos (10 a 15).")]
        public string Celular { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Comercial é obrigatório.")]
        [RegularExpression(@"^\d{8,15}$", ErrorMessage = "O Comercial deve conter apenas dígitos (8 a 15).")]
        public string Comercial { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        // Endereço
        [Required(ErrorMessage = "A rua é obrigatória.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A rua deve ter entre 3 e 100 caracteres.")]
        public string Rua { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número é obrigatório.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "O número deve conter apenas dígitos.")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "O bairro é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O bairro deve ter entre 2 e 100 caracteres.")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
        public string Cidade { get; set; } = string.Empty;

        [RegularExpression(@"^\d{8}$", ErrorMessage = "O CEP deve conter 8 dígitos (sem traço).")]
        public string? CEP { get; set; }

        public string? Complemento { get; set; }

        //public List<ContatoCadastroRequestViewModel> Contatos { get; set; } = new();
    }
}
