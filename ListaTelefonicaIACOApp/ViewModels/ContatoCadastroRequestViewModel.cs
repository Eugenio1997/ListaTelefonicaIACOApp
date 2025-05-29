using System.ComponentModel.DataAnnotations;

namespace ListaTelefonicaIACOApp.ViewModels
{
    public class ContatoCadastroRequestViewModel
    {

        // Dados pessoais

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [RegularExpression(@"^(?=.{3,})(?!.*\d)[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O Nome deve ter no mínimo 3 letras e não pode conter números.")]
        public string Nome { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Sobrenome é obrigatório.")]
        [RegularExpression(@"^(?=.{3,})(?!.*\d)[A-Za-zÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "O Sobrenome deve ter no mínimo 3 letras e não pode conter números.")]
        public string Sobrenome { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Fixo é obrigatório.")]
        [RegularExpression(@"^\(\d{2}\) \d{4}-\d{4}$", ErrorMessage = "Formato esperado: 99 99999999")]
        public string Fixo { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Celular é obrigatório.")]
        [RegularExpression(@"^\(\d{2}\) \d{5}-\d{4}$", ErrorMessage = "Celular deve estar no formato 99 999999999")]
        public string Celular { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo Comercial é obrigatório.")]
        [RegularExpression(@"^\(\d{2}\) \d{4}-\d{4}$", ErrorMessage = "Formato esperado: 99 99999999")]
        public string Comercial { get; set; } = string.Empty;
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = string.Empty;
        public int EnderecoId { get; set; }

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

        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "O CEP deve estar no formato 00000000.")]
        public string? CEP { get; set; }
        public string? Complemento { get; set; }
        public List<ContatoCadastroRequestViewModel> Contatos { get; set; } = new();
    }
}
